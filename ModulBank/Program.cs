using FluentValidation;
using ModulBank.DataAccess;
using ModulBank.Features;
using ModulBank.Features.Domain;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddOpenApi();
services.AddSwaggerGen();

services.AddSingleton<IAccountRepository, AccountRepository>();
services.AddMediatR(cfg =>
{
});

services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

CreateNewAccount.RegisterHttpEndpoint(app);
RemoveAccount.RegisterHttpEndpoint(app);
GetAccounts.RegisterHttpEndpoint(app);
ChangeAccountInterestRate.RegisterHttpEndpoint(app);
TransferMoney.RegisterHttpEndpoint(app);

app.Run();