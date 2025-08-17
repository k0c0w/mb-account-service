namespace AccountService.Persistence.Infrastructure.RabbitMq;

// ReSharper disable once ClassNeverInstantiated.Global
// Instantiated by configuration
public sealed record RabbitMqConfig
{
    public required string Host { get; init; }
    
    public required string VirtualHost { get; init; }
    
    public required string User { get; init; }
    
    public required string Password { get; init; }
    
    public required string ExchangeName { get; init; }
}