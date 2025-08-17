// ReSharper disable  UnusedAutoPropertyAccessor.Global
// Data contract, properties are used
namespace AccountService.Features.Domain.Events;

public sealed class TransferCompletedEvent : IDomainEvent
{
    public Guid SourceAccountId { get; }
    
    public Guid CreditTransactionId { get; }
    
    public Guid DestinationAccountId { get; }
    
    public Guid DebitTransactionId { get; }
    
    public decimal Amount { get; }
    
    public string Currency { get; }

    public TransferCompletedEvent(Transaction creditTransaction, Transaction debitTransaction)
    {
        if (creditTransaction.Type != TransactionType.Credit || debitTransaction.Type != TransactionType.Debit)
        {
            throw new ArgumentException("Transaction type mismatch.");
        }

        if (creditTransaction.Amount != debitTransaction.Amount 
            || creditTransaction.CounterpartyAccountId != debitTransaction.AccountId
            || creditTransaction.AccountId != debitTransaction.CounterpartyAccountId)
        {
            throw new ArgumentException("Arguments are inconsistent.");
        }

        SourceAccountId = creditTransaction.AccountId;
        CreditTransactionId = creditTransaction.Id;
        DebitTransactionId = debitTransaction.Id;
        DestinationAccountId = debitTransaction.AccountId;

        var transferCurrency = creditTransaction.Amount;
        Amount = transferCurrency.Amount;
        Currency = transferCurrency.Code.Value;
    }
}