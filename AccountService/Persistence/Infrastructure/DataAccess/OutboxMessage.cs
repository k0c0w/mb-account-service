namespace AccountService.Persistence.Infrastructure.DataAccess;

public sealed class OutboxMessage
{
    public required Guid Id { get; init; }
    
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Payload { get; init; }

    public Dictionary<string, string> Properties { get; init; } = [];
    
    public Dictionary<string, object>? Headers { get; init; }
    
    public required DateTimeOffset OccuredAtUtc { get; init; }
    
    public DateTimeOffset? ProcessedAtUtc { get; set; }
}