using AccountService.Validation;
using FluentValidation;

namespace AccountService.Features;

public class CreateNewAccountCommandValidator : AbstractValidator<CreateNewAccountCommand>
{
    public CreateNewAccountCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .SetValidator(new GuidValidator());

        RuleFor(x => x.AccountType)
            .NotNull()
            .NotEmpty();
        
        RuleFor(x => x.CurrencyCode)
            .SetValidator(new CurrencyCodeValidator());

        When(x => x.AccountType?.ToLower() != "checking", () =>
        {
            RuleFor(x => x.InterestRate)
                .NotNull()
                .NotEmpty()
                .LessThanOrEqualTo(3m)
                .GreaterThanOrEqualTo(-3m);
        });
    }
}