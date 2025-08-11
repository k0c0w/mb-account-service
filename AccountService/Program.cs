using System.Reflection;
using AccountService.Authentication;
using AccountService.Features;
using AccountService.Features.DataAccess;
using AccountService.Features.Domain;
using AccountService.Jobs;
using AccountService.Middlewares;
using AccountService.Persistence;
using AccountService.Persistence.Services;
using AccountService.Swagger;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var currentAssembly = Assembly.GetExecutingAssembly();
var dbConfig = builder.Configuration.GetRequiredSection("Database");

services.AddOpenApi();
services.AddSwagger(builder.Configuration);

services.AddControllers();
services.AddLogging(cfg => cfg.AddConsole());
services.AddMemoryCache();
services.AddMiddlewaresFromAssembly(currentAssembly);

services.AddSingleton<IUserValidator, UserValidator>()
        .AddSingleton<ICurrencyValidator, CurrencyValidator>()
        .AddTransient<IAccountInterestRewarder, AccountInterestRewarder>()
        .AddFeatures();

services.AddDbContextFactory<AccountServiceDbContext>(
    cfg => cfg.UseNpgsql(dbConfig.GetValue<string>("ConnectionString")));
services.AddScoped<AccountServiceDbContext>(sp =>
    sp.GetRequiredService<IDbContextFactory<AccountServiceDbContext>>().CreateDbContext());

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

// used by testing
[UsedImplicitly]
public partial class Program;