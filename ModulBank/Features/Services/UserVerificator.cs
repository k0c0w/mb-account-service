using ModulBank.Features.Domain;

namespace ModulBank.Features.Services;

public class UserVerificator : IUserVerificator
{
    private static readonly List<Guid> Users =
    [
        Guid.Parse("88bc045c-ecc8-4cb7-8cd9-b28938abef55"),
        Guid.Parse("6d8600da-cc20-42b9-a410-37c0b9e5b6c9"),
        Guid.Parse("6e3f15c5-7f3f-418f-8f99-77c0b10d91ca"),
        Guid.Parse("b5743f83-86be-432c-ae06-1bafc73c9f21"),
        Guid.Parse("fe88a76d-c896-4f78-91bd-1a6f2b7b8dab"),
    ];
    
    public Task<bool> UserWithIdExsitsAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult(Users.Contains(userId));
    }
}