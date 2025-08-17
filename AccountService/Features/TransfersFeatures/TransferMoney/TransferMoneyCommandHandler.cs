using AccountService.Features.Domain;
using AccountService.Features.Domain.Services;
using AccountService.Persistence.Infrastructure.DataAccess;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Features.TransfersFeatures.TransferMoney;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
public class TransferMoneyCommandHandler(AccountServiceDbContext dbContext, IDomainEventNotifier eventNotifier) 
    : IRequestHandler<TransferMoneyCommand>
{
    private AccountServiceDbContext DbContext => dbContext;
    
    private DbSet<Account> AccountRepository => DbContext.Accounts;
    
    private IDomainEventNotifier EventNotifier => eventNotifier;
    
    public async Task Handle(TransferMoneyCommand request, CancellationToken ct)
    {
        var sender = await GetAccountIdOrThrowNotFoundAsync(request.SenderAccountId, 
            "Sender account is not found.", ct);
        var recipient = await GetAccountIdOrThrowNotFoundAsync(request.RecipientAccountId, 
            "Recipient account is not found.", ct);
        
        var currency = new Currency(new CurrencyCode(sender.Balance.Code.Value), request.Amount);
        await sender.SendMoneyAsync(recipient, currency, eventNotifier: eventNotifier);
        AccountRepository.UpdateRange(sender, recipient);
        
        await DbContext.SaveChangesAsync(ct);
    }

    private async Task<Account> GetAccountIdOrThrowNotFoundAsync(Guid id, string notFoundMessage, CancellationToken ct)
    {
        var account = await AccountRepository
            .Include(a => a.TransactionHistory)
            .FindByIdAsync(id, ct);
        
        if (account is null)
        {
            throw DomainException.CreateExistenceException(notFoundMessage);
        }

        return account;
    }
}