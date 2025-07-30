using AccountService.Features.Domain;
using MediatR;

namespace AccountService.Features;

public class TransferMoneyCommandHandler(IAccountRepository accountRepository) : IRequestHandler<TransferMoneyCommand>
{
    private IAccountRepository AccountRepository => accountRepository;
    
    public async Task Handle(TransferMoneyCommand request, CancellationToken ct)
    {
        var sender = await FindAccountByIdAsync(request.SenderAccountId, ct);
        if (sender is null)
        {
            throw DomainException.CreateExistenceException("Sender account is not found.");
        }
        
        var recipient = await FindAccountByIdAsync(request.RecipientAccountId, ct);
        if (recipient is null)
        {
            throw DomainException.CreateExistenceException("Recipient account is not found.");
        }

        var currency = new Currency(new CurrencyCode(sender.Balance.Code.Value), request.Amount);
        sender.SendMoney(recipient, currency);

        await AccountRepository.UpdateAsync(sender, ct);
        await AccountRepository.UpdateAsync(recipient, ct);
    }

    private async Task<Account?> FindAccountByIdAsync(Guid id, CancellationToken ct)
    {
        var byIdFilter = new IAccountRepository.FindAccountsFilter.ByIdFilter(id);
        
        var accounts = await AccountRepository.FindAsync(byIdFilter, ct);

        return accounts.Count == 0 ? default : accounts[0];
    }
}