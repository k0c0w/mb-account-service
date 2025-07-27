using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ModulBank.Middlewares;

public class GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    : IMiddleware
{
    private ILogger Logger => logger;
    
    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Unhandled exception: {Message}.", e.Message);

            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "If error persists, contact developers."
            };
            var json = JsonSerializer.Serialize(details);

            await ctx.Response.WriteAsync(json);
        }
    }
}