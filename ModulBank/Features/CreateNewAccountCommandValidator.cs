using FluentValidation;
using ModulBank.Features.Domain;
using ModulBank.Validation;

namespace ModulBank.Features;

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
    }
}