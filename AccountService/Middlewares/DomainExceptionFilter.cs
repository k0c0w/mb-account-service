using System.Net;
using AccountService.Features;
using AccountService.Features.Domain;

namespace AccountService.Middlewares;

public class DomainExceptionFilter : IMiddleware
{
    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        object error;
        HttpStatusCode responseStatusCode;
        try
        {
            await next(ctx);
            return;
        }
        catch (DomainException e) when (e.Type == DomainException.DomainExceptionType.ExistenceError)
        {
            responseStatusCode = HttpStatusCode.NotFound;
            error = e.Message;
        }
        catch (DomainException e) when (e.Type == DomainException.DomainExceptionType.ValidationError)
        {
            responseStatusCode = HttpStatusCode.BadRequest;
            error = e.Message;
        }
        catch (DomainException e) when (e.Type == DomainException.DomainExceptionType.ConcurrencyError)
        {
            responseStatusCode = HttpStatusCode.Conflict;
            error = e.Message;
        }

        await WriteErrorsAsJsonAsync(ctx, responseStatusCode, error);
    }

    private static Task WriteErrorsAsJsonAsync<TError>(HttpContext ctx, HttpStatusCode statusCode, TError errors)
    {
        ctx.Response.StatusCode = (int)statusCode;
        ctx.Response.ContentType = "application/json";
        
        var error = MbResultWithError<TError>.Fail(errors);
        
        return ctx.Response.WriteAsJsonAsync(error);
    }
}