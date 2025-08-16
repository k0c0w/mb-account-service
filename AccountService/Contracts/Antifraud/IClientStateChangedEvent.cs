namespace AccountService.Contracts.Antifraud;

public interface IClientStateChangedEvent
{
    Guid EventId { get; }
    
    DateTimeOffset OccuredAt { get; }
    
    Guid ClientId { get; }
}