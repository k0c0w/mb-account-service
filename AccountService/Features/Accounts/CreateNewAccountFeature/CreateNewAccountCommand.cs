using AccountService.Domain;
using JetBrains.Annotations;
using MediatR;

namespace AccountService.Features.Accounts.CreateNewAccountFeature;

public sealed record CreateNewAccountCommand : IRequest<CreatedAccountDto>, ITransactionalRequest
{
    /// <summary>
    /// An account owner identity
    /// </summary>
    // Property is being used by serialization.
    [UsedImplicitly]
    public required Guid OwnerId { get; init; } 
    
    /// <summary>
    /// An account currency
    /// </summary>
    // Property is being used by serialization.
    [UsedImplicitly]
    public required string CurrencyCode { get; init; }
    
    /// <summary>
    /// An account type 
    /// </summary>
    /// <see cref="AccountType"/>
    // Property is being used by serialization.
    [UsedImplicitly]
    public required AccountType AccountType { get; init; }
    
    /// <summary>
    /// An account rate of interest 
    /// </summary>
    /// <remarks>
    /// Not all account types support rate of interest
    /// </remarks>
    /// <see cref="AccountType"/>
    // Property is being used by serialization.
    [UsedImplicitly]
    public decimal? InterestRate { get; init; }
}
