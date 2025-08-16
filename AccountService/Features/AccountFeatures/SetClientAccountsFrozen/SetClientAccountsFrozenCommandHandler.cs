using AccountService.Features.DataAccess;
using AccountService.Features.Domain;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Features.AccountFeatures.SetClientAccountsFrozen;

[UsedImplicitly]
public sealed class SetClientAccountsFrozenCommandHandler(AccountServiceDbContext db) 
    : IRequestHandler<SetClientAccountsFrozenCommand>
{
    private AccountServiceDbContext Db => db;
    
    public async Task Handle(SetClientAccountsFrozenCommand request, CancellationToken ct)
    {
        var accounts = await Db.Accounts
            .WithOwnerId(request.OwnerId)
            .ToArrayAsync(ct);

        if (accounts.Length == 0)
        {
            throw DomainException.CreateValidationException("Accounts were not found.", 
                new ArgumentException(nameof(request.OwnerId), $"User {request.OwnerId} does not have any account."));
        }

        foreach (var account in accounts)
        {
            if (request.Freeze)
            {
                account.Freeze();
            }
            else
            {
                account.Unfreeze();
            }
        }
        
        Db.Accounts.UpdateRange(accounts);
        await Db.SaveChangesAsync(ct);
    }
}