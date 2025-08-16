namespace AccountService.Features.Domain.Events;

public class MoneyDebitedEvent : AccountEvent
{
    public decimal Amount { get; } 

    public string Currency { get; }

    public Guid OperationId { get; }

    public MoneyDebitedEvent(Transaction transaction) : base(transaction.AccountId)
    {
        if (transaction.Type != TransactionType.Debit)
        {
            throw new ArgumentException("Wrong transaction passed.");
        }
        
        Amount = transaction.Amount.Amount;
        Currency = transaction.Amount.Code.Value;
        OperationId = transaction.Id;
    }
}