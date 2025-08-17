// ReSharper disable UnusedAutoPropertyAccessor.Global
// Get are used in serialization
namespace AccountService.Features.Domain.Events;

public sealed class AccountOpenedEvent : IDomainEvent
{
    public Guid EventId { get; }

    public DateTimeOffset OccurredAt { get; }
    public EventMeta Meta { get; }

    public Guid OwnerId { get; }

    public string Currency { get; } 

    public string Type { get; } 

    public Guid AccountId { get; }

    public AccountOpenedEvent(Account account)
    {
        EventId = Guid.CreateVersion7();
        Meta = new EventMeta
        {
            CorrelationId = EventId,
            Version = "v1",
            CausationId = EventId
        };

        OccurredAt = account.CreationTimeUtc;
        OwnerId = account.OwnerId;
        Currency = account.Balance.Code.Value;
        Type = Enum.GetName(account.Type) ?? throw new ArgumentException("Unspecified account type.");
        AccountId = account.Id;
    }
}