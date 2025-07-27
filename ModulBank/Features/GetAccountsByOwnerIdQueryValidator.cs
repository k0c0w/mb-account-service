using FluentValidation;
using ModulBank.Validation;

namespace ModulBank.Features;

public class GetAccountsByOwnerIdQueryValidator : AbstractValidator<GetAccountsByOwnerIdQuery>
{
    public GetAccountsByOwnerIdQueryValidator()
    {
        RuleFor(x => x.OwnerId)
            .SetValidator(new GuidValidator());
    } 
}