namespace ModulBank.Features.Domain;

public interface IAccountRepository
{
    Task AddAsync(Account account, CancellationToken ct = default);
    
    Task UpdateAsync(Account account, CancellationToken ct = default);

    Task RemoveAsync(Account account, CancellationToken ct = default);
    
    Task<IReadOnlyList<Account>> FindAsync(FindAccountsFilter filter, CancellationToken ct = default);
    
    Task<Account> GetByIdAsync(Guid id, CancellationToken ct = default);

    public abstract record FindAccountsFilter
    {
        public sealed record ByIdFilter(Guid Id) : FindAccountsFilter;
        
        public sealed record ByOwnerIdFilter(Guid OwnerId) : FindAccountsFilter;

        public sealed record EmptyFilter : FindAccountsFilter;
    }
}