namespace AccountService.Persistence.Infrastructure.RabbitMq;

public static class PropertiesKeys
{
    public const string ExchangeName = "RMQ-Exchange";
        
    public const string RoutingKey = "RMQ-RoutingKey";
    
    public const string MessageId = "RMQ-MessageId";
    
    public const string CorrelationId = "RMQ-CorrelationId";
}