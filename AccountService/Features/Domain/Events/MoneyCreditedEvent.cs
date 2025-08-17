// ReSharper disable UnusedAutoPropertyAccessor.Global
// Data contract, properties are used
namespace AccountService.Features.Domain.Events;

public sealed class MoneyCreditedEvent : IDomainEvent
{
    public decimal Amount { get; } 

    public string Currency { get; }

    public Guid OperationId { get; }
    
    public Guid AccountId { get; }
    
    public MoneyCreditedEvent(Transaction transaction)
    {
        if (transaction.Type != TransactionType.Credit)
        {
            throw new ArgumentException("Wrong transaction passed.");
        }
        
        Amount = transaction.Amount.Amount;
        Currency = transaction.Amount.Code.Value;
        OperationId = transaction.Id;
        AccountId = transaction.AccountId;
    }
}