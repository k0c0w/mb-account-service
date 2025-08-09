using System.Data;
using AccountService.Domain;
using AccountService.Persistence.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Persistence.Services;

public class AccountInterestAwarder(AccountServiceDbContext dbContext) : IAccountInterestAwarder
{
    public async Task AccrueInterestAsync(Guid accountId, CancellationToken ct = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        try
        {
            await dbContext.Database.ExecuteSqlRawAsync("CALL accrue_interest(@accountId);", accountId, ct);    
            await transaction.CommitAsync(ct);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}