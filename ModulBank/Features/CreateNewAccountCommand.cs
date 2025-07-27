using MediatR;

namespace ModulBank.Features;

public record CreateNewAccountCommand(Guid OwnerId, string CurrencyCode, string AccountType, decimal? InterestRate)
    : IRequest<CreatedAccountDto>;
