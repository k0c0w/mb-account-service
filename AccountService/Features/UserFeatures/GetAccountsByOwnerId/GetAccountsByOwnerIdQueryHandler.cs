using AccountService.Features.DataAccess;
using AccountService.Features.Domain;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Features.UserFeatures.GetAccountsByOwnerId;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
public sealed class GetAccountsByOwnerIdQueryHandler(
    IUserValidator userValidator,
    AccountServiceDbContext dbContext
    )
    : IRequestHandler<GetAccountsByOwnerIdQuery, IEnumerable<AccountByOwnerIdDto>>
{
    private IUserValidator UserValidator => userValidator;
    
    private DbSet<Account> AccountRepository => dbContext.Accounts;

    public async Task<IEnumerable<AccountByOwnerIdDto>> Handle(GetAccountsByOwnerIdQuery request, CancellationToken ct)
    {
        if (!await UserValidator.UserWithIdExistsAsync(request.OwnerId, ct))
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
            .AsNoTracking()
            .ToArrayAsync(ct);
    }
}