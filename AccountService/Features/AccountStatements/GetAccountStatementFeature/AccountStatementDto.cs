using JetBrains.Annotations;

namespace AccountService.Features.AccountStatements.GetAccountStatementFeature;

public sealed record AccountStatementDto
{
    /// <summary>
    /// Account Identity
    /// </summary>
    // Resharper disable once. Property is being used by serialization.
    [UsedImplicitly]
    public required Guid AccountId { get; init; }
    
    /// <summary>
    /// Account owner identity
    /// </summary>
    // Resharper disable once. Property is being used by serialization.
    [UsedImplicitly]
    public required Guid OwnerId { get; init; }
    
    /// <summary>
    /// Account currency
    /// </summary>
    // Resharper disable once. Property is being used by serialization.
    [UsedImplicitly]
    public required string CurrencyCode { get; init; }
    
    /// <summary>
    /// Start of this statement period
    /// </summary>
    // Resharper disable once. Property is being used by serialization.
    [UsedImplicitly]
    public required DateTime StatementPeriodStart { get; init; }
    
    /// <summary>
    /// End of this statement period
    /// </summary>
    // Resharper disable once. Property is being used by serialization.
    [UsedImplicitly]
    public required DateTime StatementPeriodEnd { get; init; }
    
    /// <summary>
    /// Account initial balance at <see cref="StatementPeriodStart"/>
    /// </summary>
    // Resharper disable once. Property is being used by serialization.
    [UsedImplicitly]
    public required decimal AccountBalanceAtStatementPeriodStart { get; init; }
    
    /// <summary>
    /// Transactions which were applied during statement period
    /// </summary>
    // Resharper disable once. Property is being used by serialization.
    [UsedImplicitly]
    public required IEnumerable<TransactionInStatementDto> Transactions { get; init; }
}