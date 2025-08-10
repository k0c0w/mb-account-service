using System.Reflection;

namespace AccountService.Middlewares;

public static class ServiceCollectionExtensions
{
    public static void AddMiddlewaresFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var middlewares = assembly.GetTypes()
            .Where(x => x is { IsClass: true, IsAbstract: false })
            .Where(x => x.IsAssignableTo(typeof(IMiddleware)));

        foreach (var m in middlewares)
        {
            services.AddScoped(m);
        }
    }
}