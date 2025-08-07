using System.Net;
using AccountService.Features;
using FluentValidation;

namespace AccountService.Middlewares;

public class ValidationExceptionFilter : IMiddleware
{
    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        try
        {
            await next(ctx);
        }
        catch (ValidationException ex)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ctx.Response.ContentType = "application/json";

            var error = ex.Errors
                .GroupBy(e => e.PropertyName)
                .First();

            var errorMessage = $"{error.Key}: {error.First().ErrorMessage}";
            
            await ctx.Response.WriteAsJsonAsync(MbResultWithError<string>.Fail(errorMessage));
        }
    }
}