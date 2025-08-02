using AccountService.Validation;
using FluentValidation;
using JetBrains.Annotations;

namespace AccountService.Features.Users.GetAccountsByOwnerIdFeature;

// Property is being used by serialization
[UsedImplicitly]
public class GetAccountsByOwnerIdQueryValidator : AbstractValidator<GetAccountsByOwnerIdQuery>
{
    public GetAccountsByOwnerIdQueryValidator()
    {
        RuleFor(x => x.OwnerId)
            .SetValidator(new GuidValidator());
    } 
}