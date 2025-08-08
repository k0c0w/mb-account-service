using AccountService.Domain;
using AccountService.Persistence.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Persistence.Services;

public class AccountInterestAwarder(AccountServiceDbContext dbContext) : IAccountInterestAwarder
{
    public async Task AccrueInterestAsync(Guid accountId, CancellationToken ct = default)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            await dbContext.Database.ExecuteSqlRawAsync("CALL AccrueInterest(@accountId);", accountId, ct);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}