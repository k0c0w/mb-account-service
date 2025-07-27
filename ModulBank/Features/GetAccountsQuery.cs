using MediatR;

namespace ModulBank.Features;

public record GetAccountsQuery : IRequest<IEnumerable<AccountDto>>;
