using AccountService.Features.PipelineBehaviors;
using FluentValidation;

namespace AccountService.Features;

public static class ServiceCollectionExtensions
{
    public static void AddFeatures(this IServiceCollection serviceCollection)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;
        
        serviceCollection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionalBehavior<,>));
        });

        serviceCollection.AddValidatorsFromAssembly(assembly);
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
    }
}