namespace AccountService.Features;

public record AccountByOwnerIdDto
{
    public required Guid Id { get; init; }
    
    public required string CurrencyCode { get; init; }
}