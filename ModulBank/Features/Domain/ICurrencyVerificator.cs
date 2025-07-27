using System.Diagnostics.CodeAnalysis;

namespace ModulBank.Features.Domain;

public interface ICurrencyVerificator
{
    public ValueTask<bool> IsSupportedAsync(CurrencyCode currencyCode, CancellationToken ct = default);
}