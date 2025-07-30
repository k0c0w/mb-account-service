namespace AccountService.Domain;

public interface ICurrencyVerificator
{
    public ValueTask<bool> IsSupportedAsync(CurrencyCode currencyCode, CancellationToken ct = default);
}