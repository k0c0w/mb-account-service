using AccountService.Domain;
using AccountService.Persistence.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Persistence.Jobs;

public class DailyInterestAccrualJob(AccountServiceDbContext dbContext, IAccountInterestRewarder rewarder)
{
    public const string Name = "daily-interest-reward";
    
    public async Task RunAsync(CancellationToken ct)
    {
        var accountsIdEnumeration = dbContext.Accounts
            .Where(a => a.ClosingTimeUtc == null && a.Type != AccountType.Checking)
            .Select(a => a.Id)
            .AsAsyncEnumerable()
            .WithCancellation(ct);
        
        await foreach (var id in accountsIdEnumeration)
        {
            try
            {
                await rewarder.AccrueInterestAsync(id, ct);
            }
            catch
            {
                continue;
                //todo: retry policy
            }
        }
    }
}