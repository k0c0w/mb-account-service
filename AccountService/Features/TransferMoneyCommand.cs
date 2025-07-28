using MediatR;

namespace AccountService.Features;

public sealed record TransferMoneyCommand(
    Guid SenderAccountId,
    Guid RecipientAccountId,
    decimal Amount
    ) : IRequest;
