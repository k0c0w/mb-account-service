using AccountService.Validation;
using FluentValidation;
using JetBrains.Annotations;

namespace AccountService.Features.Transfers.TransferMoneyFeature;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
public class TransferMoneyCommandValidator : AbstractValidator<TransferMoneyCommand>
{
    public TransferMoneyCommandValidator()
    {
        RuleFor(x => x.SenderAccountId)
            .SetValidator(new GuidValidator());
        RuleFor(x => x.RecipientAccountId)
            .SetValidator(new GuidValidator());
        RuleFor(x => x.Amount)
            .NotEmpty()
            .GreaterThan(0m);
    }   
}