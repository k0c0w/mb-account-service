using AccountService.Features.Domain;
using JetBrains.Annotations;

namespace AccountService.Features.AccountStatementFeatures.GetAccountStatement;

public sealed record TransactionInStatementDto
{
    /// <summary>
    /// Transaction UTC Time
    /// </summary>
    // Resharper disable once. Property is being used by serialization.
    [UsedImplicitly]
    public required DateTime TransactionTime { get; init; }
        
    /// <summary>
    /// Type of this transaction
    /// </summary>
    // Resharper disable once. Property is being used by serialization.
    [UsedImplicitly]
    public required TransactionType TransactionType { get; init; }
        
    /// <summary>
    /// Amount of this transaction
    /// </summary>
    // Resharper disable once. Property is being used by serialization.
    [UsedImplicitly]
    public required decimal Amount { get; init; }
        
    /// <summary>
    /// Account balance after transaction has been applied
    /// </summary>
    // Resharper disable once. Property is being used by serialization.
    [UsedImplicitly]
    public required decimal AccountBalanceAfterTransaction { get; init; }
        
    /// <summary>
    /// Description of this transaction
    /// </summary>
    // Resharper disable once. Property is being used by serialization.
    [UsedImplicitly]
    public required string TransactionDescription { get; init; }
}