using AccountService.Features.Utils;
using MediatR;

namespace AccountService.Features;

public sealed record GetAccountStatementQuery(Guid AccountId, DateTimeOffset PeriodStartUtc, DateTimeOffset PeriodEndUtc) 
    : IRequest<AccountStatementDto>, ICachedRequest
{
    public string CacheKey => $"statements:{AccountId}-{PeriodStartUtc}-{(PeriodEndUtc - PeriodStartUtc).Days}";
    
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromDays(1);
}
