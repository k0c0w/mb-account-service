using AccountService.Features.Domain;
using MediatR;

namespace AccountService.Features;

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