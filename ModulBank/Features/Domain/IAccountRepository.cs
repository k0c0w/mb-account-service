namespace ModulBank.Features.Domain;

public interface IAccountRepository
{
    Task AddAsync(Account account, CancellationToken ct = default);
}