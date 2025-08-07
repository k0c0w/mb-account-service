using JetBrains.Annotations;
using MediatR;

namespace AccountService.Features.Transfers.TransferMoneyFeature;

public sealed record TransferMoneyCommand : IRequest
{
    /// <summary>
    /// Sender account identity
    /// </summary>
    // Property is set by serialization.
    [UsedImplicitly]
    public required Guid SenderAccountId { get; init; }
    
    /// <summary>
    /// Receiver account identity
    /// </summary>
    // Property is set by serialization.
    [UsedImplicitly]
    public required Guid RecipientAccountId { get; init; }
    
    /// <summary>
    /// Amount of sent money
    /// </summary>
    // Property is set by serialization.
    [UsedImplicitly]
    public required decimal Amount { get; init; }
} 
