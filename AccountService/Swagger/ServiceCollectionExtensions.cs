using System.Reflection;

namespace AccountService.Swagger;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        { 
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), includeControllerXmlComments: true);
            options.OperationFilter<AddDefaultResponsesFilter>();
        });
        services.AddEndpointsApiExplorer();

        return services;
    }
}