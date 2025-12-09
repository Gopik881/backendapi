using Elixir.Application.Common.Models;
using Elixir.Application.Features.Module.Commands.Update;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Features.Module.Queries.GetModuleMastersAndScreens;
using Elixir.Application.Features.Module.Queries.GetModuleSubmoduleList;
using Elixir.Application.Features.Module.Queries.GetModuleView;
using Elixir.Application.Features.Module.Queries.GetPagedCompanySubModules;
using Elixir.Application.Features.Module.Queries.GetPagedModuleUsage;
using Elixir.Application.Features.Module.Queries.GetPagedSubModules;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Elixir.Application.Common.Constants;

namespace Elixir.Admin.API.Endpoints;

public static class ModuleMappingModule
{
    private static ILogger _logger;

    public static void RegisterModuleMappingEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Get the ILoggerFactory from the service provider
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();

        // Create a logger for state
        _logger ??= loggerFactory.CreateLogger("ModuleMappingApiRoutes");

        endpoints.MapGet("api/v{version}/modulemappings/{moduleId}/companies/{pageNumber}/{pageSize}", [Authorize] async (int version, int moduleId, string? searchTerm, int pageNumber, int pageSize, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching companies for moduleId: {ModuleId}, pageNumber: {PageNumber}, pageSize: {PageSize}, searchTerm: {SearchTerm}", moduleId, pageNumber, pageSize, searchTerm);

            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var query = new GetPagedCompanySubModulesQuery(moduleId, searchTerm, pageNumber, pageSize);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogInformation("No Companies found for the ModuleId: {ModuleId}", moduleId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.COMPANY_NO_COMPANY_FOUND, false, String.Empty));
            }

