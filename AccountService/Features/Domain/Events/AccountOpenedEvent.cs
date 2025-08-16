using System.Runtime.Serialization;

namespace AccountService.Features.Domain.Events;

public sealed class AccountOpenedEvent(Account account) : AccountEvent(account.Id)
{
    public Guid OwnerId { get; } = account.OwnerId;

    public string Currency { get; } = account.Balance.Code.Value;

    public string Type { get; } = Enum.GetName(account.Type) ?? throw new ArgumentException("Unspecified account type.");
}