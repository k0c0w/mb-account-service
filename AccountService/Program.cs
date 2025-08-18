using System.Reflection;
using AccountService.Authentication;
using AccountService.Consumers.Antifraud;
using AccountService.Features;
using AccountService.Features.Domain.Services;
using AccountService.HealthChecks;
using AccountService.Jobs;
using AccountService.Middlewares;
using AccountService.Persistence.Infrastructure.DataAccess;
using AccountService.Persistence.Infrastructure.RabbitMq;
using AccountService.Persistence.Services.Domain;
using AccountService.Persistence.Services.Infrastructure;
using AccountService.Persistence.Services.Infrastructure.Outbox;
using AccountService.Swagger;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var currentAssembly = Assembly.GetExecutingAssembly();

var dbConfig = builder.Configuration.GetRequiredSection("Database");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(dbConfig.GetValue<string>("ConnectionString"));
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();
services.AddDbContextFactory<AccountServiceDbContext>(
    cfg => cfg.UseNpgsql(dataSource));
services.AddScoped<AccountServiceDbContext>(sp =>
    sp.GetRequiredService<IDbContextFactory<AccountServiceDbContext>>().CreateDbContext());

builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection("RabbitMq"));
services.Configure<RabbitMqConfig>(builder.Configuration.GetSection("RabbitMq"));
services.AddSingleton<IConnection>(sp =>
{
    var rabbitCfg = sp.GetRequiredService<IOptions<RabbitMqConfig>>().Value;
    return rabbitCfg.CreateConnectionAsync().GetAwaiter().GetResult();
});
services.AddSingleton<IChannel>(sp => sp.GetRequiredService<IConnection>().CreateChannelAsync().GetAwaiter().GetResult());

services.AddOpenApi();
services.AddSwagger(builder.Configuration);

services.AddControllers();
services.AddLogging(cfg => cfg.AddConsole());
services.AddMemoryCache();
services.AddMiddlewaresFromAssembly(currentAssembly);

services.AddSingleton<IUserValidator, UserValidator>()
        .AddSingleton<ICurrencyValidator, CurrencyValidator>()
        .AddTransient<IAccountInterestRewarder, AccountInterestRewarder>()
        .AddScoped<IDomainEventNotifier, RabbitMqDomainEventsNotifier>()
        .AddScoped<IOutboxProcessor, RabbitMqOutboxProcessor>()
        .AddFeatures();

if (builder.Environment.EnvironmentName != "Testing")
{
    services.AddHangfire(builder.Configuration.GetConnectionString("Hangfire")??throw new ArgumentException());
}

services.AddCors();
services.AddJwt(builder.Configuration);
services.AddHealthChecks()
    .AddCheck<DatabaseHealthChecks>("Database")
    .AddCheck<OutboxHealthCheck>("Outbox")
    .AddCheck<RabbitMqHealthCheck>("RabbitMq");

services.AddHostedService<AntifraudConsumer>();

var app = builder.Build();

if (dbConfig.GetValue<bool>("MustMigrate"))
{
    InProcessMigrator.ApplyMigrations(app.Services);
}

//if (builder.Environment.EnvironmentName != "Testing")
{
    app.UseHangfireJobs();
}

app.MapOpenApi();
app.UseSwaggerAndSwaggerUi(builder.Configuration);

app.UseCors(opt =>
{
    opt.AllowAnyOrigin();
});

app.Use401ResponseFormatter();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RequestLogMiddleware>();
app.UseMiddleware<GlobalExceptionFilter>();
app.UseMiddleware<ValidationExceptionFilter>();
app.UseMiddleware<DomainExceptionFilter>();

app.MapHealthChecks();
app.MapControllers()
    .RequireAuthorization();

app.Run();

// used by testing
[UsedImplicitly]
public partial class Program;