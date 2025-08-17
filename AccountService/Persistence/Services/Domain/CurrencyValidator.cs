using AccountService.Features.Domain;
using AccountService.Features.Domain.Services;

namespace AccountService.Persistence.Services.Domain;

public class CurrencyValidator : ICurrencyValidator
{
    public ValueTask<bool> IsSupportedAsync(CurrencyCode currencyCode, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        
        var value = currencyCode.Value is "RUB" or "USD" or "EUR";
        
        return ValueTask.FromResult(value);
    }
}