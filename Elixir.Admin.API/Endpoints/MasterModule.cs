using Elixir.Application.Common.Models;
using Elixir.Application.Features.Master.DTOs;
using Elixir.Application.Features.Master.Queries.GetPagedMaster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Elixir.Application.Common.Constants;

namespace Elixir.Admin.API.Endpoints;

public static class MasterModule
{
    private static ILogger _logger;
    public static void RegisterMasterModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Get the ILoggerFactory from the service provider
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        // Create a logger for state
        _logger ??= loggerFactory.CreateLogger("MasterApiRoutes");

        endpoints.MapGet("api/v{version}/master/{pageNumber}/{pageSize}", [Authorize] async (int version, string? searchTerm, int pageNumber, int pageSize, IMediator mediator) =>
        {
            _logger.LogInformation("Received request for paged master records. Version: {Version}, SearchTerm: {SearchTerm}, PageNumber: {PageNumber}, PageSize: {PageSize}", version, searchTerm, pageNumber, pageSize);

            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;

            var query = new GetPagedMasterQuery(searchTerm, pageNumber, pageSize);
            var result = await mediator.Send(query);

            if (result == null)
            {
                _logger.LogInformation("No master records found for the given criteria. SearchTerm: {SearchTerm}, PageNumber: {PageNumber}, PageSize: {PageSize}", searchTerm, pageNumber, pageSize);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.FILTER_NO_RECORDS_FOUND, false, String.Empty));
            }

            _logger.LogInformation("Master records fetched successfully. Count: {Count}, PageNumber: {PageNumber}, PageSize: {PageSize}", result.Data.Count, pageNumber, pageSize);
            return Results.Json(new ApiResponse<PaginatedResponse<MasterDto>>(200, AppConstants.ErrorCodes.COMMAN_MASTER_RETRIEVAL_SUCCESS, true, result));
        });
    }
}
