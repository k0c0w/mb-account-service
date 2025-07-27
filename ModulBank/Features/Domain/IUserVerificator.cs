namespace ModulBank.Features.Domain;

public interface IUserVerificator
{
    Task<bool> UserWithIdExsitsAsync(Guid userId, CancellationToken ct = default);
}