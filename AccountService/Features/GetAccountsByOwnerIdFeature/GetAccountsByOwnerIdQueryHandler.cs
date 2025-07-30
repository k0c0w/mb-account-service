using AccountService.Features.Domain;
using MediatR;

namespace AccountService.Features;

public sealed class GetAccountsByOwnerIdQueryHandler(
    IUserVerificator userVerificator,
    IAccountRepository accountRepository
    )
    : IRequestHandler<GetAccountsByOwnerIdQuery, IEnumerable<AccountByOwnerIdDto>>
{
    private IUserVerificator UserVerificator => userVerificator;
    
    private IAccountRepository AccountRepository => accountRepository;

    public async Task<IEnumerable<AccountByOwnerIdDto>> Handle(GetAccountsByOwnerIdQuery request, CancellationToken ct)
    {
        if (!await UserVerificator.UserWithIdExsitsAsync(request.OwnerId, ct))
        {
            throw DomainException.CreateValidationException("User does not exist.", 
                new InvalidOperationException($"User with id {request.OwnerId} does not exist in system."));
        }
        
        var filter = new IAccountRepository.FindAccountsFilter.ByOwnerIdFilter(request.OwnerId);
        var accounts = await AccountRepository.FindAsync(filter, ct);

        return accounts
            .Select(FromDomainToDto)
            .ToArray();
    }

    private static AccountByOwnerIdDto FromDomainToDto(Account account)
        => new()
        {
            Id = account.Id,
            CurrencyCode = account.Balance.Code.ToString()
        };
}