using MediatR;

namespace AccountService.Features.Accounts.RemoveAccountFeature;

public sealed record RemoveAccountCommand(Guid AccountId) : IRequest;
