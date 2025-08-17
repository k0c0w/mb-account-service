
namespace AccountService.Features.Domain.Events;

public static class EventsFactory
{
    public static EventEnvelope<AccountOpenedEvent> AccountOpenedV1(Account account)
    {
        var id = account.Id;
        
        return new EventEnvelope<AccountOpenedEvent>
        {
            Id = id,
            Payload = new AccountOpenedEvent(account),
            OccuredAt = account.CreationTimeUtc,
            Meta = new EventMeta
            {
                Version = "v1",
                CausationId = id,
                CorrelationId = id
            }
        };
    }
    
    public static EventEnvelope<MoneyCreditedEvent> MoneyCreditedV1(Transaction creditTransaction)
    {
        return new EventEnvelope<MoneyCreditedEvent>
        {
            Id = creditTransaction.Id,
            Payload = new MoneyCreditedEvent(creditTransaction),
            OccuredAt = creditTransaction.TimeUtc,
            Meta = new EventMeta
            {
                Version = "v1",
                CausationId = creditTransaction.Id,
                CorrelationId = creditTransaction.Id
            }
        };
    }
    
    public static EventEnvelope<MoneyDebitedEvent> MoneyDebitedV1(Transaction debitTransaction)
    {
        return new EventEnvelope<MoneyDebitedEvent>
        {
            Id = debitTransaction.Id,
            Payload = new MoneyDebitedEvent(debitTransaction),
            OccuredAt = debitTransaction.TimeUtc,
            Meta = new EventMeta
            {
                Version = "v1",
                CausationId = debitTransaction.Id,
                CorrelationId = debitTransaction.Id
            }
        };
    }
    
    public static EventEnvelope<InterestAccruedEvent> InterestAccruedV1(DateTimeOffset from, DateTimeOffset to, decimal amount, Guid accountId)
    {
        var id = Guid.CreateVersion7();
        
        return new EventEnvelope<InterestAccruedEvent>
        {
            Id = id,
            Payload = new InterestAccruedEvent(from, to, amount, accountId),
            OccuredAt = DateTimeOffset.UtcNow,
            Meta = new EventMeta
            {
                Version = "v1",
                CausationId = id,
                CorrelationId = id
            }
        };
    }
    
    public static EventEnvelope<TransferCompletedEvent> TransferCompletedV1(Transaction creditTransaction, Transaction debitTransaction)
    {
        var id = Guid.CreateVersion7();
        
        return new EventEnvelope<TransferCompletedEvent>
        {
            Id = id,
            Payload = new TransferCompletedEvent(creditTransaction, debitTransaction),
            OccuredAt = DateTimeOffset.UtcNow,
            Meta = new EventMeta
            {
                Version = "v1",
                CausationId = id,
                CorrelationId = id
            }
        };
    }
}