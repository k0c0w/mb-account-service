using AccountService.Features.Domain;
using AccountService.Tests.Factories;

namespace AccountService.Tests.UnitTests.DomainTests;

public class AccountTests : TestsBase
{
    [Theory]
    [InlineData(TransactionType.Debit, 50, 0, 50)] 
    [InlineData(TransactionType.Credit, 25, 50, 25)]
    public void ApplyIncomingTransaction_ShouldAcceptTransaction_WhenValidIncomingTransaction(
        TransactionType incomingTransactionType,
        decimal transactionAmount,
        decimal startingBalance,
        decimal expectedBalance)
    {
        // Arrange
        var currencyCode = new CurrencyCode("RUB");
        var account = AccountsFactory.CreateAccount(
            accountType: AccountType.Checking,
            balance: startingBalance,
            currencyCode: currencyCode
        );

        var accountTransactionsBefore = account.TransactionHistory.Count;
        var money = new Currency(currencyCode, transactionAmount);
        var timeBefore = DateTime.UtcNow;

        // Act
        account.ApplyIncomingTransaction(incomingTransactionType, money);

        // Assert
        Assert.Equal(expectedBalance, account.Balance.Amount);
        Assert.Equal(accountTransactionsBefore + 1, account.TransactionHistory.Count);

        var lastTransaction = account.TransactionHistory.OrderBy(x => x.TimeUtc).Last();
        Assert.Equal(transactionAmount, lastTransaction.Amount.Amount);
        Assert.Equal(currencyCode, lastTransaction.Amount.Code);
        Assert.True(timeBefore <= lastTransaction.TimeUtc);
        Assert.Null(lastTransaction.CounterpartyAccountId);
        Assert.NotEqual(Guid.Empty, lastTransaction.Id);
        Assert.Equal(account.Id, lastTransaction.AccountId);
        Assert.Equal(incomingTransactionType, lastTransaction.Type);
    }

