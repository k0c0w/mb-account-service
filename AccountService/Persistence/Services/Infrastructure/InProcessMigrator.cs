using AccountService.Persistence.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Persistence.Services.Infrastructure;

public static class InProcessMigrator
{
    public static void ApplyMigrations(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AccountServiceDbContext>();
        
        dbContext.Database.Migrate();
    }
}