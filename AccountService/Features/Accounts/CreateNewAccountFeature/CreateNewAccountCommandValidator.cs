using AccountService.Domain;
using AccountService.Validation;
using FluentValidation;
using JetBrains.Annotations;

namespace AccountService.Features.Accounts.CreateNewAccountFeature;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
public class CreateNewAccountCommandValidator : AbstractValidator<CreateNewAccountCommand>
{
    public CreateNewAccountCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .SetValidator(new GuidValidator());

        RuleFor(x => x.AccountType)
            .NotNull()
            .NotEmpty()
            .IsInEnum();
        
        RuleFor(x => x.CurrencyCode)
            .SetValidator(new CurrencyCodeValidator());

        When(x => x.AccountType != AccountType.Checking, () =>
        {
            RuleFor(x => x.InterestRate)
                .NotNull()
                .NotEmpty()
                .LessThanOrEqualTo(3m)
                .GreaterThanOrEqualTo(-3m);
        });
        
        When(x => x.AccountType == AccountType.Checking, () =>
        {
            RuleFor(x => x.InterestRate)
                .Null()
                .WithMessage("This account does not support rate of interest.");
        });
    }
}