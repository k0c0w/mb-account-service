// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
namespace AccountService.Persistence.Infrastructure.DataAccess;

public sealed class InboxDeadMessage
{
    public long Id { get; init; }
    
    public required string MessageId { get; init; }
    
    public required DateTimeOffset ReceivedAt { get; init; }
    
    public required string Handler { get; init; }
    
    public required string Payload { get; init; }
    
    public required string Error { get; init; }
}
