using FluentValidation;
using JetBrains.Annotations;

namespace AccountService.Features.Accounts.RemoveAccountFeature;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
public class RemoveAccountCommandValidator : AbstractValidator<RemoveAccountCommand>
{
    public RemoveAccountCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty();
    }
}