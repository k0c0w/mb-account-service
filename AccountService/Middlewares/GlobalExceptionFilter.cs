using System.Net;
using AccountService.Features;

namespace AccountService.Middlewares;

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
            
            var errorResult = MbResultWithError<string>.Fail("Internal Server Error.");
            await ctx.Response.WriteAsJsonAsync(errorResult);
        }
    }
}