using MediatR;

namespace AccountService.Features;

public sealed record GetAccountsByOwnerIdQuery(Guid OwnerId) : IRequest<IEnumerable<AccountByOwnerIdDto>>;
