using MediatR;

namespace AccountService.Features;

public sealed record RegisterExternalTransactionCommand(
    Guid AccountId,
    string CurrencyCode,
    decimal Amount,
    string TransactionType
    )
    : IRequest;
