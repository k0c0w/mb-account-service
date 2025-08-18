using System.Reflection;
using AccountService.Features.Domain;
using Bogus;

namespace AccountService.Tests.Factories;

public static class AccountsFactory
{
    private static readonly Faker Faker = new();

    public static Account CreateAccount(
        AccountType accountType,
        decimal balance,
        Guid? ownerId = null,
        Guid? accountId = null,
        CurrencyCode? currencyCode = null,
        DateTimeOffset? creationTime = null,
        AccountInterestRate? interestRate = null,
        DateTimeOffset? closeTimeUtc = null,
        bool isFrozen = false)
    {
        ownerId ??= Guid.CreateVersion7();
        accountId ??= Guid.CreateVersion7();
        var code = currencyCode ?? new CurrencyCode(Faker.Finance.Currency().Code);
        var createdAt = (creationTime ?? Faker.Date.RecentOffset(30)).Date.ToUniversalTime();
        var rate = accountType == AccountType.Checking
            ? null
            : interestRate ?? new AccountInterestRate(Faker.Random.Decimal(0.01m, 0.15m));

#pragma warning disable CS8600
        var account = (Account)Activator.CreateInstance(
            typeof(Account),
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            null,
            null
        ) ?? throw new InvalidOperationException();
#pragma warning restore CS8600

        SetProperty(account, nameof(Account.Id), accountId);
        SetProperty(account, nameof(Account.OwnerId), ownerId);
        SetProperty(account, nameof(Account.Type), accountType);
        SetProperty(account, nameof(Account.InterestRate), rate);
        SetProperty(account, nameof(Account.CreationTimeUtc), new DateTimeOffset(createdAt));

        var txHistory = new List<Transaction>();
        SetField(account, "_transactionHistory", txHistory);

        DateTimeOffset txTime;
        if (closeTimeUtc.HasValue)
        {
            if (createdAt > closeTimeUtc.Value)
            {
                throw new ArgumentException($"{nameof(creationTime)} cannot be after {nameof(closeTimeUtc)}");
            }

            var maxTxTime = closeTimeUtc.Value.AddMinutes(-1);
            txTime = Faker.Date.BetweenOffset(createdAt, maxTxTime).UtcDateTime;
        }
        else
        {
            txTime = createdAt.AddMinutes(Faker.Random.Int(0, 1440)).ToUniversalTime();
        }

        var transactionsToCreate = balance > 0 && closeTimeUtc == null ? Faker.Random.Int(1, 2) : 1;

        var remainingBalance = balance;

        for (var i = 0; i < transactionsToCreate; i++)
        {
            var txAmount = i == transactionsToCreate - 1
                ? remainingBalance
                : Faker.Random.Decimal(0.01m, remainingBalance);
            remainingBalance -= txAmount;

#pragma warning disable CS8600
            var tx = (Transaction)Activator.CreateInstance(
                typeof(Transaction),
                BindingFlags.Instance | BindingFlags.Public,
                null,
                [accountId, TransactionType.Debit, new Currency(code, txAmount), Faker.Lorem.Sentence(), null],
                null
            ) ?? throw new InvalidOperationException();
#pragma warning restore CS8600

            SetProperty(tx, nameof(Transaction.TimeUtc), txTime);

            txHistory.Add(tx);
        }

        var computedBalance = closeTimeUtc.HasValue ? decimal.Zero : txHistory.Sum(t => t.Amount.Amount);

        SetProperty(account, nameof(Account.Balance), new Currency(code, computedBalance));

        var modifiedAt = closeTimeUtc ?? txHistory.Max(t => t.TimeUtc);
        SetProperty(account, nameof(Account.ModifiedAt), modifiedAt);

        if (closeTimeUtc.HasValue)
        {
            SetProperty(account, nameof(Account.ClosingTimeUtc), closeTimeUtc.Value);
            SetProperty(account, nameof(Account.Status), AccountStatus.Closed);
        }
        else
        {
            var status = isFrozen ? AccountStatus.Frozen : AccountStatus.Active;
            SetProperty(account, nameof(Account.Status), status);
        }

        return account;
    }

    private static void SetProperty<T>(object obj, string propName, T value)
    {
        var type = obj.GetType();
        var prop = type.GetProperty(propName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        if (prop?.SetMethod != null)
        {
            prop.SetValue(obj, value);
        }
        else
        {
            var backingField = type.GetField($"<{propName}>k__BackingField",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (backingField == null)
                throw new InvalidOperationException($"Neither setter nor backing field found for {propName}");
            backingField.SetValue(obj, value);
        }
    }

    private static void SetField<T>(object obj, string fieldName, T value)
    {
        var field = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
            throw new InvalidOperationException($"Field {fieldName} not found on {obj.GetType().Name}");
        field.SetValue(obj, value);
    }
}