using MediatR;
using ModulBank.Features.Domain;

namespace ModulBank.Features;

public class RemoveAccountCommandHandler(IAccountRepository accountRepository) : IRequestHandler<RemoveAccountCommand>
{
    private IAccountRepository AccountRepository => accountRepository;
    
    public async Task Handle(RemoveAccountCommand request, CancellationToken ct)
    {
        var idFilter = new IAccountRepository.FindAccountsFilter.ByIdFilter(request.AccountId);

        var accounts = await AccountRepository.FindAsync(idFilter, ct);
        
        if (request.AccountId == Guid.Empty || accounts.Count == 0)
        {
            throw DomainException.CreateExistenceException("Account is not found.");
        }

        var account = accounts[0];
        
        account.Close();
        await AccountRepository.RemoveAsync(account, ct);
    }
}