namespace AccountService.Consumers;

public class Envelope<T>
{
    public required Guid Id { get; init; }
    
    public required DateTimeOffset OccuredAt { get; init; }
    
    public required T Payload { get; init; }
    
    public required EventMeta Meta { get; init; }
}