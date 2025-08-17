namespace AccountService.Persistence.Infrastructure.DataAccess;

public sealed class InboxMessage
{
    public required Guid Id { get; init; }
    
    public DateTimeOffset ProcessedAt { get; init; }
}