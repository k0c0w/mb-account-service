using MediatR;

namespace AccountService.Features.UserFeatures.GetAccountsByOwnerId;

public sealed record GetAccountsByOwnerIdQuery(Guid OwnerId) : IRequest<IEnumerable<AccountByOwnerIdDto>>;
