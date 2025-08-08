using AccountService.Domain;
using AccountService.Persistence.DataAccess;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Features.Accounts.GetAccountsFeature;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
public sealed class GetAccountsQueryHandler(AccountServiceDbContext dbContext)
    : IRequestHandler<GetAccountsQuery, IEnumerable<AccountDto>>
{
    private DbSet<Account> AccountRepository => dbContext.Accounts;
    
    public async Task<IEnumerable<AccountDto>> Handle(GetAccountsQuery request, CancellationToken ct)
    {
        return await AccountRepository
            .Select(a => FromDomainToDto(a))
            .ToArrayAsync(ct);
    }

    private static AccountDto FromDomainToDto(Account account)
    {
        return new AccountDto
        {
            Id = account.Id,
            OwnerId = account.OwnerId,
            Currency = account.Balance.Code.ToString(),
            Balance = account.Balance.Amount,
            Type = account.Type,
            InterestRate = account.InterestRate?.Value ?? default(decimal?),
            CreationTimeUtc = account.CreationTimeUtc,
            ClosingTimeUtc = account.ClosingTimeUtc
        };
    }
}