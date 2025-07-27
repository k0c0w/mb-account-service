namespace ModulBank.Features.Domain;

public class Account
{
    public Guid Id { get; }
    
    public Guid OwnerId { get; } 
    
    public AccountType Type { get; protected set; }
    
    public Currency Balance { get; protected set; }
    
    public AccountInterestRate? InterestRate { get; protected set; }
    
    public DateTimeOffset CreationTimeUtc { get; protected set; }
    
    public DateTimeOffset? ClosingTimeUtc { get; protected set; }

    public IReadOnlyList<Transaction> TransactionHistory => _transactions;

    private readonly List<Transaction> _transactions;
    
    public Account(
        Guid ownerId, 
        CurrencyCode currencyCode, 
        AccountType type,
        AccountInterestRate? interestRate = default
        )
    {
        Id = Guid.CreateVersion7();

        if (OwnerId == Guid.Empty)
        {
            throw DomainException.CreateValidationException($"{nameof(Account)} {nameof(OwnerId)} is invalid.", 
                new ArgumentException("Guid is empty.", nameof(ownerId)));
        }
        OwnerId = ownerId;
        
        if (!Enum.IsDefined(type))
        {
            throw DomainException.CreateValidationException($"Unknown {nameof(AccountType)} value.",
                new ArgumentOutOfRangeException(nameof(type)));
        }
        Type = type;

        if (type == AccountType.Checking && interestRate is not null)
        {
            throw DomainException.CreateValidationException(
                $"{nameof(AccountInterestRate)} is not supported for {nameof(AccountType.Checking)} {nameof(Account)}.",
                new InvalidOperationException("Operation is not supported.")
            );
        }

        InterestRate = interestRate;
        CreationTimeUtc = DateTimeOffset.UtcNow;
        Balance = new Currency(currencyCode, decimal.Zero);

        _transactions = [];
    }
    
}