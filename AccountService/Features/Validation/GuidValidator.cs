using FluentValidation;

namespace AccountService.Features.Validation;

public class GuidValidator : AbstractValidator<Guid>
{
    public GuidValidator()
    {
        RuleFor(x => x)
            .NotEmpty();
    }
}