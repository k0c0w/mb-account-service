using MediatR;

namespace AccountService.Features.Accounts.ChangeAccountInterestRateFeature;

public sealed record ChangeAccountInterestRateCommand(Guid AccountId, decimal Value) : IRequest;
