using System.Collections.Concurrent;
using ModulBank.Features.Domain;

namespace ModulBank.DataAccess;

public class AccountRepository : IAccountRepository
{
    private static readonly ConcurrentDictionary<Guid, Account> Accounts = [];
    
    public Task AddAsync(Account account, CancellationToken ct = default)
    {
        Accounts.TryAdd(account.Id, account);
        
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Account account, CancellationToken ct = default)
    {
        Accounts.AddOrUpdate(account.Id, (_) => account, (_, _) => account);
        
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Account account, CancellationToken ct = default)
    {
        Accounts.Remove(account.Id, out var _);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Account>> FindAsync(IAccountRepository.FindAccountsFilter filter, CancellationToken ct = default)
    {
        IReadOnlyList<Account> res = [];
        if (filter is IAccountRepository.FindAccountsFilter.ByIdFilter byIdFilter)
        {
            if (Accounts.TryGetValue(byIdFilter.Id, out var value))
            {
                res = [value];
            }
            
            return Task.FromResult(res);
        }

        if (filter is IAccountRepository.FindAccountsFilter.EmptyFilter)
        {
            res  = Accounts.Values.ToArray();

            return Task.FromResult(res);
        }

        throw new NotImplementedException($"Unknown filter {nameof(filter)}.");
    }

    public async Task<Account> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var idFilter = new IAccountRepository.FindAccountsFilter.ByIdFilter(id);

        var accounts = await FindAsync(idFilter, ct);
        
        if (id == Guid.Empty || accounts.Count == 0)
        {
            throw DomainException.CreateExistenceException("Account is not found.");
        }

        return accounts[0];
    }
}