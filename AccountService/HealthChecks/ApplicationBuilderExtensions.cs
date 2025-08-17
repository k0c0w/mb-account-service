using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AccountService.HealthChecks;

public static class ApplicationBuilderExtensions
{
    public static void MapHealthChecks(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = (context, _) =>
            {
                context.Response.StatusCode = 200;
                context.Response.Headers.ContentType = "text/plain";
                return context.Response.WriteAsync("Healthy");
            },
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        })
        .AllowAnonymous()
        .WithName("Liveness")
        .WithTags("Health")
        .WithMetadata(new ApiExplorerSettingsAttribute { GroupName = "Health" });
        
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                },
            })
            .AllowAnonymous()
            .WithName("Readiness")
            .WithTags("Health")
            .WithMetadata(new ApiExplorerSettingsAttribute { GroupName = "Health" });
    }
}