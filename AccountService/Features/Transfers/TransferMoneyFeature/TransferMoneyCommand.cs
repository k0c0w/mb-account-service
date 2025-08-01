using MediatR;

namespace AccountService.Features;

public sealed record TransferMoneyCommand : IRequest
{
    public required Guid SenderAccountId { get; init; }
    
    public required Guid RecipientAccountId { get; init; }
    
    public required decimal Amount { get; init; }
} 
