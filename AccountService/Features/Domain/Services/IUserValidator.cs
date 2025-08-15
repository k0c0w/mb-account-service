namespace AccountService.Features.Domain.Services;

public interface IUserValidator
{
    Task<bool> UserWithIdExistsAsync(Guid userId, CancellationToken ct = default);
}