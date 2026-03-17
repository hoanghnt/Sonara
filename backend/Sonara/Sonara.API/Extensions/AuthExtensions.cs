using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sonara.API.JwtService;
using Sonara.Application.Interfaces;

namespace Sonara.API.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddScoped(provider =>
        {
            var jwtSettings = provider.GetRequiredService<IOptions<JwtSettings>>().Value;

            jwtSettings.Secret = Environment.GetEnvironmentVariable("JWT_SECRET") ??
                                 throw new InvalidOperationException("JWT_SECRET is not configured.");

            return jwtSettings;
        });

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var secret = Environment.GetEnvironmentVariable("JWT_SECRET")
                             ?? throw new InvalidOperationException("JWT_SECRET is not configured.");

                options.RequireHttpsMetadata = false; //set true in Production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });

        services.AddScoped<ITokenService, TokenService>(provider =>
        {
            var jwtSettings = provider.GetRequiredService<JwtSettings>();
            return new TokenService(jwtSettings.Secret);
        });
        return services;
    }
}