using MediatR;

namespace AccountService.Features;

public sealed record RemoveAccountCommand(Guid AccountId) : IRequest;
