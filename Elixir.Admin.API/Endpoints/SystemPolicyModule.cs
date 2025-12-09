using Elixir.Application.Common.Models;
using Elixir.Application.Features.SystemPolicies.Commands.UpdateSystemPolicy;
using Elixir.Application.Features.SystemPolicies.DTOs;
using Elixir.Application.Features.SystemPolicies.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Elixir.Application.Common.Constants;

namespace Elixir.Admin.API.Endpoints
{
    public static class SystemPolicyModule
    {
        private static ILogger _logger;

        public static void RegisterSystemPolicyEndpoints(this IEndpointRouteBuilder endpoints)
        {
            // Get the ILoggerFactory from the service provider
            var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();

            // Create a logger for state
            _logger ??= loggerFactory.CreateLogger("SystemPolicyApiRoutes");

            endpoints.MapPut("api/v{version}/system-policy/{systemPolicyId}", [Authorize] async (int version,int systemPolicyId,CreateUpdateSystemPolicyDto updateSystemPolicyDto,IMediator mediator) =>
            {
                _logger.LogInformation("Updating system policy with ID: {SystemPolicyId}", systemPolicyId);
                var command = new UpdateSystemPolicyCommand(systemPolicyId, updateSystemPolicyDto);                
                var result = await mediator.Send(command);
                if (!result)
                {
                    _logger.LogError("System policy update failed for ID: {SystemPolicyId}", systemPolicyId);
                    return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.SYSTEM_POLICY_UPDATE_FAILED, false, string.Empty));
                }
                _logger.LogInformation("System policy updated successfully for ID: {SystemPolicyId}", systemPolicyId);
                return Results.Json(new ApiResponse<bool>(204, AppConstants.ErrorCodes.SYSTEM_POLICY_SAVE_SUCCESS, true, true));
            });

            endpoints.MapGet("api/v{version}/system-policy/default", async (int version,IMediator mediator) =>
            {
                _logger.LogInformation("Fetching default system policy.");
                var query = new GetDefaultSystemPolicyQuery();
                var result = await mediator.Send(query);
                if (result == null)
                {
                    _logger.LogError("Default system policy not found.");
                    return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.COUNTRY_NOT_FOUND, false, string.Empty));
                }
                _logger.LogInformation("Default system policy fetched successfully.");
                return Results.Json(new ApiResponse<SystemPolicyDto>(200, AppConstants.ErrorCodes.SYSTEM_POLICY_FETCHED_SUCCESSFULLY, true, result));
            });
        }
    }
}
