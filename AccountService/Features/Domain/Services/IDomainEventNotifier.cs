using AccountService.Features.Domain.Events;

namespace AccountService.Features.Domain.Services;

public interface IDomainEventNotifier
{
    Task NotifyAsync(IDomainEvent occuredEvent);
}