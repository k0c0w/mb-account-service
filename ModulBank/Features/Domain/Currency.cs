namespace ModulBank.Features.Domain;

public readonly struct Currency
{
    public CurrencyCode Code { get; }
    
    public decimal Amount { get; }

    public Currency(CurrencyCode code, decimal currencyAmount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(currencyAmount, nameof(currencyAmount));

        Code = code;
        Amount = currencyAmount;
    }

    public override string ToString() => $"{Code} {Amount}";
}