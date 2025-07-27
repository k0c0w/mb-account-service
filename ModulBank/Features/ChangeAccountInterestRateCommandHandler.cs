using MediatR;
using ModulBank.Features.Domain;

namespace ModulBank.Features;

public class ChangeAccountInterestRateCommandHandler(IAccountRepository accountRepository)
    : IRequestHandler<ChangeAccountInterestRateCommand>
{
    private IAccountRepository AccountRepository => accountRepository;
    
    public async Task Handle(ChangeAccountInterestRateCommand request, CancellationToken ct)
    {
        var interestRate = new AccountInterestRate(request.Value);
            
        var byIdFilter = new IAccountRepository.FindAccountsFilter.ByIdFilter(request.AccountId);
        var accounts = await AccountRepository.FindAsync(byIdFilter, ct);
        if (accounts.Count == 0 || request.AccountId == Guid.Empty)
        {
            throw DomainException.CreateExistenceException("Account does not exist.");
        }

        var account = accounts[0];
        account.ChangeInterestRate(interestRate);

        await AccountRepository.UpdateAsync(account, ct);
    }
}