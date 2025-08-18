using RabbitMQ.Client;

namespace AccountService.Persistence.Infrastructure.RabbitMq;

// ReSharper disable once ClassNeverInstantiated.Global
// Instantiated by configuration
public sealed record RabbitMqConfig
{
    public required string Host { get; set; }

    public required int Port { get; set; } = 5672;
    
    public required string VirtualHost { get; set; }
    
    public required string User { get; set; }
    
    public required string Password { get; set; }
    
    public required string ExchangeName { get; set; }

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