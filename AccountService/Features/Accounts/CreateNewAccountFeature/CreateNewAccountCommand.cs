using AccountService.Domain;
using MediatR;

namespace AccountService.Features;

public sealed record CreateNewAccountCommand : IRequest<CreatedAccountDto>
{
    public required Guid OwnerId { get; init; } 
    
    public required string CurrencyCode { get; init; }
    
    public required AccountType AccountType { get; init; }
    
    public decimal? InterestRate { get; init; }
}
