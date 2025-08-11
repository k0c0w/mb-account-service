using AccountService.Features.Domain;

namespace AccountService.Persistence.Services;

public class CurrencyValidator : ICurrencyValidator
{
    public ValueTask<bool> IsSupportedAsync(CurrencyCode currencyCode, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        
        var value = currencyCode.Value is "RUB" or "USD" or "EUR";
        
        return ValueTask.FromResult(value);
    }
}