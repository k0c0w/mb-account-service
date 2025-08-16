using AccountService.Contracts.Antifraud;
using AccountService.Features.AccountFeatures.SetClientAccountsFrozen;
using MassTransit;
using MediatR;

namespace AccountService.Consumers;

// ReSharper disable once ClassNeverInstantiated.Global
// Consumer is started by masstransit library
public sealed class AntifraudClientStateChangedEventConsumer(
    ILogger<AntifraudClientStateChangedEventConsumer> logger,
    IMediator mediator)
    : IConsumer<IClientStateChangedEvent>
{
    public async Task Consume(ConsumeContext<IClientStateChangedEvent> context)
    {
        var message = context.Message;
        var ownerId = message.ClientId;
        var routingKey = context.RoutingKey();
        
        if (routingKey == "client.blocked")
        {
            await mediator.Send(SetClientAccountsFrozenCommand.BlockAccounts(ownerId));
        }
        else if (routingKey == "client.unblocked")
        {
            await mediator.Send(SetClientAccountsFrozenCommand.UnblockAccounts(ownerId));
        }
        else
        {
            throw new NotImplementedException("Put messages into inbox_dead_letters");
        }
    }
}