using System.Reflection;
using FluentValidation;

namespace AccountService.Validation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services, 
        params Assembly[] validatorsAssemblies
        )
    {
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        
        services.AddValidatorsFromAssemblies(validatorsAssemblies);

        return services;
    }
}