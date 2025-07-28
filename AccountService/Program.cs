using FluentValidation;
using AccountService.DataAccess;
using AccountService.Features;
using AccountService.Features.Domain;
using AccountService.Features.Services;
using AccountService.Middlewares;
using AccountService.PipelineBehaviours;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var currentAssembly = typeof(Program).Assembly;

services.AddOpenApi();
services.AddSwaggerGen();

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

CreateNewAccount.RegisterHttpEndpoint(app);
RemoveAccount.RegisterHttpEndpoint(app);
GetAccounts.RegisterHttpEndpoint(app);
GetAccountsByOwnerId.RegisterHttpEndpoint(app);
ChangeAccountInterestRate.RegisterHttpEndpoint(app);
TransferMoney.RegisterHttpEndpoint(app);
RegisterExternalTransaction.RegisterHttpEndpoint(app);
GetAccountStatement.RegisterHttpEndpoint(app);

app.Run();