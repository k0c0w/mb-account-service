using MediatR;

namespace AccountService.Features;

public record CreateNewAccountCommand(Guid OwnerId, string CurrencyCode, string AccountType, decimal? InterestRate)
    : IRequest<CreatedAccountDto>;
