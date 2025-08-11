using AccountService.Features.Validation;
using FluentValidation;
using JetBrains.Annotations;

namespace AccountService.Features.AccountFeatures.ChangeAccountInterestRate;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
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