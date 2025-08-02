using AccountService.Domain;
using JetBrains.Annotations;
using MediatR;

namespace AccountService.Features.Accounts.RemoveAccountFeature;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
public class RemoveAccountCommandHandler(IAccountRepository accountRepository) : IRequestHandler<RemoveAccountCommand>
{
    private IAccountRepository AccountRepository => accountRepository;
    
    public async Task Handle(RemoveAccountCommand request, CancellationToken ct)
    {
        var account = await AccountRepository.GetByIdAsync(request.AccountId, ct);
        
        account.Close();
        await AccountRepository.RemoveAsync(account, ct);
    }
}