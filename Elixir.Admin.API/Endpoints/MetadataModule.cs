using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.UserRightsMetatadata.DTOs;
using Elixir.Application.Features.UserRightsMetatadata.Queries.GetUserRightsMetadata;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using Elixir.Application.Common.Constants;

namespace Elixir.Admin.API.Endpoints;

public static class MetadataModule
{
    private static ILogger _logger;
    public static void RegisterMetadataEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        // Create a logger for state
        _logger ??= loggerFactory.CreateLogger("MetadataApiRoutes");

        endpoints.MapGet("/api/v{version}/metadata/user-rights/{userType}", [Authorize] async (int version, int userType, IMediator mediator) =>
        {
            var query = new GetUserRightsMetadataByUserTypeQuery(userType);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogInformation("No Metadata found for the user type.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.FILTER_NO_RECORDS_FOUND, false, string.Empty));
            }
            _logger.LogInformation("Metadata found for the user type");
            return Results.Json(new ApiResponse<UserRightsMetedataResponseDto>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        });

    }
}
