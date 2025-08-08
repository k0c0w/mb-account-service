using AccountService.Domain;
using AccountService.Persistence.DataAccess;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Features.Accounts.ChangeAccountInterestRateFeature;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
public class ChangeAccountInterestRateCommandHandler(AccountServiceDbContext dbContext)
    : IRequestHandler<ChangeAccountInterestRateCommand>
{
    private DbSet<Account> Accounts => DbContext.Accounts;
    
    private AccountServiceDbContext DbContext => dbContext;
    
    public async Task Handle(ChangeAccountInterestRateCommand request, CancellationToken ct)
    {
        var interestRate = new AccountInterestRate(request.Value);
            
        var account = await Accounts.FindByIdAsync(request.AccountId, ct);
        if (account is null)
        {
            throw DomainException.CreateExistenceException("Account does not exists.");
        }
        
        account.ChangeInterestRate(interestRate);

        Accounts.Update(account);

        await DbContext.SaveChangesAsync(ct);
    }
}