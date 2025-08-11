using AccountService.Features.DataAccess;
using AccountService.Features.Domain;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Features.AccountFeatures.RemoveAccount;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
public class RemoveAccountCommandHandler(AccountServiceDbContext dbContext) : IRequestHandler<RemoveAccountCommand>
{
    private AccountServiceDbContext DbContext => dbContext;
    private DbSet<Account> Accounts => DbContext.Accounts;
    
    public async Task Handle(RemoveAccountCommand request, CancellationToken ct)
    {
        var account = await Accounts.FindByIdAsync(request.AccountId, ct);
        if (account is null)
        {
            throw DomainException.CreateExistenceException("Account does not exist.");
        }
        
        account.Close();
        Accounts.Update(account);

        Accounts.Remove(account);

        await dbContext.SaveChangesAsync(ct);
    }
}