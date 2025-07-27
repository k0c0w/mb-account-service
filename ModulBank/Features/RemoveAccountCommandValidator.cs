using FluentValidation;

namespace ModulBank.Features;

public class RemoveAccountCommandValidator : AbstractValidator<RemoveAccountCommand>
{
    public RemoveAccountCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty();
    }
}