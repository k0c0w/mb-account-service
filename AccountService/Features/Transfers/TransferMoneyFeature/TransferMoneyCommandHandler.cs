using AccountService.Domain;
using AccountService.Persistence.DataAccess;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Features.Transfers.TransferMoneyFeature;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
public class TransferMoneyCommandHandler(AccountServiceDbContext dbContext) : IRequestHandler<TransferMoneyCommand>
{
    private AccountServiceDbContext DbContext => dbContext;
    
    private DbSet<Account> AccountRepository => DbContext.Accounts;
    
    public async Task Handle(TransferMoneyCommand request, CancellationToken ct)
    {
        var sender = await AccountRepository.FindByIdAsync(request.SenderAccountId, ct);
        if (sender is null)
        {
            throw DomainException.CreateExistenceException("Sender account is not found.");
        }
        
        var recipient = await AccountRepository.FindByIdAsync(request.RecipientAccountId, ct);
        if (recipient is null)
        {
            throw DomainException.CreateExistenceException("Recipient account is not found.");
        }

        var currency = new Currency(new CurrencyCode(sender.Balance.Code.Value), request.Amount);
        sender.SendMoney(recipient, currency);
        AccountRepository.UpdateRange(sender, recipient);
        
        await DbContext.SaveChangesAsync(ct);
    }
}