using AccountService.Domain;

namespace AccountService.Features;

public record CreatedAccountDto
{
    /// <summary>
    /// Identity of created account
    /// </summary>
    public required Guid Id { get; init; }
    
    /// <summary>
    /// Identity of account owner
    /// </summary>
    public required Guid OwnerId { get; init; } 
    
    /// <summary>
    /// Account type
    /// </summary>
    public required AccountType Type { get; init; }
        
    /// <summary>
    /// Account balance
    /// </summary>
    public required decimal Balance { get; init; }
    
    /// <summary>
    /// Account currency (iso 4217)
    /// </summary>
    public required string Currency { get; init; }
        
    /// <summary>
    /// Rate of interest for this account
    /// </summary>
    public required decimal? InterestRate { get; init; }
        
    /// <summary>
    /// This account creation time UTC
    /// </summary>
    public required DateTimeOffset CreationTimeUtc { get; init; }
        
    /// <summary>
    /// This account closing time UTC
    /// </summary>
    public required DateTimeOffset? ClosingTimeUtc { get; init; }
}