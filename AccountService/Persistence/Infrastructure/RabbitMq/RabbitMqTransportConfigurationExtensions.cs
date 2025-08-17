namespace AccountService.Persistence.Infrastructure.RabbitMq;

public static class RabbitMqTransportConfigurationExtensions
{
    public static IServiceCollection AddMasstransitOverRabbitMq(this IServiceCollection services, RabbitMqConfig appCfg)
    {
        return services;
    }
}