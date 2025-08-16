using AccountService.Features.Domain.Events;
using AccountService.Features.Domain.Services;

namespace AccountService.Tests.Fakes;

public class EventNotifierFake: IDomainEventNotifier
{
    private readonly List<IDomainEvent> _occuredEvents = [];

    public IReadOnlyList<IDomainEvent> OccuredEvents => _occuredEvents;
    
    public IEnumerable<Type> OccuredEventsTypes => _occuredEvents.Select(x => x.GetType()).Distinct();
    
    public Task NotifyAsync(IDomainEvent occuredEvent)
    {
        _occuredEvents.Add(occuredEvent);
        return Task.CompletedTask;
    }
}