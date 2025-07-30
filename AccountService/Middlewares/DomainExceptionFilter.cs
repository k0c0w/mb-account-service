using System.Net;
using System.Text.Json;
using AccountService.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Middlewares;

public class DomainExceptionFilter : IMiddleware
{
    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        try
        {
            await next(ctx);
        }
        catch (DomainException e) when(e.Type == DomainException.DomainExceptionType.ExistenceError)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
            ctx.Response.ContentType = "application/json";

            var details = new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found.",
                Detail = e.Message
            };
            var json = JsonSerializer.Serialize(details);

            await ctx.Response.WriteAsync(json);
        }
        catch (DomainException e) when(e.Type == DomainException.DomainExceptionType.ValidationError)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ctx.Response.ContentType = "application/json";

            var details = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid operation or arguments.",
                Detail = e.Message
            };
            var json = JsonSerializer.Serialize(details);

            await ctx.Response.WriteAsync(json);
        }
    }
}