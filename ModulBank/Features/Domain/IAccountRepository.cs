namespace ModulBank.Features.Domain;

public interface IAccountRepository
{
    Task AddAsync(Account account, CancellationToken ct = default);

    Task RemoveAsync(Account account, CancellationToken ct = default);
    
    Task<IReadOnlyList<Account>> FindAsync(FindAccountsFilter filter, CancellationToken ct = default);

    public abstract record FindAccountsFilter
    {
        public sealed record ByIdFilter(Guid Id) : FindAccountsFilter;

        public sealed record EmptyFilter : FindAccountsFilter;
    }
}