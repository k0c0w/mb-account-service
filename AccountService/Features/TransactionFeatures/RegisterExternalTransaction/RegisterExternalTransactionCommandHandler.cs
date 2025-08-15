using AccountService.Features.DataAccess;
using AccountService.Features.Domain;
using AccountService.Features.Domain.Services;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Features.TransactionFeatures.RegisterExternalTransaction;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
public class RegisterExternalTransactionCommandHandler(
    AccountServiceDbContext dbContext,
    ICurrencyValidator currencyValidator
    )
    : IRequestHandler<RegisterExternalTransactionCommand>
{
    private AccountServiceDbContext DbContext => dbContext;
    
    private DbSet<Account> Accounts => DbContext.Accounts;
    
    private ICurrencyValidator CurrencyValidator => currencyValidator;
    
    public async Task Handle(RegisterExternalTransactionCommand request, CancellationToken ct)
    {
        if (!Enum.IsDefined(request.TransactionType))
        {
            throw DomainException.CreateValidationException("Unsupported transaction type.", 
                new ArgumentOutOfRangeException(nameof(request.TransactionType), 
                    request.TransactionType,
                    "Unsupported transaction type met. Forgot to add one?"));
        }
        
        if (request.Amount <= decimal.Zero)
        {
            throw DomainException.CreateValidationException("Invalid transaction payload.", 
                new ArgumentOutOfRangeException(nameof(request.Amount), request.Amount,  "Value should be positive."));
        }

        var currencyCode = new CurrencyCode(request.CurrencyCode);
        if (!await CurrencyValidator.IsSupportedAsync(currencyCode, ct))
        {
            throw DomainException.CreateValidationException("Unsupported currency.", 
                new ArgumentException($"Unsupported currency met: {request.CurrencyCode}."));
        }

        var currency = new Currency(currencyCode, request.Amount);

        var account = await Accounts
            .Include(a => a.TransactionHistory)
            .FindByIdAsync(request.AccountId, ct);
        
        if (account is null)
        {
            throw DomainException.CreateValidationException("An account is not found.",
                new InvalidOperationException("An attempt to register transaction for non exiting account.", 
                    new ArgumentException($"Invalid argument value: {request.AccountId}.")));
        }
        
        account.ApplyIncomingTransaction(request.TransactionType, currency);
        Accounts.Update(account);
        
        await DbContext.SaveChangesAsync(ct);
    }
}