using FluentValidation;

namespace AccountService.Validation;

public class GuidValidator : AbstractValidator<Guid>
{
    public GuidValidator()
    {
        RuleFor(x => x)
            .NotEmpty();
    }
}