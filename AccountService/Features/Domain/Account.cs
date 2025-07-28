namespace AccountService.Features.Domain;

public class Account
{
    private const decimal InitialBalance = decimal.Zero;
    
    public Guid Id { get; }
    
    public Guid OwnerId { get; } 
    
    public AccountType Type { get; protected set; }
    
    public Currency Balance { get; private set; }
    
    public AccountInterestRate? InterestRate { get; private set; }
    
    public DateTimeOffset CreationTimeUtc { get; protected set; }
    
    public DateTimeOffset? ClosingTimeUtc { get; private set; }

    private bool IsClosed { get; set; }

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
        Balance = new Currency(currencyCode, InitialBalance);

        _transactions = [];
    }

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

        IsClosed = true;
    }

    public void ChangeInterestRate(AccountInterestRate interestRate)
    {
        if (Type == AccountType.Checking)
        {
            throw DomainException.CreateValidationException("Account does not support interest rate.",
                new InvalidOperationException($"An attempt to set interest rate to account {Id} which has type {Type}."));
        }

        InterestRate = interestRate;
    }

    public void SendMoney(Account recipient, Currency money)
    {
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
        return _transactions
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
        if (Balance.Amount - money.Amount < decimal.Zero)
        {
            var invOpMessage =  $"An attempt to credit money from account {Id} by {money.Amount}, but account has only {Balance.Amount}.";
            throw DomainException.CreateValidationException("Insufficient account balance.", 
                new InvalidOperationException(invOpMessage));
        }
    }
    
    private void CreditMoney(Currency money, string description, Guid? counterpartyAccountId = default)
    {
        var transaction = new Transaction(Id, 
            TransactionType.Credit, 
            money, 
            description,
            counterpartyAccountId);
        _transactions.Add(transaction);

        Balance = new Currency(Balance.Code, Balance.Amount - money.Amount);
    }
    
    private void DebitMoney(Currency money, string description, Guid? counterpartyAccountId = default)
    {
        var transaction = new Transaction(Id, 
            TransactionType.Debit, 
            money, 
            description,
            counterpartyAccountId);
        _transactions.Add(transaction);

        Balance = new Currency(Balance.Code, Balance.Amount + money.Amount);
    }
}