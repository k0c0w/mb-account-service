namespace ModulBank.Features.Domain;

public sealed record AccountInterestRate
{
    public decimal Value { get; private init; }

    public AccountInterestRate(decimal interestRate)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(interestRate, decimal.Zero);
        Value = interestRate;
    }
}