using System.Text.Json;
using AccountService.Features.Domain.Events;
using AccountService.Persistence.Infrastructure.DataAccess;
using AccountService.Persistence.Services.Domain;
using Microsoft.Extensions.Options;

namespace AccountService.Persistence.Infrastructure.RabbitMq;

public sealed class RabbitMqDomainEventsNotifier(AccountServiceDbContext dbContext, IOptions<RabbitMqConfig> rabbitCfg) 
    : DomainEventsNotifier(dbContext)
{
    private string ExchangeName => rabbitCfg.Value.ExchangeName;

    private static OutboxMessage WithProperties<T>(EventEnvelope<T> message, Dictionary<string, string> properties)
    where T : IDomainEvent
    {
        var headers = new Dictionary<string, object>
        {
            {"X-Correlation-Id", message.Meta.CorrelationId},
            {"X-Causation-Id", message.Meta.CausationId}
        };
        
        return new OutboxMessage
        {
            Id = Guid.CreateVersion7(),
            Payload = JsonSerializer.Serialize(message),
            OccuredAtUtc = DateTimeOffset.UtcNow,
            Headers = headers,
            Properties = properties
        };
    }

    protected override OutboxMessage GetFrom(EventEnvelope<MoneyCreditedEvent> occuredEvent)
    {
        var properties = new Dictionary<string, string>
        {
            { PropertiesKeys.ExchangeName, ExchangeName },
            { PropertiesKeys.RoutingKey, "money.credited" }
        };
        
        return WithProperties(occuredEvent, properties);
    }

    protected override OutboxMessage GetFrom(EventEnvelope<MoneyDebitedEvent> occuredEvent)
    {
        var properties = new Dictionary<string, string>
        {
            { PropertiesKeys.ExchangeName, ExchangeName },
            { PropertiesKeys.RoutingKey, "money.debited" }
        };
        
        return WithProperties(occuredEvent, properties);
    }

    protected override OutboxMessage GetFrom(EventEnvelope<TransferCompletedEvent> occuredEvent)
    {
        var properties = new Dictionary<string, string>
        {
            { PropertiesKeys.ExchangeName, ExchangeName },
            { PropertiesKeys.RoutingKey, "money.transferred" }
        };
        
        return WithProperties(occuredEvent, properties);
    }

    protected override OutboxMessage GetFrom(EventEnvelope<AccountOpenedEvent> occuredEvent)
    {
        var properties = new Dictionary<string, string>
        {
            { PropertiesKeys.ExchangeName, ExchangeName },
            { PropertiesKeys.RoutingKey, "account.opened" }
        };
        
        return WithProperties(occuredEvent, properties);
    }

    protected override OutboxMessage GetFrom(EventEnvelope<InterestAccruedEvent> occuredEvent)
    {
        var properties = new Dictionary<string, string>
        {
            { PropertiesKeys.ExchangeName, ExchangeName },
            { PropertiesKeys.RoutingKey, "money.interest_occured" }
        };
        
        return WithProperties(occuredEvent, properties);
    }
}