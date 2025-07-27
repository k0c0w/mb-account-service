namespace ModulBank.Features.Domain;

public record CurrencyCode
{
    /// <summary>
    /// ISO 4217 currency code
    /// </summary>
    public string Value { get; }

    public CurrencyCode(string iso4217CurrencyCode)
    {
        ArgumentException.ThrowIfNullOrEmpty(iso4217CurrencyCode, nameof(iso4217CurrencyCode));
        Value = iso4217CurrencyCode;
    }

    public override string ToString() => Value;
}