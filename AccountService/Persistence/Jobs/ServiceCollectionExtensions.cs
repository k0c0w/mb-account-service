using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AccountService.Persistence.Jobs;

public static class ServiceCollectionExtensions
{
    public static void AddJobs(this IServiceCollection services, string connectionString)
    {
        services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(cfg =>
                {
                    cfg.UseNpgsqlConnection(connectionString);
                });
        });

        services.AddHangfireServer();
        
        RecurringJob.AddOrUpdate<DailyInterestAccrualJob>(
            DailyInterestAccrualJob.Name,
            j => j.RunAsync(CancellationToken.None),
            Cron.Daily
        );
    }
}