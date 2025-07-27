using MediatR;

namespace ModulBank.Features;

public sealed record GetAccountsByOwnerIdQuery(Guid OwnerId) : IRequest<IEnumerable<AccountByOwnerIdDto>>;
