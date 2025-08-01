using FluentValidation;

namespace AccountService.Validation;

public class CurrencyCodeValidator : AbstractValidator<string>
{
    public CurrencyCodeValidator()
    {
        RuleFor(x => x)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .Length(3);
    }
}