using RabbitMQ.Client;

namespace AccountService.Persistence.Infrastructure.RabbitMq;

// ReSharper disable once ClassNeverInstantiated.Global
// Instantiated by configuration
public sealed record RabbitMqConfig
{
    public required string Host { get; init; }

    public required int Port { get; init; } = 5672;
    
    public required string VirtualHost { get; init; }
    
    public required string User { get; init; }
    
    public required string Password { get; init; }
    
    public required string ExchangeName { get; init; }

    public Task<IConnection> CreateConnectionAsync()
    {
        var factory = new ConnectionFactory
        {
            Port = Port,
            HostName = Host,
            UserName = User,
            Password = Password,
            VirtualHost = VirtualHost,
            ClientProvidedName = "account-service"
        };

        return factory.CreateConnectionAsync();
    }
}