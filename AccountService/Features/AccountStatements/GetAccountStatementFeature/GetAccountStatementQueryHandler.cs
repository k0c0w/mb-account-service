using AccountService.Domain;
using MediatR;

namespace AccountService.Features;

public sealed class GetAccountStatementQueryHandler(IAccountRepository accountRepository)
    : IRequestHandler<GetAccountStatementQuery, AccountStatementDto>
{
    private IAccountRepository AccountRepository => accountRepository;
    
    public async Task<AccountStatementDto> Handle(GetAccountStatementQuery request, CancellationToken ct)
    {
        ThrowIfInvalidPeriod(request.PeriodStartUtc, request.PeriodEndUtc);
        
        var account = await AccountRepository.GetByIdAsync(request.AccountId, ct);

        var actualStatementStartTime = Max(request.PeriodStartUtc, account.CreationTimeUtc);
        var actualStatementEndTime = Min(request.PeriodEndUtc, account.ClosingTimeUtc ?? DateTimeOffset.UtcNow);
        
        var transactions = account.TransactionHistory
            .OrderBy(t => t.TimeUtc)
            .SkipWhile(t => t.TimeUtc < actualStatementStartTime)
            .TakeWhile(t => t.TimeUtc <= actualStatementEndTime)
            .ToArray();
        
        var statementTransactions = new AccountStatementDto.AccountTransactionInStatementDto[transactions.Length];
        
        var balanceAtStart = account.GetBalanceAt(request.PeriodStartUtc);
        var balanceAtMoment = balanceAtStart;
        
        foreach (var (i,transaction) in transactions.Index())
        {
            var transactionAmount = transaction.Amount.Amount;
            balanceAtMoment += transaction.Type == TransactionType.Debit ? transactionAmount : -transactionAmount;

            statementTransactions[i] = new()
            {
                AccountBalanceAfterTransaction = balanceAtMoment,
                Amount = transaction.Amount.Amount,
                TransactionTime = transaction.TimeUtc.DateTime,
                TransactionDescription = transaction.Description,
                TransactionType = transaction.Type.ToString(),
            };
        }

        return new AccountStatementDto
        {
            AccountId = account.Id,
            CurrencyCode = account.Balance.Code.ToString(),
            OwnerId = account.OwnerId,
            StatementPeriodStart = actualStatementStartTime.DateTime,
            StatementPeriodEnd = actualStatementEndTime.DateTime,
            Transactions = statementTransactions,
            AccountBalanceAtStatementPeriodStart = balanceAtStart
        };
    }

    private static void ThrowIfInvalidPeriod(DateTimeOffset start, DateTimeOffset end)
    {
        var requestTime = DateTimeOffset.UtcNow;
        if (start > requestTime)
        {
            throw DomainException.CreateValidationException("Can not calculate statement for future.", 
                new ArgumentOutOfRangeException( nameof(GetAccountStatementQuery.PeriodStartUtc),
                    start, 
                    "Start time must be less than now."));
        }

        if (end > requestTime)
        {
            throw DomainException.CreateValidationException("Can not calculate statement for future.", 
                new ArgumentOutOfRangeException( nameof(GetAccountStatementQuery.PeriodStartUtc),
                    end, 
                    "End time must be less than now."));
        }

        if (start >= end)
        {
            throw DomainException.CreateValidationException("Invalid statement calculation gap.", 
                new ArgumentException("Start time must be less than end time."));
        }
    }

    private static DateTimeOffset Max(DateTimeOffset a, DateTimeOffset b)
    {
        return a > b ? a : b;
    }
    
    private static DateTimeOffset Min(DateTimeOffset a, DateTimeOffset b)
    {
        return a < b ? a : b;
    }
}