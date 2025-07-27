using MediatR;

namespace ModulBank.Features;

public sealed record ChangeAccountInterestRateCommand(Guid AccountId, decimal Value) : IRequest;
