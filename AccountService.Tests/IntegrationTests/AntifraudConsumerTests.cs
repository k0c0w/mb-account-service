using System.Text;
using AccountService.Features.Domain;
using AccountService.Features.TransactionFeatures.RegisterExternalTransaction;
using AccountService.Persistence.Infrastructure.DataAccess;
using AccountService.Tests.Factories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace AccountService.Tests.IntegrationTests;

public class AntifraudConsumerTests(WebAppFactory factory) : RabbitMqIntegrationTestBase, IClassFixture<WebAppFactory>
{
    private readonly IMediator _mediator = factory.Services.GetRequiredService<IMediator>();

    private readonly AccountServiceDbContext
        _dbContext = factory.Services.GetRequiredService<AccountServiceDbContext>();

    [Fact]
    public async Task ClientBlockedPreventsDebit()
    {
        var eventId = Guid.CreateVersion7();
        var clientId = Guid.Parse("88bc045c-ecc8-4cb7-8cd9-b28938abef55");
        var account = AccountsFactory.CreateAccount(
            accountType: AccountType.Checking,
            balance: 0,
            ownerId: clientId,
            accountId: Guid.CreateVersion7(),
            currencyCode: new CurrencyCode("RUB")
        );
        await _dbContext.Accounts.AddAsync(account);
        await _dbContext.SaveChangesAsync();

        // Arrange Declare queue and bind to exchange
        await using var connection = await factory.GetRabbitMqConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        var props = new BasicProperties
        {
            MessageId = eventId.ToString(),
            CorrelationId = eventId.ToString(),
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };
        const string message = """
                                   {"id":"88bc045c-ecc8-4cb7-8cd9-b28938abef55", 
                                         "occuredAt":"2025-08-18T23:05:00Z", 
                                         "payload":{"clientId":"88bc045c-ecc8-4cb7-8cd9-b28938abef55"},
                                         "meta": {
                                           "version":"v1",
                                           "correlationId": "88bc045c-ecc8-4cb7-8cd9-b28938abef55",
                                           "causationId": "88bc045c-ecc8-4cb7-8cd9-b28938abef55"
                                         }
                                   }
                               """;
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync("account.events", "client.blocked", true, props, body, CancellationToken.None);

        var debitCommand = new RegisterExternalTransactionCommand
        {
            Amount = 5m,
            AccountId = account.Id,
            CurrencyCode = account.Balance.Code.Value,
            TransactionType = TransactionType.Debit
        };

        //Act
        // Wait for AntifraudConsumer to process (adjust based on consumer speed)
        await Task.Delay(TimeSpan.FromSeconds(60));
        _dbContext.ChangeTracker.Clear();
        Task Action() => _mediator.Send(debitCommand);

        var ex = await Assert.ThrowsAsync<DomainException>(Action);
        Assert.Equal(DomainException.DomainExceptionType.ConflictError, ex.Type);

        var publishedEventsCount = await _dbContext.OutboxMessages
            .Where(x => x.ProcessedAtUtc == null)
            .CountAsync();
        Assert.Equal(0, publishedEventsCount);

        var eventReceived =
            await ConsumeEventAsync<MoneyDebitedEvent>(channel, "account.notifications", TimeSpan.FromSeconds(10));

        Assert.Null(eventReceived);
    }

    public record MoneyDebitedEvent(
        Guid Id,
        DateTimeOffset OccuredAt,
        MoneyDebitedEventPayload Payload);

    public record MoneyDebitedEventPayload(decimal Amount, string Currency, string OperationId, Guid AccountId);
}