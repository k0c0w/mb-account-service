using System.Data;
using AccountService.Features.DataAccess;
using AccountService.Features.Domain;
using AccountService.Features.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Persistence.Services;

public class AccountInterestRewarder(AccountServiceDbContext dbContext) : IAccountInterestRewarder
{
    public async Task AccrueInterestAsync(Guid accountId, CancellationToken ct = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        try
        {
            await dbContext.Database.ExecuteSqlRawAsync("CALL accrue_interest({0});", accountId);    
            await transaction.CommitAsync(ct);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}