using AccountService.Validation;
using FluentValidation;

namespace AccountService.Features;

public class GetAccountStatementQueryValidator : AbstractValidator<GetAccountStatementQuery>
{
    public GetAccountStatementQueryValidator()
    {
        RuleFor(x => x.AccountId)
            .SetValidator(new GuidValidator());
        
        RuleFor(x => x.PeriodStartUtc)
            .LessThan(x => x.PeriodEndUtc)
            .LessThan(DateTimeOffset.UtcNow);

        RuleFor(x => x.PeriodEndUtc)
            .GreaterThan(x => x.PeriodStartUtc)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow);
    }
}