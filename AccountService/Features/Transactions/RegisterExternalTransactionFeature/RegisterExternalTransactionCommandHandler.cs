using AccountService.Domain;
using JetBrains.Annotations;
using MediatR;

namespace AccountService.Features.Transactions.RegisterExternalTransactionFeature;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
public class RegisterExternalTransactionCommandHandler(
    IAccountRepository accountRepository,
    ICurrencyVerificator currencyVerificator
    )
    : IRequestHandler<RegisterExternalTransactionCommand>
{
    private IAccountRepository AccountRepository => accountRepository;
    
    private ICurrencyVerificator CurrencyVerificator => currencyVerificator;
    
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
        if (!await CurrencyVerificator.IsSupportedAsync(currencyCode, ct))
        {
            throw DomainException.CreateValidationException("Unsupported currency.", 
                new ArgumentException($"Unsupported currency met: {request.CurrencyCode}."));
        }

        var currency = new Currency(currencyCode, request.Amount);

        var account = await FindAccountOrThrowExceptionAsync(request.AccountId, ct);
        account.ApplyIncomingTransaction(request.TransactionType, currency);

        await AccountRepository.UpdateAsync(account, ct);
    }

    private async Task<Account> FindAccountOrThrowExceptionAsync(Guid accountId, CancellationToken ct)
    {
        var byIdAccountFilter = new IAccountRepository.FindAccountsFilter.ByIdFilter(accountId);
        var accounts = await AccountRepository.FindAsync(byIdAccountFilter, ct);

        if (accounts.Count == 0)
        {
            throw DomainException.CreateValidationException("An account is not found.",
                new InvalidOperationException("An attempt to register transaction for non exiting account.", 
                    new ArgumentException($"Invalid argument value: {accountId}.")));
        }

        return accounts.First(x => x.Id == accountId);
    }
}