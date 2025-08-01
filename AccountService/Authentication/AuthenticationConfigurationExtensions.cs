using System.Text;
using AccountService.Features;
using Microsoft.IdentityModel.Tokens;

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
        services.AddAuthentication("Bearer")
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = GetByPath(configuration, "Jwt:Issuer"),
                    ValidAudience = GetByPath(configuration, "Jwt:Audience"),
                    IssuerSigningKey = GetKey(configuration)
                };
            });
        services.AddAuthorization();
    }

    private static SymmetricSecurityKey GetKey(IConfiguration configuration)
    {
        var key = GetByPath(configuration, "Jwt:Key");
        
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
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