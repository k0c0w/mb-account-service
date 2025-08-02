using AccountService.Domain;
using JetBrains.Annotations;
using MediatR;

namespace AccountService.Features.Accounts.ChangeAccountInterestRateFeature;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
public class ChangeAccountInterestRateCommandHandler(IAccountRepository accountRepository)
    : IRequestHandler<ChangeAccountInterestRateCommand>
{
    private IAccountRepository AccountRepository => accountRepository;
    
    public async Task Handle(ChangeAccountInterestRateCommand request, CancellationToken ct)
    {
        var interestRate = new AccountInterestRate(request.Value);
            
        var account = await AccountRepository.GetByIdAsync(request.AccountId, ct);
        account.ChangeInterestRate(interestRate);

        await AccountRepository.UpdateAsync(account, ct);
    }
}