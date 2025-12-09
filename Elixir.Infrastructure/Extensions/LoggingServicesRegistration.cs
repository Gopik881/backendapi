using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Configuration;


namespace Elixir.Infrastructure.Extensions;

public static class LoggingServicesRegistration
{
    public static IServiceCollection AddLoggingInfrastructureServices(this IServiceCollection services, ILoggingBuilder loggingBuilder, IConfiguration configuration)
    {
        // Retrieve Connection String from Configuration
        var connectionString = configuration.GetSection("ApplicationInsights:ConnectionString").Value;
        // Setup OpenTelemetry for Application Insights
        services.AddOpenTelemetry()
        .WithTracing(tracing =>
        {
            tracing
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ElixirHR"))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();

            if (!string.IsNullOrEmpty(connectionString))
            {
                tracing.AddAzureMonitorTraceExporter(options =>
                {
                    options.ConnectionString = connectionString;
                });
                Console.WriteLine("Application Insights Enabled!");
            }
            else
            {
                Console.WriteLine("Application Insights connection string missing. Falling back to Console logging.");
            }
        });
        // Setup Logging Providers
        loggingBuilder.ClearProviders(); // Remove default logging providers

        if (!string.IsNullOrEmpty(connectionString))
        {
            loggingBuilder.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyApp"));
            });
        }
        else
        {
            loggingBuilder.AddConsole();
            loggingBuilder.AddDebug();
        }

        return services;
    }
}
