using System.Diagnostics;

public class RequestLogMiddleware(ILogger<RequestLogMiddleware> logger) : IMiddleware
{
    private ILogger Logger => logger;

    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = ctx.TraceIdentifier;

        Logger.LogInformation("Incoming request [{RequestId}] {Method} {Path} started at {TimeUtc}",
            requestId,
            ctx.Request.Method,
            ctx.Request.Path,
            DateTime.UtcNow);

        await next(ctx);

        stopwatch.Stop();

        Logger.LogInformation("Completed [{RequestId}] {Method} {Path}: responded {StatusCode} in {ElapsedMs}ms",
            requestId,
            ctx.Request.Method,
            ctx.Request.Path,
            ctx.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
}