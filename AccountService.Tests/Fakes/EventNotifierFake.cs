using AccountService.Consumers;
using AccountService.Features.Domain.Events;
using AccountService.Features.Domain.Services;

namespace AccountService.Tests.Fakes;

public class EventNotifierFake: IDomainEventNotifier
{
    private readonly List<object> _occuredEvents = [];

    public IReadOnlyList<object> OccuredEvents => _occuredEvents;
    
    public IEnumerable<Type> OccuredEventsTypes => _occuredEvents.Select(x => x.GetType()).Distinct();
    
    public Task NotifyAsync<T>(EventEnvelope<T> occuredEvent) where T : IDomainEvent
    {
        _occuredEvents.Add(occuredEvent);
        return Task.CompletedTask;
    }
}