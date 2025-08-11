namespace AccountService.Features.Domain;

public interface IUserValidator
{
    Task<bool> UserWithIdExistsAsync(Guid userId, CancellationToken ct = default);
}