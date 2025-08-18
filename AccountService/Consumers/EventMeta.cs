// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// Data contract
namespace AccountService.Consumers;

public sealed record EventMeta
{
    public string Source { get; init; } = string.Empty;

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    // Data contract
    public string Version { get; init; } = string.Empty;
    
    public Guid CorrelationId { get; init; }
    
    public Guid CausationId { get; init; }
}
