using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace AccountService.Swagger;

public static class ServiceCollectionExtensions
{
    public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        { 
            options.CustomSchemaIds(id => id.FullName?.Replace('+', '-'));
            options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(configuration["Keycloak:AuthorizationUrl"] ?? throw new ArgumentException()),
                        Scopes = new Dictionary<string, string>
                        {
                            {"openid", "openid"},
                            {"profile", "profile"}
                        }
                    }
                }
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id="Keycloak",
                            Type = ReferenceType.SecurityScheme
                        },
                        In = ParameterLocation.Header,
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        Scheme = JwtBearerDefaults.AuthenticationScheme
                    },
                    []
                }
            });
            
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), includeControllerXmlComments: true);
            options.OperationFilter<AddDefaultResponsesFilter>();
        });
    }
}