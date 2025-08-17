using AccountService.Features.Domain.Services;

namespace AccountService.Persistence.Services.Domain;

public class UserValidator : IUserValidator
{
    private static readonly List<Guid> Users =
    [
        Guid.Parse("88bc045c-ecc8-4cb7-8cd9-b28938abef55"),
        Guid.Parse("6d8600da-cc20-42b9-a410-37c0b9e5b6c9")
    ];
    
    public Task<bool> UserWithIdExistsAsync(Guid userId, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return Task.FromResult(Users.Contains(userId));
    }
}