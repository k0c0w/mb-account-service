using AccountService.Domain;
using AccountService.Persistence.DataAccess;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Features.Users.GetAccountsByOwnerIdFeature;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
public sealed class GetAccountsByOwnerIdQueryHandler(
    IUserVerificator userVerificator,
    AccountServiceDbContext dbContext
    )
    : IRequestHandler<GetAccountsByOwnerIdQuery, IEnumerable<AccountByOwnerIdDto>>
{
    private IUserVerificator UserVerificator => userVerificator;
    
    private DbSet<Account> AccountRepository => dbContext.Accounts;

    public async Task<IEnumerable<AccountByOwnerIdDto>> Handle(GetAccountsByOwnerIdQuery request, CancellationToken ct)
    {
        if (!await UserVerificator.UserWithIdExsitsAsync(request.OwnerId, ct))
        {
            throw DomainException.CreateValidationException("User does not exist.", 
                new InvalidOperationException($"User with id {request.OwnerId} does not exist in system."));
        }
        
        return await AccountRepository
            .WithOwnerId(request.OwnerId)
            .Select(a => new AccountByOwnerIdDto
            {
                Id = a.Id,
                CurrencyCode = a.Balance.Code.ToString()
            })
            .ToArrayAsync(ct);
    }
}