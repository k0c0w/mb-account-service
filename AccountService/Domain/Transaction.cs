namespace AccountService.Domain;

public class Transaction
{
    public Guid Id { get; }

    public Guid AccountId { get; }

    public Guid? CounterpartyAccountId { get; }

    public TransactionType Type { get; }

    public Currency Amount { get; }

    public string Description { get; }

    public DateTimeOffset TimeUtc { get; }
    
    public Transaction(
        Guid accountId, 
        TransactionType type, 
        Currency amount, 
        string description,
        Guid? counterpartyAccountId = default)
    {
        Id = Guid.CreateVersion7();
        TimeUtc = DateTimeOffset.UtcNow;

        if (!Enum.IsDefined(type))
        {
            throw DomainException.CreateValidationException("Unknown transaction type.",
                new ArgumentOutOfRangeException(nameof(type), type, $"Unknown type value: {type}. Forgot to add new?"));
        }

        Type = type;

        if (accountId == Guid.Empty)
        {
            throw DomainException.CreateValidationException($"{nameof(Transaction)} {nameof(AccountId)} is invalid.",
                new ArgumentException("Guid is empty.", nameof(accountId)));
        }

        AccountId = accountId;

        if (counterpartyAccountId is not null && counterpartyAccountId == Guid.Empty)
        {
            throw DomainException.CreateValidationException($"{nameof(Transaction)} {nameof(CounterpartyAccountId)} is invalid.",
                new ArgumentException("Guid is empty.", nameof(counterpartyAccountId)));
        }

        CounterpartyAccountId = counterpartyAccountId;
        Description = description.Trim();
        Amount = amount;
    }
}