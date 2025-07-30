using AccountService.Validation;
using FluentValidation;

namespace AccountService.Features;

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