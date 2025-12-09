using Elixir.Application.Features.Authentication.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Elixir.Infrastructure.Services;
using Elixir.Application.Interfaces.Services;

namespace Elixir.Infrastructure.Extensions;

public static class AuthenticationServicesRegistration
{
    public static IServiceCollection AddAuthenticationInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICryptoService,CryptoService>();
        //Bind JWT Settings
        var jwtSection = configuration.GetSection("JwtSettings"); // Get JWT settings
        services.Configure<JwtSettings>(jwtSection);
        var jwtSettings = jwtSection.Get<JwtSettings>();

        //Bind Password Reset Keys Settings
        var prkSection = configuration.GetSection("PasswordResetKeysSettings"); // Get JWT settings
        services.Configure<PasswordResetKeysSettings>(prkSection);

        // Register RSA key once for reuse
        services.AddSingleton(provider =>
        {
            var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(jwtSettings.RsaKey), out _);
            return rsa;
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var rsa = services.BuildServiceProvider().GetRequiredService<RSA>();
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings.SecretKey)),
                TokenDecryptionKey = new RsaSecurityKey(rsa),
            };
        });
        return services;
    }
}
