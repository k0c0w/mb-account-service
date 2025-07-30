namespace AccountService.Features;

public sealed record AccountStatementDto
{
    public required Guid AccountId { get; init; }
    
    public required Guid OwnerId { get; init; }
    
    public required string CurrencyCode { get; init; }
    
    public required DateTime StatementPeriodStart { get; init; }
    
    public required DateTime StatementPeriodEnd { get; init; }
    
    public required decimal AccountBalanceAtStatementPeriodStart { get; init; }
    
    public required IEnumerable<AccountTransactionInStatementDto> Transactions { get; init; }

    public sealed record AccountTransactionInStatementDto
    {
        public required DateTime TransactionTime { get; init; }
        
        public required string TransactionType { get; init; }
        
        public required decimal Amount { get; init; }
        
        public required decimal AccountBalanceAfterTransaction { get; init; }
        
        public required string TransactionDescription { get; init; }
    }
}