// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// Data contract
namespace AccountService.Consumers.Antifraud;

public sealed class ClientStateChanged
{
    public Guid ClientId { get; init; }
}