using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace AccountService.HealthChecks;

public class RabbitMqHealthCheck(IConnection connection) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        return Task.FromResult(connection.IsOpen 
            ? HealthCheckResult.Healthy() 
            : HealthCheckResult.Unhealthy("Connection is closed."));
    }
}