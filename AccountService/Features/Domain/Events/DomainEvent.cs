namespace AccountService.Features.Domain.Events;

public abstract class DomainEvent
{
    public Guid EventId { get; } = Guid.CreateVersion7();
    
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}