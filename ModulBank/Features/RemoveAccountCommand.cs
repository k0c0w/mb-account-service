using MediatR;

namespace ModulBank.Features;

public sealed record RemoveAccountCommand(Guid AccountId) : IRequest;
