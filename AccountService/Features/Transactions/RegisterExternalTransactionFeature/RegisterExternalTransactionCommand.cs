using AccountService.Domain;
using JetBrains.Annotations;
using MediatR;

namespace AccountService.Features.Transactions.RegisterExternalTransactionFeature;

public sealed record RegisterExternalTransactionCommand : IRequest
{
    /// <summary>
    /// Identity of account which this transaction will be applied to
    /// </summary>
    // Property is set by serialization.
    [UsedImplicitly]
    public required Guid AccountId { get; init; }
    
    /// <summary>
    /// Currency of this transaction
    /// </summary>
    // Property is set by serialization.
    [UsedImplicitly]
    public required string CurrencyCode { get; init; }
    
    /// <summary>
    /// Amount of transaction
    /// </summary>
    public required decimal Amount { get; init; }
    
    /// <summary>
    /// Transaction type
    /// </summary>
    public required TransactionType TransactionType { get; init; }
}
