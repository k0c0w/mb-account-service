using MediatR;

namespace AccountService.Features.AccountFeatures.RemoveAccount;

public sealed record RemoveAccountCommand(Guid AccountId) : IRequest, ITransactionalRequest;
