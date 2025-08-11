using MediatR;

namespace AccountService.Features.AccountStatementFeatures.GetAccountStatement;

public sealed record GetAccountStatementQuery(Guid AccountId, DateTimeOffset PeriodStartUtc, DateTimeOffset PeriodEndUtc) 
    : IRequest<AccountStatementDto>, ICachedRequest
{
    public string CacheKey => $"statements:{AccountId}-{PeriodStartUtc}-{(PeriodEndUtc - PeriodStartUtc).Days}";
    
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromDays(1);
}
