using AccountService.Consumers;
using AccountService.Features.DataAccess;
using AccountService.Features.Domain.Events;
using MassTransit;
using RabbitMQ.Client;

namespace AccountService.Persistence.RabbitMq;

public static class RabbitMqTransportConfigurationExtensions
{
    public static IServiceCollection AddMasstransitOverRabbitMq(this IServiceCollection services, RabbitMqConfig appCfg)
    {
        services.AddMassTransit(b =>
        {
            b.AddConsumer<AntifraudClientStateChangedEventConsumer>();
            
            b.AddEntityFrameworkOutbox<AccountServiceDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });

            b.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(appCfg.Host, appCfg.VirtualHost, h =>
                {
                    h.Username(appCfg.User);
                    h.Password(appCfg.Password);
                });
                
                cfg.ReceiveEndpoint("account.antifraud", x =>
                {
                    x.ConfigureConsumeTopology = false;
                    
                    x.ConfigureConsumer<AntifraudClientStateChangedEventConsumer>(ctx);
                });

                cfg.Message<AccountOpenedEvent>(x => x.SetEntityName(appCfg.ExchangeName));
                cfg.Publish<AccountOpenedEvent>(x =>
                {
                    x.Exclude = true;
                    x.ExchangeType = ExchangeType.Topic;
                });
                
                cfg.Message<MoneyCreditedEvent>(x => x.SetEntityName(appCfg.ExchangeName));
                cfg.Publish<MoneyCreditedEvent>(x =>
                {
                    x.Exclude = true;
                    x.ExchangeType = ExchangeType.Topic;
                });
                
                cfg.Message<MoneyDebitedEvent>(x => x.SetEntityName(appCfg.ExchangeName));
                cfg.Publish<MoneyDebitedEvent>(x =>
                {
                    x.Exclude = true;
                    x.ExchangeType = ExchangeType.Topic;
                });
                
                cfg.Message<TransferCompletedEvent>(x => x.SetEntityName(appCfg.ExchangeName));
                cfg.Publish<TransferCompletedEvent>(x =>
                {
                    x.Exclude = true;
                    x.ExchangeType = ExchangeType.Topic;
                });
                
                cfg.Message<InterestAccruedEvent>(x => x.SetEntityName(appCfg.ExchangeName));
                cfg.Publish<InterestAccruedEvent>(x =>
                {
                    x.Exclude = true;
                    x.ExchangeType = ExchangeType.Topic;
                });
            });
        });

        return services;
    }
}