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
            .NotEmpty()
            .Must(x => Enum.TryParse<AccountType>(x, out _))
            .WithMessage("Unsupported account type.");
        
        RuleFor(x => x.CurrencyCode)
            .NotNull()
            .NotEmpty()
            .Length(3);
    }
}