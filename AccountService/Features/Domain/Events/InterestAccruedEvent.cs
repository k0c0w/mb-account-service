// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// Instantiated via reflection, Properties are used by consumers (they are data contracts)
namespace AccountService.Features.Domain.Events;

public sealed class InterestAccruedEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.CreateVersion7();

    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
    
    public EventMeta Meta { get; }
    
    public DateTimeOffset PeriodFrom { get; }

    public DateTimeOffset PeriodTo { get; }
    
    public decimal Amount { get; }
    
    public Guid AccountId { get; }

    public InterestAccruedEvent(DateTimeOffset from, DateTimeOffset to, decimal amount, Guid accountId)
    {
        if (PeriodFrom > PeriodTo || PeriodTo > DateTimeOffset.UtcNow)
        {
            throw new ArgumentException("Wrong period.");
        }

        if (Math.Abs(amount) == 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Accrual amount can not be 0.");
        }

        if (accountId == Guid.Empty)
        {
            throw new ArgumentException(nameof(accountId));
        }

        PeriodFrom = from;
        PeriodTo = to;
        Amount = amount;
        AccountId = accountId;
        Meta = new EventMeta
        {
            Version = "v1",
            CausationId = EventId,
            CorrelationId = EventId
        };
    }
}