    [Fact]
    public void ApplyIncomingTransaction_ShouldThrow_WhenClosed()
    {
        // Arrange
        const string expectedErrMessage = "Account is already closed.";
        const DomainException.DomainExceptionType expectedErrType = DomainException.DomainExceptionType.ValidationError;
        var currencyCode = new CurrencyCode("RUB");
        var account = AccountsFactory.CreateAccount(
            accountType: AccountType.Checking,
            balance: 0m,
            currencyCode: currencyCode,
            creationTime: DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
            closeTimeUtc: DateTimeOffset.UtcNow
        );
        var money = new Currency(currencyCode, 50m);
        
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => account.ApplyIncomingTransaction(TransactionType.Credit, money));
        AssertDomainException(ex, expectedErrMessage, expectedErrType);
    }
    
    [Fact]
    public void ApplyIncomingTransaction_ShouldThrow_WhenNotEnoughMoney()
    {
        // Arrange
        const string expectedErrMessage = "Insufficient account balance.";
        const DomainException.DomainExceptionType expectedErrType = DomainException.DomainExceptionType.ValidationError;
        var currencyCode = new CurrencyCode("RUB");
        var zeroBalancedAccount = AccountsFactory.CreateAccount(
            accountType: AccountType.Checking,
            balance: 0m,
            currencyCode: currencyCode
        );
        var withdrawMoney = new Currency(currencyCode, 50m);
    
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            zeroBalancedAccount.ApplyIncomingTransaction(TransactionType.Credit, withdrawMoney)
        );
        AssertDomainException(exception, expectedErrMessage, expectedErrType);
    }

    [Fact]
    public void SendMoney_ShouldThrow_WhenRecipientIsSameAccount()
    {
        // Arrange
        const string expectedErrMessage = "Can not funds the same account.";
        const DomainException.DomainExceptionType expectedErrType = DomainException.DomainExceptionType.ValidationError;
        var accountId = Guid.CreateVersion7();
        var accountOwnerId = Guid.CreateVersion7();
        var creationTime = DateTime.UtcNow;
        const decimal accountBalance = 50;
        const decimal transactionMoney = 20;
        var accountType = PickRandom<AccountType>();
        var currencyCode = new CurrencyCode("RUB");
        var sender = AccountsFactory.CreateAccount(accountType, 
            accountBalance, 
            accountId: accountId, 
            ownerId: accountOwnerId,
            currencyCode: currencyCode,
            creationTime: creationTime
        );
        var recipient = AccountsFactory.CreateAccount(accountType, 
            accountBalance, 
            accountId: accountId, 
            ownerId: accountOwnerId,
            currencyCode: currencyCode,
            creationTime: creationTime
            );
        var money = new Currency(sender.Balance.Code, transactionMoney);
        
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => sender.SendMoney(recipient, money));
        AssertDomainException(exception, expectedErrMessage, expectedErrType);
    }
    
    [Fact]
    public void SendMoney_ShouldSendMoney_WhenEnoughBalance()
    {
        // Arrange
        const decimal debitBalanceAmount = 50m;
        const decimal sendMoneyAmount = 25m;
        const decimal expectedABalance = debitBalanceAmount - sendMoneyAmount;
        var currencyCode = new CurrencyCode("RUB");
        var sendMoney = new Currency(currencyCode, sendMoneyAmount);
        var accountType = PickRandom<AccountType>();
        
        var sender = AccountsFactory.CreateAccount(
            accountType: accountType,
            balance: debitBalanceAmount,
            currencyCode: currencyCode
        );

        var recipient = AccountsFactory.CreateAccount(
            accountType: AccountType.Checking,
            balance: 0,
            currencyCode: currencyCode
        );
        
        // Act
        sender.SendMoney(recipient, sendMoney);
        
        // Assert
        Assert.Equal(expectedABalance, sender.Balance.Amount);
        Assert.Equal(sendMoneyAmount, recipient.Balance.Amount);
    }

    [Fact]
    public void SendMoney_ShouldThrow_WhenSenderClosed()
    {
        const string expectedErrMessage = "Sender account is already closed.";
        const DomainException.DomainExceptionType expectedErrType = DomainException.DomainExceptionType.ValidationError;
        var currencyCode = new CurrencyCode("RUB");
        var account = AccountsFactory.CreateAccount(
            accountType: AccountType.Checking,
            balance: 0m,
            currencyCode: currencyCode,
            creationTime: DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
            closeTimeUtc: DateTimeOffset.UtcNow
        );
        var recipient = AccountsFactory.CreateAccount(
            accountType: AccountType.Checking,
            balance: 0,
            currencyCode: currencyCode
        );
        var sendMoney = new Currency(currencyCode, 50m);
        
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => account.SendMoney(recipient, sendMoney));
        AssertDomainException(ex, expectedErrMessage, expectedErrType);
    }
    
    [Fact]
    public void SendMoney_ShouldThrow_WhenRecipientClosed()
    {
        const string expectedErrMessage = "Recipient is already closed.";
        const DomainException.DomainExceptionType expectedErrType = DomainException.DomainExceptionType.ValidationError;
        var currencyCode = new CurrencyCode("RUB");
        var recipient = AccountsFactory.CreateAccount(
            accountType: AccountType.Checking,
            balance: 0m,
            currencyCode: currencyCode,
            creationTime: DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
            closeTimeUtc: DateTimeOffset.UtcNow
        );
        var sender = AccountsFactory.CreateAccount(
            accountType: AccountType.Checking,
            balance: 50m,
            currencyCode: currencyCode
        );
        var sendMoney = new Currency(currencyCode, 50m);
        
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => sender.SendMoney(recipient, sendMoney));
        AssertDomainException(ex, expectedErrMessage, expectedErrType);
    }
    
    [Fact]
    public void SendMoney_ShouldThrow_WhenNotEnoughBalance()
    {
        // Arrange
        const string expectedErrMessage = "Insufficient account balance.";
        const DomainException.DomainExceptionType expectedErrType = DomainException.DomainExceptionType.ValidationError;
        const decimal sendMoneyAmount = 25m;
        var currencyCode = new CurrencyCode("RUB");
        var sendMoney = new Currency(currencyCode, sendMoneyAmount);
        
        var sender = AccountsFactory.CreateAccount(
            accountType: AccountType.Checking,
            balance: 0,
            currencyCode: currencyCode
        );

        var recipient = AccountsFactory.CreateAccount(
            accountType: AccountType.Checking,
            balance: 0,
            currencyCode: currencyCode
        );
        
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => sender.SendMoney(recipient, sendMoney));
        AssertDomainException(exception, expectedErrMessage, expectedErrType);
    }
    
     [Fact]
    public void Close_WhenBalanceIsZeroAndAccountIsOpen()
    {
        // Arrange
        var account = AccountsFactory.CreateAccount(
            accountType: PickRandom<AccountType>(),
            balance: 0m,
            currencyCode:  new CurrencyCode("USD")
        );
        var timeBefore = DateTime.UtcNow;

        // Act
        account.Close();

        // Assert
        Assert.NotNull(account.ClosingTimeUtc);
        Assert.Equal(account.ModifiedAt, account.ClosingTimeUtc);
        Assert.True(timeBefore <= account.ClosingTimeUtc);
        Assert.True(account.CreationTimeUtc <= account.ClosingTimeUtc);
    }

    [Fact]
    public void Close_ShouldThrow_WhenAccountIsAlreadyClosed()
    {
        // Arrange
        const DomainException.DomainExceptionType exErrType = DomainException.DomainExceptionType.ValidationError;
        const string exMessage = "Account is already closed.";
        var account = AccountsFactory.CreateAccount(
            accountType: PickRandom<AccountType>(),
            balance: 0m,
            currencyCode: new CurrencyCode("USD"),
            creationTime: DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
            closeTimeUtc: DateTimeOffset.UtcNow
        );

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => account.Close());
        AssertDomainException(exception, exMessage, exErrType);
    }

    [Fact]
    public void Close_ShouldThrow_WhenBalanceIsNotZero()
    {
        // Arrange
        const DomainException.DomainExceptionType exErrType = DomainException.DomainExceptionType.ValidationError;
        const string exMessage = "All money must be withdrawn before the account is closed.";
        var account = AccountsFactory.CreateAccount(
            accountType: PickRandom<AccountType>(),
            balance: 100m,
            currencyCode: new CurrencyCode("USD")
        );

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => account.Close());
        AssertDomainException(exception, exMessage, exErrType);
    }

    private static void AssertDomainException(DomainException ex, 
        string expectedMessage, 
        DomainException.DomainExceptionType exceptionType)
    {
        Assert.Equal(expectedMessage, ex.Message);
        Assert.Equal(exceptionType, ex.Type);
    }
}
