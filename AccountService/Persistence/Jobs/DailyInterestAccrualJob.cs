using AccountService.Domain;
using AccountService.Persistence.DataAccess;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Persistence.Jobs;

[UsedImplicitly]
public class DailyInterestAccrualJob(IDbContextFactory<AccountServiceDbContext> dbContextFactory, 
    IAccountInterestRewarder interestRateRewardService,
    ILogger<DailyInterestAccrualJob> jobLogger)
{
    public const string Name = "daily-interest-reward";
    
    public async Task RunAsync(CancellationToken ct)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(ct);
        var accountsIdEnumeration = dbContext.Accounts
            .Where(a => a.ClosingTimeUtc == null && a.Type != AccountType.Checking)
            .Select(a => a.Id)
            .AsAsyncEnumerable()
            .WithCancellation(ct);
        
        await foreach (var id in accountsIdEnumeration)
        {
            try
            {
                await interestRateRewardService.AccrueInterestAsync(id, ct);
            }
            catch(Exception ex)
            {
                jobLogger.LogError(ex, "An exception during job: {Message}", ex.Message);
                //todo: retry policy
            }
        }
    }
}