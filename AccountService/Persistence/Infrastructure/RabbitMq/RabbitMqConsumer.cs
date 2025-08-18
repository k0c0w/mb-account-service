using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AccountService.Persistence.Infrastructure.RabbitMq;

public abstract class RabbitMqConsumer(
    IServiceScopeFactory scopeFactory,
    ILogger logger)
    : BackgroundService
{
    protected IServiceScopeFactory ScopeFactory => scopeFactory;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // Initialization on Hosted Service startup due to async operation
    protected IChannel Channel { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
    protected ILogger Logger => logger;

    protected abstract string QueueName { get; }

    protected abstract Task HandleMessageAsync(BasicDeliverEventArgs args);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        using var scope = ScopeFactory.CreateScope();
        var connection = scope.ServiceProvider.GetRequiredService<IConnection>();
        
        Channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(Channel);
        consumer.ReceivedAsync += OnReceivedAsync;
        
        await Channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        Logger.LogInformation("Consumer for queue {queue} started", QueueName);
    }

    private async Task OnReceivedAsync(object model, BasicDeliverEventArgs ea)
    {
        var props = ea.BasicProperties;
        Logger.LogInformation("{EventId}:{Type}:{CorrelationId} received event", props.MessageId, ea.RoutingKey, props.CorrelationId);
        
        await HandleMessageAsync(ea);
        
        Logger.LogInformation("{EventId}:{Type}:{CorrelationId} event handled", props.MessageId, ea.RoutingKey, props.CorrelationId);
    }

    public override void Dispose()
    {
        Channel.Dispose();
        base.Dispose();
    }
}