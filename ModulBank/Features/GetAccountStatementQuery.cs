using MediatR;

namespace ModulBank.Features;

public sealed record GetAccountStatementQuery(Guid AccountId, DateTimeOffset PeriodStartUtc, DateTimeOffset PeriodEndUtc) 
    : IRequest<AccountStatementDto>;
