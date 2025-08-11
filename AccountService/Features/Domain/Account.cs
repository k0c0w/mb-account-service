using JetBrains.Annotations;

namespace AccountService.Features.Domain;

public class Account
{
    private const decimal InitialBalance = decimal.Zero;
    
    public Guid Id { get; protected init; }
    
    public Guid OwnerId { get; protected init; } 
    
    public AccountType Type { get; protected init; }
    
    public Currency Balance { get; protected set; }

    public AccountInterestRate? InterestRate { get; protected set; }
    
    public DateTimeOffset CreationTimeUtc { get; protected init; }
    
    public DateTimeOffset? ClosingTimeUtc { get; protected set; }
    
    public DateTimeOffset ModifiedAt { get; protected set; }
    
    public IReadOnlyList<Transaction> TransactionHistory => _transactionHistory;

    private readonly List<Transaction> _transactionHistory;

    private bool IsClosed => ClosingTimeUtc is not null;
    
    public Account(
        Guid ownerId, 
        CurrencyCode currencyCode, 
        AccountType type,
        AccountInterestRate? interestRate = default
        )
    {
        Id = Guid.CreateVersion7();

        if (ownerId == Guid.Empty)
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
        ModifiedAt = CreationTimeUtc;
        Balance = new Currency(currencyCode, InitialBalance);

        _transactionHistory = [];
    }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [UsedImplicitly]
    // Fabric method, used with reflection
    protected Account() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public void Close()
    {
        if (IsClosed)
        {
            throw DomainException.CreateValidationException(
                "Account is already closed.", 
                new InvalidOperationException($"An attempt to close account {Id} which is already closed at {ClosingTimeUtc}."));
        }

        if (Balance.Amount != decimal.Zero)
        {
            throw DomainException.CreateValidationException(
                "All money must be withdrawn before the account is closed.",
                new InvalidOperationException($"An attempt to close non empty account {Id}."));
        }
        
        ClosingTimeUtc = DateTimeOffset.UtcNow;
        ModifiedAt = ClosingTimeUtc.Value;
    }

    public void ChangeInterestRate(AccountInterestRate interestRate)
    {
        if (Type == AccountType.Checking)
        {
            throw DomainException.CreateValidationException("Account does not support interest rate.",
                new InvalidOperationException($"An attempt to set interest rate to account {Id} which has type {Type}."));
        }

        InterestRate = interestRate;
        ModifiedAt = DateTimeOffset.UtcNow;
    }

    public void SendMoney(Account recipient, Currency money)
    {
        if (this == recipient || Id == recipient.Id)
        {
            throw DomainException.CreateValidationException(
                "Can not funds the same account.",
                new ArgumentException($"An attempt to send money to same account with Id {Id}.")
            );
        }
        
        if (IsClosed)
        {
            throw DomainException.CreateValidationException(
                "Sender account is already closed.", 
                new InvalidOperationException($"An attempt to withdraw money from account {Id}, but {Id} is already closed at {ClosingTimeUtc}."));
        }
        
        if (recipient.IsClosed)
        {
            throw DomainException.CreateValidationException(
                "Recipient is already closed.", 
                new InvalidOperationException($"An attempt to debit account {recipient.Id} which is already closed at {ClosingTimeUtc}."));
        }

        if (recipient.Balance.Code != Balance.Code)
        {
            var invOpMessage = $"An attempt to send money from account {Id} to {recipient.Id}, but currencies did not match: {Balance.Code} != {recipient.Balance.Code}.";
            throw DomainException.CreateValidationException("Recipient account has another currency type.", 
                new InvalidOperationException(invOpMessage));
        }

        ThrowIfInsufficientBalance(money);

        CreditMoney(money, "Withdraw money operation", recipient.Id);
        recipient.DebitMoney(money, "Deposit money operation", Id);
    }

    public void ApplyIncomingTransaction(TransactionType transactionType, Currency money)
    {
        if (money.Code != Balance.Code)
        {
            var invOpMessage = $"An attempt to apply incoming transaction, but currencies did not match: {money.Code} != {Balance.Code}.";
            throw DomainException.CreateValidationException("Account has another currency type.", 
                new InvalidOperationException(invOpMessage));
        }
        
        if (IsClosed)
        {
            throw DomainException.CreateValidationException(
                "Account is already closed.", 
                new InvalidOperationException($"An attempt to perform operation with account {Id}, but {Id} is already closed at {ClosingTimeUtc}."));
        }

        if (transactionType == TransactionType.Credit)
        {
            ThrowIfInsufficientBalance(money);
            CreditMoney(money, "External payment operation");
        }
        else
        {
            DebitMoney(money, "External payment operation");
        }
    }
    
    public decimal GetBalanceAt(DateTimeOffset time)
    {
        return _transactionHistory
            .TakeWhile(t => t.TimeUtc <= time)
            .Aggregate(InitialBalance, (sum, transaction) =>
        {
            var amount = transaction.Amount.Amount;
            var valueToAdd = transaction.Type == TransactionType.Debit ? amount : -amount;

            return sum + valueToAdd;
        });
    }
    
    private void ThrowIfInsufficientBalance(Currency money)
    {
        if (Balance.Amount - money.Amount >= decimal.Zero)
        {
            return;
        }
        
        var invOpMessage =  $"An attempt to credit money from account {Id} by {money.Amount}, but account has only {Balance.Amount}.";
        throw DomainException.CreateValidationException("Insufficient account balance.", 
            new InvalidOperationException(invOpMessage));
    }
    
    private void CreditMoney(Currency money, string description, Guid? counterpartyAccountId = default)
    {
        var transaction = new Transaction(Id, 
            TransactionType.Credit, 
            money, 
            description,
            counterpartyAccountId);
        _transactionHistory.Add(transaction);

        Balance = new Currency(Balance.Code, Balance.Amount - money.Amount);
        ModifiedAt = DateTimeOffset.UtcNow;
    }
    
    private void DebitMoney(Currency money, string description, Guid? counterpartyAccountId = default)
    {
        var transaction = new Transaction(Id, 
            TransactionType.Debit, 
            money, 
            description,
            counterpartyAccountId);
        _transactionHistory.Add(transaction);

        Balance = new Currency(Balance.Code, Balance.Amount + money.Amount);
        ModifiedAt = DateTimeOffset.UtcNow;
    }
}