using AccountService.Features.Domain;
using AccountService.Validation;
using FluentValidation;

namespace AccountService.Features;

public class RegisterExternalTransactionCommandValidator : AbstractValidator<RegisterExternalTransactionCommand>
{
    public RegisterExternalTransactionCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .SetValidator(new GuidValidator());
        RuleFor(x => x.CurrencyCode)
            .SetValidator(new CurrencyCodeValidator());
        RuleFor(x => x.Amount)
            .GreaterThan(decimal.Zero);
        RuleFor(x => x.TransactionType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(x => Enum.TryParse<TransactionType>(x, out _))
            .WithMessage("Unsupported Transaction type.");
    }
}