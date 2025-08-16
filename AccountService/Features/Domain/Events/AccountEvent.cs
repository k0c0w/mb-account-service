namespace AccountService.Features.Domain.Events;

public abstract class AccountEvent(Guid accountId) : DomainEvent
{
    public Guid AccountId { get; } = accountId;
}