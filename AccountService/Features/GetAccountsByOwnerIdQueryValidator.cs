using AccountService.Validation;
using FluentValidation;

namespace AccountService.Features;

public class GetAccountsByOwnerIdQueryValidator : AbstractValidator<GetAccountsByOwnerIdQuery>
{
    public GetAccountsByOwnerIdQueryValidator()
    {
        RuleFor(x => x.OwnerId)
            .SetValidator(new GuidValidator());
    } 
}