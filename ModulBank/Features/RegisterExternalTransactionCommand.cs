using MediatR;

namespace ModulBank.Features;

public sealed record RegisterExternalTransactionCommand(
    Guid AccountId,
    string CurrencyCode,
    decimal Amount,
    string TransactionType
    )
    : IRequest;
