using AccountService.Domain;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Persistence.DataAccess;

public static class AccountQueryableExtensions
{
    public static Task<Account?> FindByIdAsync(this IQueryable<Account> accounts, Guid id,
        CancellationToken ct = default)
        => accounts.FirstOrDefaultAsync(a => a.Id == id, ct);
    
    public static IQueryable<Account> WithOwnerId(this IQueryable<Account> accounts, Guid ownerId)
        => accounts.Where(a => a.OwnerId == ownerId);
}