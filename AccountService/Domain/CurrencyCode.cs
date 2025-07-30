using System.Text.RegularExpressions;

namespace AccountService.Domain;

public record CurrencyCode
{
    /// <summary>
    /// ISO 4217 currency code
    /// </summary>
    public string Value { get; }

    public CurrencyCode(string currencyCode)
    {
        ArgumentException.ThrowIfNullOrEmpty(currencyCode, nameof(currencyCode));
        var value = currencyCode.ToUpper();
        if (currencyCode.Length != 3 || !Regex.IsMatch(value, @"[a-z][a-z][a-z]", RegexOptions.Compiled))
        {
            throw new ArgumentException("CurrencyCode must be in ISO 4217 format.");
        }
        Value = value;
    }

    public override string ToString() => Value;
}