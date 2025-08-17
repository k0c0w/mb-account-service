using System.Reflection;
using AccountService.Authentication;
using AccountService.Features;
using AccountService.Features.Domain.Services;
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

var rabbitCfg = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqConfig?>() ?? throw new ArgumentException();
builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection("RabbitMq"));

await using var rabbitConnection = await rabbitCfg.CreateConnectionAsync();
await using var writeChannel = await rabbitConnection.CreateChannelAsync();
await using var readChannel = await rabbitConnection.CreateChannelAsync();

services.AddSingleton(rabbitConnection);
services.AddKeyedSingleton("write", writeChannel);
services.AddKeyedSingleton("read", readChannel);

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

var app = builder.Build();

if (dbConfig.GetValue<bool>("MustMigrate"))
{
    InProcessMigrator.ApplyMigrations(app.Services);
}

if (builder.Environment.EnvironmentName != "Testing")
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

app.MapControllers()
    .RequireAuthorization();

app.Run();

await rabbitConnection.CloseAsync();

// used by testing
[UsedImplicitly]
public partial class Program;