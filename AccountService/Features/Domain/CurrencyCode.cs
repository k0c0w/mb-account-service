namespace AccountService.Features.Domain;

public record CurrencyCode
{
    /// <summary>
    /// ISO 4217 currency code
    /// </summary>
    public string Value { get; }

    public CurrencyCode(string iso4217CurrencyCode)
    {
        ArgumentException.ThrowIfNullOrEmpty(iso4217CurrencyCode, nameof(iso4217CurrencyCode));
        if (iso4217CurrencyCode.Length != 3)
        {
            throw new ArgumentException("ISO 4217 codes are 3 length strings.");
        }
        Value = iso4217CurrencyCode;
    }

    public override string ToString() => Value;
}