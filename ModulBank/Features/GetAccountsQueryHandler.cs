using MediatR;
using ModulBank.Features.Domain;

namespace ModulBank.Features;

public sealed class GetAccountsQueryHandler(IAccountRepository accountRepository)
    : IRequestHandler<GetAccountsQuery, IEnumerable<AccountDto>>
{
    private IAccountRepository AccountRepository => accountRepository;
    
    public async Task<IEnumerable<AccountDto>> Handle(GetAccountsQuery request, CancellationToken ct)
    {
        var filter = new IAccountRepository.FindAccountsFilter.EmptyFilter();
        var accounts = await AccountRepository.FindAsync(filter, ct);

        return accounts
            .Select(FromDomainToDto)
            .ToArray();
    }

    private static AccountDto FromDomainToDto(Account account)
    {
        return new AccountDto
        {
            Id = account.Id,
            OwnerId = account.OwnerId,
            Currency = account.Balance.Code.ToString(),
            Balance = account.Balance.Amount,
            Type = account.Type.ToString(),
            InterestRate = account.InterestRate?.Value ?? default(decimal?),
            CreationTimeUtc = account.CreationTimeUtc,
            ClosingTimeUtc = account.ClosingTimeUtc,
        };
    }
}