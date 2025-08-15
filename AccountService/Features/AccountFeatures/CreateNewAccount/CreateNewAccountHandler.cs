using AccountService.Features.DataAccess;
using AccountService.Features.Domain;
using AccountService.Features.Domain.Services;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Features.AccountFeatures.CreateNewAccount;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
public sealed class CreateNewAccountHandler(
    ICurrencyValidator currencyValidator,
        IUserValidator userValidator,
        AccountServiceDbContext dbContext
    )
    : IRequestHandler<CreateNewAccountCommand, CreatedAccountDto>
{
    private AccountServiceDbContext DbContext => dbContext;
    
    private DbSet<Account> Accounts => DbContext.Accounts;
    
    private ICurrencyValidator CurrencyValidator => currencyValidator;
    
    private IUserValidator UserValidator => userValidator;

    public async Task<CreatedAccountDto> Handle(CreateNewAccountCommand request, CancellationToken ct)
    {
        var currencyCode = new CurrencyCode(request.CurrencyCode);
        if (!await CurrencyValidator.IsSupportedAsync(currencyCode, ct))
        {
            throw DomainException.CreateValidationException("Unsupported currency.", 
                new ArgumentException($"Unsupported currency met: {request.CurrencyCode}."));
        }
        
        if (!Enum.IsDefined(request.AccountType))
        {
            throw DomainException.CreateValidationException("Unsupported account type.", 
                new ArgumentOutOfRangeException(nameof(request.AccountType), 
                    request.AccountType,
                    "Unsupported account type met. Forgot to add one?"));
        }

        if (!await UserValidator.UserWithIdExistsAsync(request.OwnerId, ct))
        {
            throw DomainException.CreateValidationException("User does not exist.", 
                new InvalidOperationException($"User with id {request.OwnerId} does not exist in system."));
        }

        var interestRate = request.InterestRate.HasValue && request.InterestRate.Value != 0
            ? new AccountInterestRate(request.InterestRate.Value)
            : default;
        
        var account = new Account(request.OwnerId, currencyCode, request.AccountType, interestRate);
        await Accounts.AddAsync(account, ct);

        await DbContext.SaveChangesAsync(ct);

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
            Type = account.Type,
            InterestRate = account.InterestRate?.Value ?? default(decimal?),
            CreationTimeUtc = account.CreationTimeUtc,
            ClosingTimeUtc = account.ClosingTimeUtc
        };
    }
}