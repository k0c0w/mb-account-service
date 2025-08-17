using System.Text;
using System.Text.Json;
using AccountService.Persistence.Infrastructure.DataAccess;
using AccountService.Persistence.Services.Infrastructure.Outbox;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace AccountService.Persistence.Infrastructure.RabbitMq;

public sealed class RabbitMqOutboxProcessor(
    IChannel rabbitMqBus,
    AccountServiceDbContext dbContext, 
    ILogger<RabbitMqOutboxProcessor> logger,
    IOptions<OutboxProcessorConfig> config) : OutboxProcessor(dbContext, logger, config)
{
    protected override ValueTask PublishMessageAsync(OutboxMessage outboxMessage, CancellationToken ct = default)
    {
        if (!outboxMessage.Properties.TryGetValue(PropertiesKeys.ExchangeName, out var exchange))
        {
            throw new ArgumentException("Exchange name was not provided.");
        }

        if (!outboxMessage.Properties.TryGetValue(PropertiesKeys.RoutingKey, out var routingKey))
        {
            routingKey = string.Empty;
        }

        const bool routeToQueueDirectly = false;

        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            Headers = outboxMessage.Headers?
                .Select(kv => new KeyValuePair<string,object?>(kv.Key, ConvertHeaderValue(kv.Value)))
                .ToDictionary()
        };
        var body = Encoding.UTF8.GetBytes(outboxMessage.Payload);

        return rabbitMqBus.BasicPublishAsync(exchange, routingKey, routeToQueueDirectly, props, body, ct);
    }
    
    private static object ConvertHeaderValue(object? value)
    {
        return value switch
        {
            null => string.Empty,
            JsonElement json => json.ValueKind switch
            {
                JsonValueKind.String => json.GetString() ?? string.Empty,
                JsonValueKind.Number => json.TryGetInt64(out var n) ? n : json.GetRawText(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => json.GetRawText()
            },
            Guid guid => guid.ToString(),
            _ => value
        };
    }
}