using Hangfire;
using Hangfire.PostgreSql;

namespace AccountService.Jobs;

public static class ServiceCollectionExtensions
{
    public static void AddHangfire(this IServiceCollection services, string connectionString)
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
    }

    public static void UseHangfireJobs(this IApplicationBuilder app)
    {
        var jobRegistry = app.ApplicationServices.GetRequiredService<IRecurringJobManagerV2>();
        jobRegistry.AddOrUpdate<DailyInterestAccrualJob>(DailyInterestAccrualJob.Name, 
            j=>j.RunAsync(CancellationToken.None), 
            Cron.Daily);
    }
}