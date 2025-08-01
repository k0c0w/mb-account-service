using JetBrains.Annotations;

namespace AccountService.Features.Users.GetAccountsByOwnerIdFeature;

public record AccountByOwnerIdDto
{
    /// <summary>
    /// Account Identifier
    /// </summary>
    // Property is being used by serialization
    [UsedImplicitly]
    public required Guid Id { get; init; }
    
    /// <summary>
    /// Account currency
    /// </summary>
    // Property is being used by serialization
    [UsedImplicitly]
    public required string CurrencyCode { get; init; }
}