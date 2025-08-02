using MediatR;

namespace AccountService.Features.Accounts.GetAccountsFeature;

public record GetAccountsQuery : IRequest<IEnumerable<AccountDto>>;
