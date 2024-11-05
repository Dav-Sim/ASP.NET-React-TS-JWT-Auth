using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Example.Auth.Services;
using System.Text;

namespace Example.Auth;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        var key = Encoding.ASCII.GetBytes(configuration["AuthSettings:Jwt:Secret"]);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["accessToken"];
                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["AuthSettings:Jwt:Issuer"],
                    ValidAudience = configuration["AuthSettings:Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AppRoles.Admin, policy => policy.RequireRole(AppRoles.Admin));
            options.AddPolicy(AppRoles.User, policy => policy.RequireRole(AppRoles.User));
            options.AddPolicy(AppRoles.NotVerified, policy => policy.RequireRole(AppRoles.NotVerified));
        });

        services.AddTransient<TokenProvider>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<PasswordService>();

        return services;
    }

    public static IServiceCollection AddUserService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<UserService>();

        return services;
    }
}
