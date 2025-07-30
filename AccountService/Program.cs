using FluentValidation;
using AccountService.Persistance.DataAccess;
using AccountService.Domain;
using AccountService.Features;
using AccountService.Persistance.Services;
using AccountService.Middlewares;
using AccountService.PipelineBehaviours;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var currentAssembly = typeof(Program).Assembly;

services.AddOpenApi();
services.AddSwaggerGen();

services.AddControllers();
services.AddLogging(cfg => cfg.AddConsole());
services.AddMemoryCache();
services.AddAllFromAssembly(currentAssembly);

services.AddSingleton<IUserVerificator, UserVerificator>();
services.AddSingleton<ICurrencyVerificator, CurrencyVerificator>();
services.AddSingleton<IAccountRepository, AccountRepository>();

services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(currentAssembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
});

services.AddValidatorsFromAssembly(currentAssembly);

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<RequestLogMiddleware>();
app.UseMiddleware<GlobalExceptionFilter>();
app.UseMiddleware<ValidationExceptionFilter>();
app.UseMiddleware<DomainExceptionFilter>();

app.MapControllers();

app.Run();