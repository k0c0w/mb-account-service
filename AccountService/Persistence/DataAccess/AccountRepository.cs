using System.Collections.Concurrent;
using AccountService.Domain;

namespace AccountService.Persistence.DataAccess;

public class AccountRepository : IAccountRepository
{
    private static readonly ConcurrentDictionary<Guid, Account> Accounts = [];
    
    public Task AddAsync(Account account, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        
        Accounts.TryAdd(account.Id, account);
        
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Account account, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        
        Accounts.AddOrUpdate(account.Id, _ => account, (_, _) => account);
        
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Account account, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        
        Accounts.Remove(account.Id, out _);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Account>> FindAsync(IAccountRepository.FindAccountsFilter filter, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        
        IReadOnlyList<Account> res = [];
        switch (filter)
        {
            case IAccountRepository.FindAccountsFilter.ByIdFilter byIdFilter:
            {
                if (Accounts.TryGetValue(byIdFilter.Id, out var value))
                {
                    res = [value];
                }
            
                return Task.FromResult(res);
            }
            case IAccountRepository.FindAccountsFilter.EmptyFilter:
            {
                res  = Accounts.Values.ToArray();

                return Task.FromResult(res);
            }

            case IAccountRepository.FindAccountsFilter.ByOwnerIdFilter byOwnerIdFilter:
                res  = Accounts.Values
                    .ToArray()
                    .Where(x => x.OwnerId == byOwnerIdFilter.OwnerId)
                    .ToArray();

                return Task.FromResult(res);
            default:
                throw new NotImplementedException($"Unknown filter {nameof(filter)}.");
        }
    }

    public async Task<Account> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        
        var idFilter = new IAccountRepository.FindAccountsFilter.ByIdFilter(id);

        var accounts = await FindAsync(idFilter, ct);
        
        if (id == Guid.Empty || accounts.Count == 0)
        {
            throw DomainException.CreateExistenceException("Account is not found.");
        }

        return accounts[0];
    }
}