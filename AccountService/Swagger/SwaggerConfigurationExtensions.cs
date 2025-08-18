using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace AccountService.Swagger;

public static class SwaggerConfigurationExtensions
{
    private static readonly Dictionary<string, string> Scopes = new()
    {
        { "openid", "openid" },
        { "profile", "profile" }
    };
    
    public static void UseSwaggerAndSwaggerUi(this IApplicationBuilder app, IConfiguration configuration)
    {
        var clientId = configuration["Authentication:Audience"] ?? throw new ArgumentException();
        
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.OAuthClientId(clientId);
            options.OAuthScopes(Scopes.Values.ToArray());
        });
    }
    
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
                        Scopes = Scopes
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
            options.DocumentFilter<AddPossibleEventsDescriptionFilter>();
        });
    }
}