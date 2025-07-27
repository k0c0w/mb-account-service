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

    public Task RemoveAsync(Account account, CancellationToken ct = default)
    {
        Accounts.Remove(account.Id, out var _);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Account>> FindAsync(IAccountRepository.FindAccountsFilter filter, CancellationToken ct = default)
    {
        if (filter is not IAccountRepository.FindAccountsFilter.ByIdFilter byIdFilter)
            throw new NotImplementedException();
        IReadOnlyList<Account> res = [];
        if (Accounts.TryGetValue(byIdFilter.Id, out var value))
        {
            res = [value];
        }
            
        return Task.FromResult(res);
    }
}