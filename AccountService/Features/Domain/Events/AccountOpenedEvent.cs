namespace AccountService.Features.Domain.Events;

public sealed class AccountOpenedEvent(Account account) : IDomainEvent
{
    public Guid EventId { get; } = Guid.CreateVersion7();

    public DateTimeOffset OccurredAt { get; } = account.CreationTimeUtc;

    public Guid OwnerId { get; } = account.OwnerId;

    public string Currency { get; } = account.Balance.Code.Value;

    public string Type { get; } = Enum.GetName(account.Type) ?? throw new ArgumentException("Unspecified account type.");

    public Guid AccountId { get; } = account.Id;
}