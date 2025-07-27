using MediatR;

namespace ModulBank.Features;

public sealed record TransferMoneyCommand(
    Guid SenderAccountId,
    Guid RecipientAccountId,
    decimal Amount
    ) : IRequest;
