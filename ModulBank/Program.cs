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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

CreateNewAccount.RegisterHttpEndpoint(app);

app.Run();