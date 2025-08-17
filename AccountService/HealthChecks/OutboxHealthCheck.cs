using AccountService.Persistence.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AccountService.HealthChecks;

public sealed class OutboxHealthCheck(AccountServiceDbContext dbContext) : IHealthCheck
{ 
    private const int WarningThreshold = 100;
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {        
            var unprocessedCount = await dbContext.OutboxMessages
                .Where(x => x.ProcessedAtUtc == null)
                .CountAsync(ct);

            return unprocessedCount > WarningThreshold 
                ? HealthCheckResult.Degraded($"Outbox pending messages: {unprocessedCount}.") 
                : HealthCheckResult.Healthy();
        }
        catch
        {
            return HealthCheckResult.Unhealthy();
        }
    }
}