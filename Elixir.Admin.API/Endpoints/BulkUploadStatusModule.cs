using Elixir.Application.Common.DTOs;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.BulkUploadStatus.GetBulkUploadStatus.Command.DeleteBulkUploadStatus;
using Elixir.Application.Features.BulkUploadStatus.GetBulkUploadStatus.Queries.GetBulkUploadStatusQuery;
using Elixir.Application.Features.BulkUploadStatus.GetBulkUploadStatus.Queries.GetPagedBulkUploadStatusQuery;
using Elixir.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Elixir.Application.Common.Constants;

namespace Elixir.Admin.API.Endpoints;

public static class BulkUploadStatusModule
{
    private static ILogger _logger;
    public static void RegisterBulkUploadStatusEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        _logger ??= loggerFactory.CreateLogger("BulkUploadStatusApiRoutes");

        endpoints.MapGet("api/v{version}/bulk-upload-status/{pageNumber}/{pageSize}", [Authorize] async (int version, Guid ProcessId, int pageNumber, int pageSize, IMediator mediator) =>
        {
            _logger.LogInformation("Received request to get bulk-upload-status.");
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var query = new GetPagedBulkUploadStatusQuery(ProcessId, pageNumber, pageSize);
            var result = await mediator.Send(query);

            _logger.LogInformation("Bulk upload errors fetched successfully.");
            return Results.Json(new ApiResponse<PaginatedResponse<BulkUploadErrorListDto>>(200, AppConstants.ErrorCodes.COMMON_MASTER_LIST_FILE_UPLOAD_SUCCESS, true, result));
        });

        endpoints.MapGet("api/v{version}/bulk-upload-status/download/errorlist.xlsx", [Authorize] async (int version, Guid ProcessId, IFileHandlingService fileHandlingService, IMediator mediator) =>
        {
            _logger.LogInformation("Received request to get bulk-upload-status error list.");

            var query = new GetBulkUploadStatusQuery(ProcessId);
            var result = await mediator.Send(query);
            var downloadFile = await fileHandlingService.GetBulkUploadStatusDownloadAsync(result);
            _logger.LogInformation("Bulk upload error list fetched successfully.");
            // No error code constant for file download, keeping as is
            return Results.File(downloadFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "errorlist.xlsx");
        });

        endpoints.MapDelete("api/v{version}/bulk-upload-status", [Authorize] async (int version, Guid ProcessId, IMediator mediator) =>
        {
            _logger.LogInformation("Delete bulk-upload-status error list.");

            var query = new DeleteBulkUploadStatusCommand(ProcessId);
            var result = await mediator.Send(query);

            _logger.LogInformation("Deleted Bulk upload errors successfully.");
            return Results.Json(new ApiResponse<bool>(200, AppConstants.ErrorCodes.COMMON_MASTER_LIST_DELETE_SUCCESS, true, result));
        });
    }
}