using AccountService.Persistence.Infrastructure.DataAccess;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace AccountService.Tests.IntegrationTests;

// called by Xunit
[UsedImplicitly]
public class WebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("account-service")
        .WithUsername("postgresql")
        .WithPassword("postgresql")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDbContextFactory<AccountServiceDbContext>>();

            // Remove all DbContext registrations
            services.RemoveAll<AccountServiceDbContext>();

            // Add test DbContextFactory pointing to container
            services.AddDbContextFactory<AccountServiceDbContext>(opt =>
                opt.UseNpgsql(_dbContainer.GetConnectionString()));

            // Re-register DbContext from factory
            services.AddScoped<AccountServiceDbContext>(sp =>
                sp.GetRequiredService<IDbContextFactory<AccountServiceDbContext>>().CreateDbContext());
        });
    }

    public Task InitializeAsync() => _dbContainer.StartAsync();

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await base.DisposeAsync();
    }
}
