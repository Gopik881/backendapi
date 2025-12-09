using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Notification.Commands.UpdateNotification.UpdateAllUsers;
using Elixir.Application.Features.Notification.Commands.UpdateNotification.UpdateUser;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Features.Notification.Queries.GetAllNotifications;
using Elixir.Application.Features.Notification.Queries.GetPagedNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Elixir.Admin.API.Endpoints;

public static class NotificationModule
{
    private static ILogger _logger;

    public static void RegisterNotificationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        _logger ??= loggerFactory.CreateLogger("NotificationApiRoutes");



        // Get paged notifications for a user
        endpoints.MapGet("api/v{version}/notifications/user/{userId}/paged/{pageNumber}/{pageSize}", [Authorize] async (int version, int userId, int pageNumber, int pageSize, string? searchTerm, IMediator mediator, ClaimsPrincipal principal) =>
        {
            _logger.LogInformation("Fetching paged notifications for userId: {UserId}, Page: {PageNumber}, Size: {PageSize}, SearchTerm: {SearchTerm}", userId, pageNumber, pageSize, searchTerm);
            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;

            var LoggedInuserId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            bool IsSuperUser = String.Equals(principal.FindFirst(AppConstants.IS_SUPER_ADMIN)?.Value, AppConstants.IS_SUPER_ADMIN_TRUE);

            var query = new GetPagedNotificationsQuery(userId, IsSuperUser, pageNumber, pageSize, searchTerm);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogInformation("Notifications not found for userId: {UserId} with search term '{SearchTerm}'.", userId, searchTerm);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.NOTIFICATION_NOT_FOUND, false, string.Empty));
            }

            _logger.LogInformation("Notifications fetched successfully for userId: {UserId}.", userId);
            return Results.Json(new ApiResponse<PaginatedResponse<NotificationDto>>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        });


        // Mark all notifications as read for a user
        endpoints.MapPut("api/v{version}/notifications/user/mark-all-read", [Authorize] async (int version,IMediator mediator, ClaimsPrincipal principal) =>
        {
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            _logger.LogInformation("Marking all notifications as read for userId: {UserId}", userId);
            var command = new UpdateAllUserMarkAsReadNotificationCommand(userId);
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogInformation("No notifications found to mark as read for userId: {UserId}", userId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.NOTIFICATION_NOT_FOUND, false, string.Empty));
            }

            _logger.LogInformation("All notifications marked as read for userId: {UserId}", userId);
            return Results.Json(new ApiResponse<bool>(204, AppConstants.ErrorCodes.USER_MAPPING_UPDATE_SUCCESS, true, true));
        });

        // Mark a single notification as read for a user
        endpoints.MapPut("api/v{version}/notifications/user/notification/{NotificationId}/mark-read", [Authorize] async (int version, int NotificationId, IMediator mediator) =>
        {
            _logger.LogInformation("Marking notification as read for userMappingNotificationId: {UserMappingNotificationId}", NotificationId);
            var command = new UpdateUserMarkAsReadNotificationCommand(NotificationId);
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogInformation("Notification not found for userMappingNotificationId: {UserMappingNotificationId}", NotificationId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.NOTIFICATION_NOT_FOUND, false, string.Empty));
            }

            _logger.LogInformation("Notification marked as read for userMappingNotificationId: {UserMappingNotificationId}", NotificationId);
            return Results.Json(new ApiResponse<bool>(204, AppConstants.ErrorCodes.USER_MAPPING_UPDATE_SUCCESS, true, true));
        });

        
    }
}
