namespace AccountService.Features;

public class AccountDto
{
    public required Guid Id { get; init; }
        
    public required Guid OwnerId { get; init; } 
    
    public required string Type { get; init; }
        
    public required decimal Balance { get; init; }
    
    public required string Currency { get; init; }
        
    public required decimal? InterestRate { get; init; }
        
    public required DateTimeOffset CreationTimeUtc { get; init; }
        
    public required DateTimeOffset? ClosingTimeUtc { get; init; }
}