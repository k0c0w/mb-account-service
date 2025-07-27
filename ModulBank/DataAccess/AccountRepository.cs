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
}