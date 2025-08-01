using System.Reflection;
using AccountService.Authentication;
using AccountService.Domain;
using AccountService.Middlewares;
using AccountService.Persistence.DataAccess;
using AccountService.Persistence.Services;
using AccountService.PipelineBehaviours;
using AccountService.Swagger;
using AccountService.Validation;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var currentAssembly = Assembly.GetExecutingAssembly();

services.AddOpenApi();
services.AddSwagger(builder.Configuration);

services.AddControllers();
services.AddLogging(cfg => cfg.AddConsole());
services.AddMemoryCache();
services.AddAllFromAssembly(currentAssembly);

services.AddSingleton<IUserVerificator, UserVerificator>();
services.AddSingleton<ICurrencyVerificator, CurrencyVerificator>();
services.AddSingleton<IAccountRepository, AccountRepository>();
services.AddCors();
services.AddJwt(builder.Configuration);

services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(currentAssembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
});

services.AddFluentValidation(currentAssembly);

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

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