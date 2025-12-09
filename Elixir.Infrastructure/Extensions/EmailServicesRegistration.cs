using Elixir.Application.Common.Models;
using Elixir.Application.Interfaces.Services;
using Elixir.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Elixir.Infrastructure.Extensions;

public static class EmailServicesRegistration
{
    public static IServiceCollection AddEmailInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {

        var emailSection = configuration.GetSection("EmailConfigSettings"); // Get Email settings
        services.Configure<EmailConfigSettings>(emailSection);
        //var emailSettings = emailSection.Get<EmailConfigSettings>();
        // Register Email Service
        services.AddScoped<IEmailService, EmailService>();
        return services;

    }
}
