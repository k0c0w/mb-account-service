namespace AccountService.Features.Domain;

public interface ICurrencyValidator
{
    public ValueTask<bool> IsSupportedAsync(CurrencyCode currencyCode, CancellationToken ct = default);
}