            _logger.LogInformation("Companies for the ModuleId: {ModuleId} retrieved successfully", moduleId);
            return Results.Json(new ApiResponse<PaginatedResponse<CompanyModuleDto>>(200, AppConstants.ErrorCodes.COMPANY_LIST_UPDATE_SUCCESS, true, result));
        });

        endpoints.MapGet("api/v{version}/modulemappings/{moduleId}/submodules/{pageNumber}/{pageSize}", [Authorize] async (int version, int moduleId, string? searchTerm, int pageNumber, int pageSize, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching submodules for moduleId: {ModuleId}, pageNumber: {PageNumber}, pageSize: {PageSize}, searchTerm: {SearchTerm}", moduleId, pageNumber, pageSize, searchTerm);

            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var query = new GetPagedSubModulesQuery(moduleId, searchTerm, pageNumber, pageSize);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogInformation("No Submodules found for the ModuleId: {ModuleId}", moduleId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.FILTER_NO_RECORDS_FOUND, false, String.Empty));
            }

            _logger.LogInformation("Submodules for the ModuleId: {ModuleId} retrieved successfully", moduleId);
            return Results.Json(new ApiResponse<PaginatedResponse<SubModuleDto>>(200, AppConstants.ErrorCodes.COMPANY_LIST_UPDATE_SUCCESS, true, result));
        });

        endpoints.MapGet("api/v{version}/modulemappings/usage/{pageNumber}/{pageSize}", [Authorize] async (int version, string? searchTerm, int pageNumber, int pageSize, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching module usage, pageNumber: {PageNumber}, pageSize: {PageSize}, searchTerm: {SearchTerm}", pageNumber, pageSize, searchTerm);

            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var query = new GetPagedModuleUsageQuery(searchTerm, pageNumber, pageSize);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogInformation("No Data found for the Module usage.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.FILTER_NO_RECORDS_FOUND, false, String.Empty));
            }

            _logger.LogInformation("Module usage data retrieved successfully");
            return Results.Json(new ApiResponse<PaginatedResponse<ModuleUsageDto>>(200, AppConstants.ErrorCodes.MODULE_MANAGEMENT_GET_MODULE_MAPPING_SUCCESS, true, result));
        });

        endpoints.MapGet("api/v{version}/modulemappings", [Authorize] async (int version, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching all modules");

            var result = await mediator.Send(new GetAllModulesQuery());
            if (result == null || !result.Any())
            {
                _logger.LogInformation("No Modules found.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.FILTER_NO_RECORDS_FOUND, false, string.Empty));
            }

            _logger.LogInformation("Modules retrieved successfully");
            return Results.Json(new ApiResponse<IEnumerable<ModuleDto>>(200, AppConstants.ErrorCodes.MODULE_MANAGEMENT_GET_MODULE_MAPPING_SUCCESS, true, result));
        });

        endpoints.MapGet("api/v{version}/modulemappings/structure", [Authorize] async (int version, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching module structure for moduleId: {ModuleId}");

            var result = await mediator.Send(new GetModuleStructureQuery());
            if (result == null || !result.Any())
            {
                _logger.LogInformation("No structure found for ModuleId: {ModuleId}");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.FILTER_NO_RECORDS_FOUND, false, string.Empty));
            }

            _logger.LogInformation("Module structure for ModuleId: {ModuleId} retrieved successfully");
            return Results.Json(new ApiResponse<List<ModuleStrucureResponseDto>>(200, AppConstants.ErrorCodes.MODULE_MANAGEMENT_GET_MODULE_MAPPING_SUCCESS, true, result));
        });

        endpoints.MapPut("api/v{version}/modulemappings/structure", [Authorize] async (int version, ModuleCreateDto updateModuleDto, IMediator mediator) =>
        {
            _logger.LogInformation("Updating module structure for module: {ModuleName}", updateModuleDto.ModuleName);

            var result = await mediator.Send(new UpdateModuleCommand(updateModuleDto));
            if (result == null)
            {
                _logger.LogInformation("Failed to update module structure for module: {ModuleName}", updateModuleDto.ModuleName);
                return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.MODULE_MANAGEMENT_UPDATE_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Module structure updated successfully for module: {ModuleName}", updateModuleDto.ModuleName);
            return Results.Json(new ApiResponse<ModuleStructureResponseV2>(200, AppConstants.ErrorCodes.COMPANY_LIST_UPDATE_SUCCESS, true, result));
        });

        endpoints.MapPut("api/v{version}/modules/update", [Authorize] async (int version, [FromBody] ModuleCreateDto updateModuleDto, IMediator mediator, IValidator<UpdateModuleCommand> validator) =>
        {
            _logger.LogInformation("Updating module with provided details.");

            var command = new UpdateModuleCommand(updateModuleDto);

            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation failed while updating module for API version {Version}.", version);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }

            var result = await mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to update module.");
                return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.MODULE_MANAGEMENT_UPDATE_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Module updated successfully.");
            return Results.Json(new ApiResponse<ModuleStructureResponseV2>(200, AppConstants.ErrorCodes.COMPANY_LIST_UPDATE_SUCCESS, true, result));
        });

        endpoints.MapGet("api/v{version}/module/view/{moduleId}", [Authorize] async (int version, int moduleId, IMediator mediator, ILogger<Program> logger) =>
        {
            logger.LogInformation("Fetching module view for moduleId: {ModuleId}", moduleId);

            var result = await mediator.Send(new GetModuleViewByModuleIdQuery(moduleId));
            if (result == null)
            {
                logger.LogInformation("No module view found for ModuleId: {ModuleId}", moduleId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.MODULE_MANAGEMENT_GET_VIEW_FAILED, false, string.Empty));
            }

            logger.LogInformation("Module view for ModuleId: {ModuleId} retrieved successfully", moduleId);
            return Results.Json(new ApiResponse<ModuleDetailsDto>(200, AppConstants.ErrorCodes.MODULE_MANAGEMENT_GET_MODULE_MAPPING_SUCCESS, true, result));
        });

        endpoints.MapPost("api/v{version}/module/submodules", [Authorize] async (int version, [FromBody] List<int> moduleIds, IMediator mediator, ILogger<Program> logger) =>
        {
            logger.LogInformation("Fetching submodules for module IDs: {ModuleIds}", moduleIds);

            var result = await mediator.Send(new GetModuleSubModuleListQuery(moduleIds));
            if (result == null || result.Count == 0)
            {
                logger.LogInformation("No submodules found for module IDs: {ModuleIds}", moduleIds);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.FILTER_NO_RECORDS_FOUND, false, string.Empty));
            }

            logger.LogInformation("Submodules retrieved successfully for module IDs: {ModuleIds}", moduleIds);
            return Results.Json(new ApiResponse<List<object>>(200, AppConstants.ErrorCodes.COMPANY_LIST_UPDATE_SUCCESS, true, result));
        });

        endpoints.MapPost("api/v{version}/module/masters-and-screens", [Authorize] async (int version, [FromBody] List<int> moduleIds, bool isMaster, IMediator mediator, ILogger<Program> logger) =>
        {
            logger.LogInformation("Fetching module masters and screens for module IDs: {ModuleIds} with IsMaster: {IsMaster}", moduleIds, isMaster);

            var result = await mediator.Send(new GetModuleMastersAndScreensQuery(moduleIds, isMaster));
            if (result == null || result.Count == 0)
            {
                logger.LogInformation("No module masters or screens found for module IDs: {ModuleIds} with IsMaster: {IsMaster}", moduleIds, isMaster);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.FILTER_NO_RECORDS_FOUND, false, string.Empty));
            }

            logger.LogInformation("Module masters and screens retrieved successfully for module IDs: {ModuleIds} with IsMaster: {IsMaster}", moduleIds, isMaster);
            return Results.Json(new ApiResponse<List<object>>(200, AppConstants.ErrorCodes.COMPANY_LIST_UPDATE_SUCCESS, true, result));
        });

    }
}
