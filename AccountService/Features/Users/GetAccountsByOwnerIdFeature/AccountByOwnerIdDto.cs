namespace AccountService.Features;

public record AccountByOwnerIdDto
{
    /// <summary>
    /// Account Identifier
    /// </summary>
    public required Guid Id { get; init; }
    
    /// <summary>
    /// Account currency
    /// </summary>
    public required string CurrencyCode { get; init; }
}