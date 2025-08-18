using AccountService.Features.AccountFeatures.CreateNewAccount;
using AccountService.Features.Domain;
using AccountService.Persistence.Infrastructure.DataAccess;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Tests.IntegrationTests;

public abstract class OutboxPatternTests(WebAppFactory factory) : RabbitMqIntegrationTestBase, IClassFixture<WebAppFactory>
{
    private readonly IMediator _mediator = factory.Services.GetRequiredService<IMediator>();

    private readonly AccountServiceDbContext
        _dbContext = factory.Services.GetRequiredService<AccountServiceDbContext>();

    [Fact]
    public async Task OutboxPublishesAfterFailure()
    {
        // Arrange: Declare queue and bind to exchange


        var command = new CreateNewAccountCommand
        {
            AccountType = AccountType.Checking,
            CurrencyCode = "RUB",
            OwnerId = Guid.Parse("88bc045c-ecc8-4cb7-8cd9-b28938abef55")
        };

        await factory.RabbitMqContainer.PauseAsync();

        // Act: Create an account (stores AccountOpenedEvent in outbox)
        var createdAccount = await _mediator.Send(command);

        // Assert: Event is in outbox and not processed
        var outboxCount = await _dbContext.OutboxMessages
            .Where(o => o.ProcessedAtUtc == null)
            .CountAsync();
        Assert.True(outboxCount > 0);
        await factory.RabbitMqContainer.UnpauseAsync();
        await Task.Delay(TimeSpan.FromSeconds(30));

        outboxCount = await _dbContext
            .OutboxMessages
            .Where(o => o.ProcessedAtUtc == null)
            .CountAsync();

        Assert.True(outboxCount == 0);

        await using var connection = await factory.GetRabbitMqConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        var eventReceived = await ConsumeEventAsync<AccountOpenedEvent>(channel, "account.crm", TimeSpan.FromSeconds(10));

        Assert.NotNull(eventReceived);
        Assert.Equal(createdAccount.Id, eventReceived.Payload.AccountId);
        Assert.Equal(createdAccount.OwnerId, eventReceived.Payload.OwnerId);
        Assert.Equal(createdAccount.Currency, eventReceived.Payload.Currency);
        Assert.Equal(createdAccount.Type.ToString(), eventReceived.Payload.Type);
    }
    
    [UsedImplicitly]
    private record AccountOpenedEvent(
        Guid Id,
        DateTimeOffset OccuredAt,
        AccountOpenedEventPayload Payload);

    [UsedImplicitly]
    public abstract record AccountOpenedEventPayload(Guid OwnerId, string Currency, string Type, Guid AccountId);
}