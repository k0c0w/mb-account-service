using AccountService.Jobs;
using AccountService.Persistence.Infrastructure.DataAccess;
using AccountService.Persistence.Infrastructure.RabbitMq;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using RabbitMQ.Client;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace AccountService.Tests.IntegrationTests;

// called by Xunit
[UsedImplicitly]
public class WebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("account-service")
        .WithUsername("postgresql")
        .WithPassword("postgresql")
        .Build();

    public RabbitMqContainer RabbitMqContainer { get; } = new RabbitMqBuilder()
        .WithImage("rabbitmq:4-management")
        .WithUsername("guest")
        .WithPassword("guest")
        .WithHostname("localhost")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            services.AddHangfire(_dbContainer.GetConnectionString());
            services.RemoveAll<IDbContextFactory<AccountServiceDbContext>>();
            
            // Remove all DbContext registrations
            services.RemoveAll<AccountServiceDbContext>();

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_dbContainer.GetConnectionString());
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();
            services.AddDbContextFactory<AccountServiceDbContext>(
                cfg => cfg.UseNpgsql(dataSource));
            services.AddScoped<AccountServiceDbContext>(sp =>
                sp.GetRequiredService<IDbContextFactory<AccountServiceDbContext>>().CreateDbContext());
            
            services.Configure<RabbitMqConfig>(opt =>
            {
                opt.Host = RabbitMqContainer.Hostname;
                opt.Port = RabbitMqContainer.GetMappedPublicPort(5672);
                opt.User = "guest";
                opt.Password = "guest";
                opt.VirtualHost = "/";
            });
        });
    }

    public async Task InitializeAsync()
    {
        await Task.WhenAll(RabbitMqContainer.StartAsync(), _dbContainer.StartAsync());

        await using var conn = await GetRabbitMqConnectionAsync();
        await using var chan = await conn.CreateChannelAsync();
        await chan.ExchangeDeclareAsync("account.events", ExchangeType.Topic, durable:true);
        await chan.QueueDeclareAsync("account.crm", durable: true, exclusive: false, autoDelete: false);
        await chan.QueueDeclareAsync("account.notifications", durable: true, exclusive: false, autoDelete: false);
        await chan.QueueDeclareAsync("account.antifraud", durable: true, exclusive: false, autoDelete: false);
        await chan.QueueDeclareAsync("account.audit", durable: true, exclusive: false, autoDelete: false);

       await chan.QueueBindAsync("account.crm", "account.events", "account.*");
       await chan.QueueBindAsync("account.notifications", "account.events", "money.*");
       await chan.QueueBindAsync("account.antifraud", "account.events", "client.*");
       await  chan.QueueBindAsync("account.audit", "account.events", "#");
    }

    public Task<IConnection> GetRabbitMqConnectionAsync()
    {
        var f = new ConnectionFactory
        {
            HostName = RabbitMqContainer.Hostname,
            Port = RabbitMqContainer.GetMappedPublicPort(5672),
            UserName = "guest",
            Password = "guest",
            VirtualHost = "/"
        };
       return f.CreateConnectionAsync();
    }

    public new async Task DisposeAsync()
    {
        await Task.WhenAll(RabbitMqContainer.StopAsync(), _dbContainer.StopAsync());
        await base.DisposeAsync();
    }
}
