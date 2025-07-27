using FluentValidation;

namespace ModulBank.Validation;

public class GuidValidator : AbstractValidator<Guid>
{
    public GuidValidator()
    {
        RuleFor(x => x)
            .NotEmpty();
    }
}