using AccountService.Domain;

namespace AccountService.Features;

public sealed record AccountStatementDto
{
    /// <summary>
    /// Account Identity
    /// </summary>
    public required Guid AccountId { get; init; }
    
    /// <summary>
    /// Account owner identity
    /// </summary>
    public required Guid OwnerId { get; init; }
    
    /// <summary>
    /// Account currency
    /// </summary>
    public required string CurrencyCode { get; init; }
    
    /// <summary>
    /// Start of this statement period
    /// </summary>
    public required DateTime StatementPeriodStart { get; init; }
    
    /// <summary>
    /// End of this statement period
    /// </summary>
    public required DateTime StatementPeriodEnd { get; init; }
    
    /// <summary>
    /// Account initial balance at <see cref="StatementPeriodStart"/>
    /// </summary>
    public required decimal AccountBalanceAtStatementPeriodStart { get; init; }
    
    /// <summary>
    /// Transactions which were applied during statement period
    /// </summary>
    public required IEnumerable<AccountTransactionInStatementDto> Transactions { get; init; }

    public sealed record AccountTransactionInStatementDto
    {
        /// <summary>
        /// Transaction UTC Time
        /// </summary>
        public required DateTime TransactionTime { get; init; }
        
        /// <summary>
        /// Type of this transaction
        /// </summary>
        public required TransactionType TransactionType { get; init; }
        
        /// <summary>
        /// Amount of this transaction
        /// </summary>
        public required decimal Amount { get; init; }
        
        /// <summary>
        /// Account balance after transaction has been applied
        /// </summary>
        public required decimal AccountBalanceAfterTransaction { get; init; }
        
        /// <summary>
        /// Description of this transaction
        /// </summary>
        public required string TransactionDescription { get; init; }
    }
}