// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// Getters are used by serialization
namespace AccountService.Features.Domain.Events;

public sealed record EventMeta
{
    public string Source => "account-service";

    public required string Version { get; init; }
    
    public required Guid CorrelationId { get; init; }
    
    public required Guid CausationId { get; init; }
}
