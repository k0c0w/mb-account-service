using AccountService.Features.Domain;
using AccountService.Features.Domain.Services;
using AccountService.Persistence.Infrastructure.DataAccess;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Features.TransfersFeatures.TransferMoney;

[UsedImplicitly]
public class TransferMoneyCommandHandler(AccountServiceDbContext dbContext, IDomainEventNotifier eventNotifier) 
    : IRequestHandler<TransferMoneyCommand>
{
    private AccountServiceDbContext DbContext => dbContext;
    
    private DbSet<Account> AccountRepository => DbContext.Accounts;
    
    // ReSharper disable once UnusedMember.Local
    private IDomainEventNotifier EventNotifier => eventNotifier;
    
    public async Task Handle(TransferMoneyCommand request, CancellationToken ct)
    {
        var sender = await GetAccountIdOrThrowNotFoundAsync(request.SenderAccountId, 
            "Sender account is not found.", ct);
        var recipient = await GetAccountIdOrThrowNotFoundAsync(request.RecipientAccountId, 
            "Recipient account is not found.", ct);
        
        var currency = new Currency(new CurrencyCode(sender.Balance.Code.Value), request.Amount);
        await sender.SendMoneyAsync(recipient, currency, eventNotifier: EventNotifier);
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