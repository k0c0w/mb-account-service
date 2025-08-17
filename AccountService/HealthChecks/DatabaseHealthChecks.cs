using AccountService.Persistence.Infrastructure.DataAccess;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AccountService.HealthChecks;

public class DatabaseHealthChecks(AccountServiceDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken ct = default)
    {
        try
        {
            if (await dbContext.Database.CanConnectAsync(ct))
            {
                return HealthCheckResult.Healthy();
            }
            
            return HealthCheckResult.Unhealthy("Database cannot be reached.");
        }
        catch
        {
            return HealthCheckResult.Unhealthy("Database check failed.");
        }
    }
}
