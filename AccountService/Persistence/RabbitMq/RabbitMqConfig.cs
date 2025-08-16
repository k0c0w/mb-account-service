namespace AccountService.Persistence.RabbitMq;

public sealed record RabbitMqConfig
{
    public string Host { get; init; }
    
    public string VirtualHost { get; init; }
    
    public string User { get; init; }
    
    public string Password { get; init; }
    
    public string ExchangeName { get; init; }
}