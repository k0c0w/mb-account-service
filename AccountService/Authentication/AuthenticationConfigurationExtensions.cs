using AccountService.Features;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AccountService.Authentication;

public static class AuthenticationConfigurationExtensions
{
    public static IApplicationBuilder Use401ResponseFormatter(this IApplicationBuilder app)
    {
        const string message = "Unauthorized access. Please provide a valid token.";
        app.Use(async (context, next) =>
        {
            await next();
            if (context.Response.StatusCode == 401)
            {
                context.Response.ContentType = "application/json";
                var error = MbResultWithError<string>.Fail(message);
                await context.Response.WriteAsJsonAsync(error);
            }
        });
        
        return app;
    }
    
    public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = configuration.GetSection("Authentication")
                    .GetValue<bool>("RequireHttpsMetadata");
                options.Audience = GetByPath(configuration, "Authentication:Audience");
                options.MetadataAddress = GetByPath(configuration, "Authentication:MetadataAddress");
                options.TokenValidationParameters = new()
                {
                    ValidIssuer = GetByPath(configuration, "Authentication:Issuer"),
                };
            });
        services.AddAuthorization();
    }

    private static string GetByPath(IConfiguration configuration, string path)
    {
        var value = configuration[path];
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException(path);
        }

        return value;
    }
}