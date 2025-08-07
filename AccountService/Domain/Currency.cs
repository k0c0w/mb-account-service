namespace AccountService.Domain;

public sealed record Currency
{
    public CurrencyCode Code { get; }
    
    public decimal Amount { get; }

    public Currency(CurrencyCode code, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount, nameof(amount));

        Code = code;
        Amount = amount;
    }
    
    public Currency(string code, decimal amount) : this(new CurrencyCode(code), amount) { }

    public override string ToString() => $"{Code} {Amount}";
}