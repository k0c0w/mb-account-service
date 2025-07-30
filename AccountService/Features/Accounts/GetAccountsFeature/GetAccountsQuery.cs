using MediatR;

namespace AccountService.Features;

public record GetAccountsQuery : IRequest<IEnumerable<AccountDto>>;
