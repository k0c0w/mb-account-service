namespace AccountService.Domain;

public sealed record AccountInterestRate
{
    private const decimal InterestRateBoundaryCoefficient = 3m;
    
    public decimal Value { get; private init; }

    public AccountInterestRate(decimal interestRate)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(interestRate, decimal.Zero);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(Math.Abs(interestRate), InterestRateBoundaryCoefficient);
        Value = interestRate;
    }
}