using MediatR;

namespace AccountService.Features.Users.GetAccountsByOwnerIdFeature;

public sealed record GetAccountsByOwnerIdQuery(Guid OwnerId) : IRequest<IEnumerable<AccountByOwnerIdDto>>;
