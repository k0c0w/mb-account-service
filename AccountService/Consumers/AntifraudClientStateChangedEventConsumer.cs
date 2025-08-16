using AccountService.Contracts.Antifraud;
using MassTransit;

namespace AccountService.Consumers;

public sealed class AntifraudClientStateChangedEventConsumer : IConsumer<IClientStateChangedEvent>
{
    public Task Consume(ConsumeContext<IClientStateChangedEvent> context)
    {
        var routingKey = context.RoutingKey();
        /*
        var work = routingKey switch
        {
            "client.blocked" => throw new NotImplementedException(),
            "client.unblocked" => throw new NotImplementedException(),
            _ => throw new NotImplementedException("Put messages into inbox_dead_letters"),
        };
        */
        throw new NotImplementedException("Put messages into inbox_dead_letters");
    }
}