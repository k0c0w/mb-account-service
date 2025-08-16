namespace AccountService.Features.Domain.Events;

public interface IDomainEvent
{
    public Guid EventId { get; }
    
    public DateTimeOffset OccurredAt { get; }
}