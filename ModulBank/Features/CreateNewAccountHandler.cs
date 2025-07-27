using MediatR;
using ModulBank.Features.Domain;

namespace ModulBank.Features;

public sealed class CreateNewAccountHandler(
    ICurrencyVerificator currencyVerificator,
        IUserVerificator userVerificator,
        IAccountRepository accountRepository
    )
    : IRequestHandler<CreateNewAccountCommand, CreatedAccountDto>
{
    private IAccountRepository AccountRepository => accountRepository;
    
    private ICurrencyVerificator CurrencyVerificator => currencyVerificator;
    
    private IUserVerificator UserVerificator => userVerificator;

    public async Task<CreatedAccountDto> Handle(CreateNewAccountCommand request, CancellationToken ct)
    {
        var currencyCode = new CurrencyCode(request.CurrencyCode);
        if (!await CurrencyVerificator.IsSupportedAsync(currencyCode, ct))
        {
            throw DomainException.CreateValidationException("Unsupported currency.", 
                new ArgumentException($"Unsupported currency met: {request.CurrencyCode}."));
        }
        
        if (!Enum.TryParse<AccountType>(request.AccountType, ignoreCase: true, out var accountType))
        {
            throw DomainException.CreateValidationException("Unsupported account type.", 
                new ArgumentOutOfRangeException(nameof(request.AccountType), 
                    $"Unsupported account type met. Forgot to add one?", request.AccountType));
        }

        if (!await UserVerificator.UserWithIdExsitsAsync(request.OwnerId, ct))
        {
            throw DomainException.CreateValidationException("User does not exist.", 
                new InvalidOperationException($"User with id {request.OwnerId} does not exist in system."));
        }

        var interestRate = request.InterestRate.HasValue
            ? new AccountInterestRate(request.InterestRate.Value)
            : default;
        
        var account = new Account(request.OwnerId, currencyCode, accountType, interestRate);

        await accountRepository.AddAsync(account, ct);

        return DomainToDto(account);
    }

    private static CreatedAccountDto DomainToDto(Account account)
    {
        return new CreatedAccountDto
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