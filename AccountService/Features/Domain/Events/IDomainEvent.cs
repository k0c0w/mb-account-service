// ReSharper disable UnusedMemberInSuper.Global
// Data contract
namespace AccountService.Features.Domain.Events;

public interface IDomainEvent
{
    public Guid EventId { get; }
    
    public DateTimeOffset OccurredAt { get; }
    
    public EventMeta Meta { get; }
}