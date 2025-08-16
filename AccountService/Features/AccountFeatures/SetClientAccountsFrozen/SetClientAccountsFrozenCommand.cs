using MediatR;

namespace AccountService.Features.AccountFeatures.SetClientAccountsFrozen;

public sealed record SetClientAccountsFrozenCommand : IRequest, ITransactionalRequest
{
    public Guid OwnerId { get; }
    
    public bool Freeze { get; }

    private SetClientAccountsFrozenCommand(Guid ownerId, bool freeze)
    {
        OwnerId = ownerId;
        Freeze = freeze;
    }

    public static SetClientAccountsFrozenCommand BlockAccounts(Guid ownerId) => new(ownerId, freeze: true);
    
    public static SetClientAccountsFrozenCommand UnblockAccounts(Guid ownerId) => new(ownerId, freeze: false);
}
