using System.Data;
using AccountService.Features.Domain.Services;
using AccountService.Persistence.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Persistence.Services.Domain;

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