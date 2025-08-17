// ReSharper disable UnusedAutoPropertyAccessor.Global
// Data contract
namespace AccountService.Features.Domain.Events;

public class EventEnvelope<T> where T : IDomainEvent
{
    public required Guid Id { get; init; }

    public required DateTimeOffset OccuredAt { get; init; }
    
    public required T Payload { get; init; }
    
    public required EventMeta Meta { get; init; }
}