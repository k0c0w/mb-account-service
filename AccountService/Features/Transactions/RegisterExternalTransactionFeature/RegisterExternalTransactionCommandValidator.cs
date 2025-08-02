using AccountService.Validation;
using FluentValidation;
using JetBrains.Annotations;

namespace AccountService.Features.Transactions.RegisterExternalTransactionFeature;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
public class RegisterExternalTransactionCommandValidator : AbstractValidator<RegisterExternalTransactionCommand>
{
    public RegisterExternalTransactionCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .SetValidator(new GuidValidator());
        RuleFor(x => x.CurrencyCode)
            .SetValidator(new CurrencyCodeValidator());
        RuleFor(x => x.Amount)
            .GreaterThan(decimal.Zero);
        RuleFor(x => x.TransactionType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .IsInEnum()
            .WithMessage("Unsupported Transaction type.");
    }
}