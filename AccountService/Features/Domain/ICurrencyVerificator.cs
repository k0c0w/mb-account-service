using System.Diagnostics.CodeAnalysis;

namespace AccountService.Features.Domain;

public interface ICurrencyVerificator
{
    public ValueTask<bool> IsSupportedAsync(CurrencyCode currencyCode, CancellationToken ct = default);
}