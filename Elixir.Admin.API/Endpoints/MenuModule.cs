using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Menu.DTOs;
using Elixir.Application.Features.Menu.Queries.GetMenuItemsByRole;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Elixir.Admin.API.Endpoints;

public static class MenuModule
{
    private static ILogger _logger;
    public static void RegisterMenuEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Get the ILoggerFactory from the service provider
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        // Create a logger for state
        _logger ??= loggerFactory.CreateLogger("MenuApiRoutes");
        _logger.LogInformation("Registering menu endpoints.");

        endpoints.MapGet("api/v{version}/menus", [Authorize] async (int version, IMediator mediator, ClaimsPrincipal principal) =>
        {
            _logger.LogInformation("Received request for menu items. Version: {Version}, User: {User}", version, principal.Identity?.Name);

            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            bool IsSuperUser = String.Equals(principal.FindFirst(AppConstants.IS_SUPER_ADMIN)?.Value, AppConstants.IS_SUPER_ADMIN_TRUE);

            _logger.LogInformation("Fetching menu items for UserId: {UserId}, IsSuperUser: {IsSuperUser}", userId, IsSuperUser);

            var query = new GetMenuItemsByRoleQuery(userId, IsSuperUser);
            var result = await mediator.Send(query);

            if (result == null)
            {
                _logger.LogInformation("No menu items found for UserId: {UserId}", userId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.FILTER_NO_RECORDS_FOUND, false, String.Empty));
            }

            _logger.LogInformation("Menu items found for UserId: {UserId}. Count: {Count}", userId, result.Count);
            return Results.Json(new ApiResponse<List<MenuItemDto>>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        });
    }
}
