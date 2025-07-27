using FluentValidation;
using ModulBank.Validation;

namespace ModulBank.Features;

public class ChangeAccountInterestRateCommandValidator : AbstractValidator<ChangeAccountInterestRateCommand>
{
    public ChangeAccountInterestRateCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .SetValidator(new GuidValidator());
        
        RuleFor(x => x.Value)
            .NotEmpty()
            .GreaterThan(-3m)
            .LessThan(3m);
    }
}