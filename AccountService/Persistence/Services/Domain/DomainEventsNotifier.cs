using AccountService.Features.Domain.Events;
using AccountService.Features.Domain.Services;
using AccountService.Persistence.Infrastructure.DataAccess;

namespace AccountService.Persistence.Services.Domain;

public abstract class DomainEventsNotifier(AccountServiceDbContext dbContext) : IDomainEventNotifier
{
    public Task NotifyAsync<T>(EventEnvelope<T> occuredEvent) where T : IDomainEvent
    {
        var outbox = GetFrom(occuredEvent as dynamic);
        
        return dbContext.OutboxMessages
            .AddAsync(outbox)
            .AsTask();
    }

    protected abstract OutboxMessage GetFrom(EventEnvelope<MoneyCreditedEvent> occuredEvent);
    
    protected abstract OutboxMessage GetFrom(EventEnvelope<MoneyDebitedEvent> occuredEvent);
    
    protected abstract OutboxMessage GetFrom(EventEnvelope<TransferCompletedEvent> occuredEvent);
    
    protected abstract OutboxMessage GetFrom(EventEnvelope<AccountOpenedEvent> occuredEvent);
    
    protected abstract OutboxMessage GetFrom(EventEnvelope<InterestAccruedEvent> occuredEvent);
    
    /// <summary>
    /// Fallback method
    /// </summary>
    /// <param name="occuredEvent">event with no concrete handler</param>
    /// <exception cref="NotImplementedException">Event was not registered or handled.</exception>
    // ReSharper disable once VirtualMemberNeverOverridden.Global
    protected virtual OutboxMessage GetFrom(EventEnvelope<IDomainEvent> occuredEvent)
    {
        throw new NotImplementedException("Unknown event occured.", 
            new ArgumentOutOfRangeException(nameof(occuredEvent), occuredEvent.GetType(), 
                $"Forgot to register {occuredEvent.GetType().FullName} event?"));
    }
}