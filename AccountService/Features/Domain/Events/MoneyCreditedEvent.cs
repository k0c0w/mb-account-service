namespace AccountService.Features.Domain.Events;

public sealed class MoneyCreditedEvent : AccountEvent
{
    public decimal Amount { get; } 

    public string Currency { get; }

    public Guid OperationId { get; }

    public MoneyCreditedEvent(Transaction transaction) : base(transaction.AccountId)
    {
        if (transaction.Type != TransactionType.Credit)
        {
            throw new ArgumentException("Wrong transaction passed.");
        }
        
        Amount = transaction.Amount.Amount;
        Currency = transaction.Amount.Code.Value;
        OperationId = transaction.Id;
    }
}