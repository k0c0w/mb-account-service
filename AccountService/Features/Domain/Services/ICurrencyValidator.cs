namespace AccountService.Features.Domain.Services;

public interface ICurrencyValidator
{
    public ValueTask<bool> IsSupportedAsync(CurrencyCode currencyCode, CancellationToken ct = default);
}