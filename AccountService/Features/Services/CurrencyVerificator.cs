using AccountService.Features.Domain;

namespace AccountService.Features.Services;

public class CurrencyVerificator : ICurrencyVerificator
{
    public ValueTask<bool> IsSupportedAsync(CurrencyCode currencyCode, CancellationToken ct = default)
    {
        var value = currencyCode.Value is "RUB" or "USD" or "EUR";
        
        return ValueTask.FromResult(value);
    }
}