using AccountService.Features.Domain.Events;

namespace AccountService.Features.Domain.Services;

public interface IEventPublisher<in T> where T : DomainEvent
{
    Task PublishAsync(T @event, CancellationToken ct = default);
}