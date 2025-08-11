using MediatR;

namespace AccountService.Features.AccountFeatures.GetAccounts;

public record GetAccountsQuery : IRequest<IEnumerable<AccountDto>>;
