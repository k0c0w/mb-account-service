using AccountService.Features.Domain;
using AccountService.Features.TransfersFeatures.TransferMoney;
using AccountService.Persistence.Infrastructure.DataAccess;
using AccountService.Tests.Factories;
using Bogus;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Tests.IntegrationTests;

public class TransferMoneyTest(WebAppFactory factory) : IClassFixture<WebAppFactory>
{
    private readonly IMediator _mediator = factory.Services.GetRequiredService<IMediator>();
    private readonly AccountServiceDbContext _dbContext = factory.Services.GetRequiredService<AccountServiceDbContext>();
    private readonly Randomizer _randomizer = new Faker().Random;

    [Fact]
    public async Task ParallelTransactionsMustPersistConsistency()
    {
        // Arrange
        const int concurrentTasks = 50; 
        const AccountType accountType = AccountType.Checking;
        var currency = new CurrencyCode("RUB");
        var sender = AccountsFactory.CreateAccount(accountType, 50_000m, currencyCode: currency);
        var recipient = AccountsFactory.CreateAccount(accountType, 1234.56m, currencyCode: currency);

        _dbContext.AddRange(sender, recipient);
        await _dbContext.SaveChangesAsync();
        
        var initialTotalMoney = sender.Balance.Amount + recipient.Balance.Amount;

        //Act
        var tasks = new Task[concurrentTasks];
        for (var i = 0; i < concurrentTasks; i++)
        {
            tasks[i] = RequestFactory();
        }

        await Task.WhenAll(tasks);

        // Assert
        var actualBalancesSum = await _dbContext.Accounts
            .Where(a => a.Id == sender.Id || a.Id == recipient.Id)
            .Select(a => a.Balance.Amount)
            .SumAsync();
        Assert.Equal(initialTotalMoney, actualBalancesSum);
        
        return;

        async Task RequestFactory()
        {
            var command = new TransferMoneyCommand
            {
                RecipientAccountId = recipient.Id, 
                SenderAccountId = sender.Id, 
                Amount = _randomizer.Decimal(min: 1m, max: 1000m)
            };
            try
            {
                await _mediator.Send(command);
            }
            catch
            {
                // ignored
            }
        }
    }
}