using System.Net;
using AccountService.Domain;
using AccountService.Features;

namespace AccountService.Middlewares;

public class DomainExceptionFilter : IMiddleware
{
    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        try
        {
            await next(ctx);
        }
        catch (DomainException e) when (e.Type == DomainException.DomainExceptionType.ExistenceError)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await WriteErrorsAsJsonAsync(ctx, e.Message);
        }
        catch (DomainException e) when (e.Type == DomainException.DomainExceptionType.ValidationError)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            
            await WriteErrorsAsJsonAsync(ctx, e.Message);
        }
    }

    private static Task WriteErrorsAsJsonAsync<TError>(HttpContext ctx, TError errors)
    {
        ctx.Response.ContentType = "application/json";
        var error = MbResultWithError<TError>.Fail(errors);
        return ctx.Response.WriteAsJsonAsync(error);
    }
}