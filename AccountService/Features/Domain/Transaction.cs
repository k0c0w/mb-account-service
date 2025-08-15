using JetBrains.Annotations;

namespace AccountService.Features.Domain;

public class Transaction
{
    // ReSharper disable once. Value is used by serialization and domain.
    public Guid Id { get; protected init; }
    
    public Guid AccountId { get;  protected init; }

    public Guid? CounterpartyAccountId { get;  protected init; }

    public TransactionType Type { get;  protected init; }

    public Currency Amount { get; protected init; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    // length is handled by domain
    public string Description { get; protected init; }

    public DateTimeOffset TimeUtc { get; protected init; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [UsedImplicitly]
    // fabric method, used with reflection
    protected Transaction()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
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