using AccountService.Features.Validation;
using FluentValidation;
using JetBrains.Annotations;

namespace AccountService.Features.UserFeatures.GetAccountsByOwnerId;

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