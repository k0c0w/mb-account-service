using MediatR;

namespace AccountService.Features;

public sealed record ChangeAccountInterestRateCommand(Guid AccountId, decimal Value) : IRequest;
