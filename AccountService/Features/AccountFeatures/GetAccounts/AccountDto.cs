using AccountService.Features.Domain;
using JetBrains.Annotations;

namespace AccountService.Features.AccountFeatures.GetAccounts;

public class AccountDto
{
    /// <summary>
    /// Account Identity
    /// </summary>
    // Property is being used by serialization.
    [UsedImplicitly]
    public required Guid Id { get; init; }
        
    /// <summary>
    /// This Account owner identity
    /// </summary>
    // Property is being used by serialization.
    [UsedImplicitly]
    public required Guid OwnerId { get; init; } 
    
    /// <summary>
    /// Account type
    /// </summary>
    // Property is being used by serialization.
    [UsedImplicitly]
    public required AccountType Type { get; init; }
        
    /// <summary>
    /// Account balance
    /// </summary>
    // Property is being used by serialization.
    [UsedImplicitly]
    public required decimal Balance { get; init; }
    
    /// <summary>
    /// Account currency (iso 4217)
    /// </summary>
    // Property is being used by serialization.
    [UsedImplicitly]
    public required string Currency { get; init; }
        
    /// <summary>
    /// Rate of interest for this account
    /// </summary>
    // Property is being used by serialization.
    [UsedImplicitly]
    public required decimal? InterestRate { get; init; }
        
    /// <summary>
    /// This account creation time UTC
    /// </summary>
    // Property is being used by serialization.
    [UsedImplicitly]
    public required DateTimeOffset CreationTimeUtc { get; init; }
        
    /// <summary>
    /// This account closing time UTC
    /// </summary>
    // Property is being used by serialization.
    [UsedImplicitly]
    public required DateTimeOffset? ClosingTimeUtc { get; init; }
}