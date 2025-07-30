using AccountService.Domain;
using MediatR;

namespace AccountService.Features;

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
        if (!Enum.TryParse<TransactionType>(request.TransactionType, out var type))
        {
            throw DomainException.CreateValidationException("Unsupported transaction type.", 
                new ArgumentOutOfRangeException(nameof(request.TransactionType), 
                    $"Unsupported transaction type met. Forgot to add one?", request.TransactionType));
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
        
        var account = await AccountRepository.GetByIdAsync(request.AccountId, ct);
        account.ApplyIncomingTransaction(type, currency);

        await AccountRepository.UpdateAsync(account, ct);
    }
}