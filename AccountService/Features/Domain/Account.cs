using AccountService.Features.Domain.Events;
using AccountService.Features.Domain.Services;
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

    public AccountStatus Status { get; protected set; }

    public IReadOnlyList<Transaction> TransactionHistory => _transactionHistory;

    private readonly List<Transaction> _transactionHistory;

    private Account(
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
        Status = AccountStatus.Active;

        _transactionHistory = [];
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [UsedImplicitly]
    // Fabric method, used with reflection
    protected Account()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public void Close()
    {
        if (Status == AccountStatus.Closed)
        {
            throw DomainException.CreateValidationException(
                "Account is already closed.",
                new InvalidOperationException(
                    $"An attempt to close account {Id} which is already closed at {ClosingTimeUtc}."));
        }

        if (Balance.Amount != decimal.Zero)
        {
            throw DomainException.CreateValidationException(
                "All money must be withdrawn before the account is closed.",
                new InvalidOperationException($"An attempt to close non empty account {Id}."));
        }

        ClosingTimeUtc = DateTimeOffset.UtcNow;
        Status = AccountStatus.Closed;
        ModifiedAt = ClosingTimeUtc.Value;
    }

    public void ChangeInterestRate(AccountInterestRate interestRate)
    {
        if (Type == AccountType.Checking)
        {
            throw DomainException.CreateValidationException("Account does not support interest rate.",
                new InvalidOperationException(
                    $"An attempt to set interest rate to account {Id} which has type {Type}."));
        }

        InterestRate = interestRate;
        ModifiedAt = DateTimeOffset.UtcNow;
    }

    public async Task SendMoneyAsync(Account recipient, Currency money, IDomainEventNotifier eventNotifier)
    {
        if (this == recipient || Id == recipient.Id)
        {
            throw DomainException.CreateValidationException(
                "Can not funds the same account.",
                new ArgumentException($"An attempt to send money to same account with Id {Id}.")
            );
        }

        if (Status == AccountStatus.Closed)
        {
            throw DomainException.CreateValidationException(
                "Sender account is already closed.",
                new InvalidOperationException(
                    $"An attempt to withdraw money from account {Id}, but {Id} is already closed at {ClosingTimeUtc}."));
        }

        if (recipient.Status == AccountStatus.Closed)
        {
            throw DomainException.CreateValidationException(
                "Recipient is already closed.",
                new InvalidOperationException(
                    $"An attempt to debit account {recipient.Id} which is already closed at {ClosingTimeUtc}."));
        }

        if (recipient.Balance.Code != Balance.Code)
        {
            var invOpMessage =
                $"An attempt to send money from account {Id} to {recipient.Id}, but currencies did not match: {Balance.Code} != {recipient.Balance.Code}.";
            throw DomainException.CreateValidationException("Recipient account has another currency type.",
                new InvalidOperationException(invOpMessage));
        }

        ThrowIfInsufficientBalance(money);

        await CreditMoneyAsync(money, "Withdraw money operation", eventNotifier, counterpartyAccountId: recipient.Id);
        await recipient.DebitMoneyAsync(money, "Deposit money operation", eventNotifier, counterpartyAccountId: Id);

        var senderTransaction = _transactionHistory[^1];
        var recipientTransaction = recipient._transactionHistory[^1];

        await eventNotifier.NotifyAsync(new TransferCompletedEvent(senderTransaction, recipientTransaction));
    }

    public Task ApplyIncomingTransactionAsync(TransactionType transactionType, Currency money,
        IDomainEventNotifier eventNotifier)
    {
        if (money.Code != Balance.Code)
        {
            var invOpMessage =
                $"An attempt to apply incoming transaction, but currencies did not match: {money.Code} != {Balance.Code}.";
            throw DomainException.CreateValidationException("Account has another currency type.",
                new InvalidOperationException(invOpMessage));
        }

        if (Status == AccountStatus.Closed)
        {
            throw DomainException.CreateValidationException(
                "Account is already closed.",
                new InvalidOperationException(
                    $"An attempt to perform operation with account {Id}, but {Id} is already closed at {ClosingTimeUtc}."));
        }

        if (transactionType == TransactionType.Credit)
        {
            ThrowIfInsufficientBalance(money);
            return CreditMoneyAsync(money, "External payment operation.", eventNotifier);
        }
        
        return DebitMoneyAsync(money, "External debit operation.", eventNotifier);
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

    public void Freeze()
    {
        switch (Status)
        {
            case AccountStatus.Closed:
                throw DomainException.CreateValidationException("Account is closed.",
                    new InvalidOperationException($"Attempt to freeze closed account {Id}."));
            case AccountStatus.Frozen:
                throw DomainException.CreateValidationException("Account is already frozen.",
                    new InvalidOperationException($"Attempt to freeze frozen account {Id}."));
            case AccountStatus.Active:
            default:
                Status = AccountStatus.Frozen;
                ModifiedAt = DateTimeOffset.UtcNow;
                break;
        }
    }

    public void Unfreeze()
    {
        if (Status != AccountStatus.Frozen)
        {
            throw DomainException.CreateValidationException("Account is not frozen.",
                new InvalidOperationException($"Attempt to unfreeze account {Id}, which is not frozen."));
        }

        Status = AccountStatus.Active;
        ModifiedAt = DateTimeOffset.UtcNow;
    }

    private void ThrowIfInsufficientBalance(Currency money)
    {
        if (Balance.Amount - money.Amount >= decimal.Zero)
        {
            return;
        }

        var invOpMessage =
            $"An attempt to credit money from account {Id} by {money.Amount}, but account has only {Balance.Amount}.";
        throw DomainException.CreateValidationException("Insufficient account balance.",
            new InvalidOperationException(invOpMessage));
    }

    private Task CreditMoneyAsync(Currency money, string description, IDomainEventNotifier eventNotifier,
        Guid? counterpartyAccountId = default)
    {
        var transaction = new Transaction(Id,
            TransactionType.Credit,
            money,
            description,
            counterpartyAccountId);
        _transactionHistory.Add(transaction);

        Balance = new Currency(Balance.Code, Balance.Amount - money.Amount);
        ModifiedAt = DateTimeOffset.UtcNow;

        return eventNotifier.NotifyAsync(new MoneyCreditedEvent(transaction));
    }

    private Task DebitMoneyAsync(Currency money, string description, IDomainEventNotifier eventNotifier,
        Guid? counterpartyAccountId = default)
    {
        if (Status == AccountStatus.Frozen)
        {
            throw DomainException.CreateValidationException("Account is frozen.",
                new InvalidOperationException($"Attempt to debit frozen account {Id}."));
        }
        
        var transaction = new Transaction(Id,
            TransactionType.Debit,
            money,
            description,
            counterpartyAccountId);
        _transactionHistory.Add(transaction);

        Balance = new Currency(Balance.Code, Balance.Amount + money.Amount);
        ModifiedAt = DateTimeOffset.UtcNow;

        return eventNotifier.NotifyAsync(new MoneyDebitedEvent(transaction));
    }

    public static async Task<Account> CreateNewAsync(Guid ownerId,
        CurrencyCode currencyCode,
        AccountType type,
        IDomainEventNotifier eventNotifier,
        AccountInterestRate? interestRate = default)
    {
        var account = new Account(ownerId, currencyCode, type, interestRate);
        await eventNotifier.NotifyAsync(new AccountOpenedEvent(account));

        return account;
    }
}