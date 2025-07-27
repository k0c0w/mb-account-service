using FluentValidation;
using ModulBank.DataAccess;
using ModulBank.Features;
using ModulBank.Features.Domain;
using ModulBank.Features.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var currentAssembly = typeof(Program).Assembly;

services.AddOpenApi();
services.AddSwaggerGen();

services.AddSingleton<IUserVerificator, UserVerificator>();
services.AddSingleton<ICurrencyVerificator, CurrencyVerificator>();
services.AddSingleton<IAccountRepository, AccountRepository>();

services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(currentAssembly);
});

services.AddValidatorsFromAssembly(currentAssembly);

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

CreateNewAccount.RegisterHttpEndpoint(app);
RemoveAccount.RegisterHttpEndpoint(app);
GetAccounts.RegisterHttpEndpoint(app);
GetAccountsByOwnerId.RegisterHttpEndpoint(app);
ChangeAccountInterestRate.RegisterHttpEndpoint(app);
TransferMoney.RegisterHttpEndpoint(app);
RegisterExternalTransaction.RegisterHttpEndpoint(app);
GetAccountStatement.RegisterHttpEndpoint(app);

app.Run();