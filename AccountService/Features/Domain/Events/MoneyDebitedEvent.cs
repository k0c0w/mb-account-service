// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace AccountService.Features.Domain.Events;

public sealed class MoneyDebitedEvent : IDomainEvent
{
    public decimal Amount { get; } 

    public string Currency { get; }

    public Guid OperationId { get; }
    
    public Guid AccountId { get; }
    
    public MoneyDebitedEvent(Transaction transaction)
    {
        if (transaction.Type != TransactionType.Debit)
        {
            throw new ArgumentException("Wrong transaction passed.");
        }
        
        Amount = transaction.Amount.Amount;
        Currency = transaction.Amount.Code.Value;
        OperationId = transaction.Id;
        AccountId = transaction.AccountId;
    }
}