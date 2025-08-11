using MediatR;

namespace AccountService.Features.AccountFeatures.ChangeAccountInterestRate;

public sealed record ChangeAccountInterestRateCommand(Guid AccountId, decimal Value) : IRequest, ITransactionalRequest;
