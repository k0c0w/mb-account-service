using AccountService.Validation;
using FluentValidation;
using JetBrains.Annotations;

namespace AccountService.Features.AccountStatements.GetAccountStatementFeature;

// Resharper disable once. Class is being called via reflection.
[UsedImplicitly]
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