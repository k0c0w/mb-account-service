using System.Net;
using AccountService.Features;
using AccountService.Validation;
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

            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .Select(g => new PropertyValidationError
                {
                    Error = g.First().ErrorMessage,
                    Property = g.Key,
                })
                .ToArray();

            await ctx.Response.WriteAsJsonAsync(MbResultWithError<PropertyValidationError>.Fail(errors));
        }
    }
}