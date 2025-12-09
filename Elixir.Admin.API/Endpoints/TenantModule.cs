using Elixir.Application.Common.Models;
using Elixir.Application.Features.Tenant.ProvisionTenantCommand;
using MediatR;
using Elixir.Application.Common.Constants;

namespace Elixir.Admin.API.Endpoints;

public static class TenantModule
{
    private static ILogger _logger;
    public static void RegisterTenantEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();

        // Create a logger for state
        _logger ??= loggerFactory.CreateLogger("TenantApiRoutes");

        //User Login
        endpoints.MapPost("api/v{version}/tenant/{tenantcode}/provision", async (int version, string tenantcode, IMediator mediator) =>
        {
            _logger.LogInformation($"Tenant provisioning request for tenant company: {tenantcode}");
            var command = new ProvisionTenantCommand(tenantcode);
            
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogError($"Tenant provisioning request failed for tenant company: {tenantcode}");
                return Results.Json(new ApiResponse<bool>(401, AppConstants.ErrorCodes.INVALID_CREDENTIALS, false, false));
            }
            _logger.LogInformation($"Tenant provisioning request succeeded for tenant company: {tenantcode}");
            return Results.Json(new ApiResponse<bool>(200, AppConstants.ErrorCodes.LOGIN_SUCCESSFUL, true, result));
        });
    }
}
