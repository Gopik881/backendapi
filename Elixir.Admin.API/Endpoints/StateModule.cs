using Elixir.Application.Common.Constants;
using Elixir.Application.Common.DTOs;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Features.Country.Queries.GetAllCountries;
using Elixir.Application.Features.State.Commands.BulkInsertStates;
using Elixir.Application.Features.State.Commands.CreateState;
using Elixir.Application.Features.State.Commands.DeleteState;
using Elixir.Application.Features.State.Commands.UpdateState;
using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Features.State.Queries.GetAllStates;
using Elixir.Application.Features.State.Queries.GetPagedStatesWithFilters;
using Elixir.Application.Features.State.Queries.GetStatesbyCountryId;
using Elixir.Application.Features.State.Queries.GetValidateDuplicateStates;
using Elixir.Application.Interfaces.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;


namespace Elixir.Admin.API.Endpoints;
public static class StateModule
{
    private static ILogger _logger;

    public static void RegisterStateEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Get the ILoggerFactory from the service provider
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();

        // Create a logger for state
        _logger ??= loggerFactory.CreateLogger("StateApiRoutes");

        endpoints.MapGet("api/v{version}/states", [Authorize] async (int version, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching all states for API version {Version}.", version);
            var query = new GetAllStatesQuery();
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogInformation("No states found for API version {Version}.", version);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.STATES_NOT_FOUND, false, String.Empty));
            }

            _logger.LogInformation("States retrieved successfully. Total states: {Count}.", result.Count());
            //return Results.Json(new ApiResponse<IEnumerable<StateDto>>(200, $"States retrieved successfully. Total states: {result.Count()}.", true, result));
            return Results.Json(new ApiResponse<IEnumerable<StateDto>>(200, AppConstants.ErrorCodes.STATES_FETECHED_SUCCESSFULLY, true, result));

        });

        endpoints.MapPost("api/v{version}/states", [Authorize] async (int version, List<CreateUpdateStateDto> createStatedto, IMediator mediator, IValidator<CreateStateCommand> validator) =>
        {
            _logger.LogInformation("Creating new states for API version {Version}.", version);
            var command = new CreateStateCommand(createStatedto);

            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation failed while creating states for API version {Version}.", version);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogInformation("Duplicate entry detected while creating states for API version {Version}.", version);
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.STATE_DUPLICATE_ENTRY, false, String.Empty));
            }

            _logger.LogInformation("State(s) created successfully for API version {Version}.", version);
            return Results.Json(new ApiResponse<bool>(204, AppConstants.ErrorCodes.STATE_CREATED_SUCCESSFULLY, true, result));
        });

        endpoints.MapPut("api/v{version}/states/{stateId}", [Authorize] async (int version, int stateId, CreateUpdateStateDto updateStatedto, IMediator mediator, IValidator<UpdateStateCommand> validator) =>
        {
            _logger.LogInformation("Updating state with ID {StateId} for API version {Version}.", stateId, version);
            var command = new UpdateStateCommand(stateId, updateStatedto);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation failed while updating state with ID {StateId} for API version {Version}.", stateId, version);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogInformation("State not found or invalid country ID while updating state with ID {StateId} for API version {Version}.", stateId, version);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.STATES_NOT_FOUND, false, String.Empty));
            }

            _logger.LogInformation("State with ID {StateId} updated successfully for API version {Version}.", stateId, version);
            return Results.Json(new ApiResponse<bool>(204, AppConstants.ErrorCodes.STATE_MASTER_UPDATE_SUCCESSFUL, true, true));
        });

        endpoints.MapDelete("api/v{version}/states/{stateId}", [Authorize] async (int version, int stateId, IMediator mediator, IValidator<DeleteStateCommand> validator) =>
        {
            _logger.LogInformation("Deleting state with ID {StateId} for API version {Version}.", stateId, version);
            var command = new DeleteStateCommand(stateId);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation failed while deleting state with ID {StateId} for API version {Version}.", stateId, version);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogInformation("State cannot be deleted as it is referenced in other tables or does not exist. State ID: {StateId}, API version: {Version}.", stateId, version);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.STATES_NOT_FOUND, false, String.Empty));
            }

            _logger.LogInformation("State with ID {StateId} deleted successfully for API version {Version}.", stateId, version);
            return Results.Json(new ApiResponse<bool>(204, AppConstants.ErrorCodes.STATE_MASTER_DELETE_SUCCESSFUL, true, true));
        });

        endpoints.MapGet("api/v{version}/states/{pageNumber}/{pageSize}", [Authorize] async (int version, string? searchTerm, int pageNumber, int pageSize, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching paged states for API version {Version}. Page: {PageNumber}, Size: {PageSize}, SearchTerm: {SearchTerm}", version, pageNumber, pageSize, searchTerm);
            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var query = new GetPagedStatesWithFiltersQuery(searchTerm, pageNumber, pageSize);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogInformation("States not found for API version {Version} with search term '{SearchTerm}'.", version, searchTerm);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.STATES_NOT_FOUND, false, String.Empty));
            }

            _logger.LogInformation("States fetched successfully for API version {Version}.", version);
            return Results.Json(new ApiResponse<PaginatedResponse<StateDto>>(200, AppConstants.ErrorCodes.STATES_FETECHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/countries/{countryId}/states", [Authorize] async (int version, int countryId, IMediator mediator) =>
        {
            _logger.LogInformation("Received request to get all states for countryId: {CountryId}.", countryId);
            var query = new GetStatesByCountryIdQuery(countryId);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogError("No states found for the given Country ID {CountryId}.", countryId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.STATES_NOT_FOUND, false, string.Empty));
            }

            _logger.LogInformation("States for Country ID {CountryId} fetched successfully.", countryId);
            return Results.Json(new ApiResponse<CountryWithStatesDto>(200, AppConstants.ErrorCodes.STATES_FETECHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/states/template/States.xlsx", [Authorize] async (int version, IFileHandlingService fileHandlingService, IMediator mediator) =>
        {
            _logger.LogInformation("Received request to get states bulk upload template.");
            var query = new GetAllCountriesQuery();
            var result = await mediator.Send(query);
            if (result == null || !result.Any())
            {
                _logger.LogError("No countries found.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.STATES_NOT_FOUND, false, string.Empty));
            }
            var template = await fileHandlingService.GetStatesBulkUploadTemplateAsync(result.Select(c => c.CountryName).ToList());
            if (template == null)
            {
                _logger.LogError("States bulk upload template not found.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.STATES_NOT_FOUND, false, string.Empty));
            }
            _logger.LogInformation("States bulk upload template fetched successfully.");
            return Results.File(template, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "States.xlsx");
        });

        endpoints.MapGet("api/v{version}/states/template/Country.xlsx", [Authorize] async (int version, IFileHandlingService fileHandlingService, IMediator mediator) =>
        {
            _logger.LogInformation("Received request to get states bulk upload template.");
    
            var template = await fileHandlingService.GetStatesBulkUploadTemplateForCountryAsync();
            if (template == null)
            {
                _logger.LogError("States bulk upload for country template not found.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.EXPORT_DATA_FAILED, false, string.Empty));
            }
            _logger.LogInformation("States bulk upload for country template fetched successfully.");
            return Results.File(template, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Country.xlsx");
        });

        endpoints.MapPost("api/v{version}/states/bulk-upload", [Authorize] async (int version, int? countryId, IFormFile formFile, IFileHandlingService fileHandlingService, IFileDataConverterService fileDataConverterService, IMediator mediator,IValidator<BulkInsertStatesCommand> validator) =>
        {
            _logger.LogInformation("Received request to upload states in bulk.");
            countryId ??= 0; // Default to 0 if not provided

            if (!fileHandlingService.IsFileFormatSupportedForBulkUpload(formFile))
            {
                _logger.LogError("File format not supported.");
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.FILE_FORMAT_NOT_SUPPORTED, false, string.Empty));
            }

            List<string> expectedHeaders = new List<string>
            {
                "StateName *",
                "StateShortName *",
                "Description"
            };
            if (!fileHandlingService.ValidateTemplateHeaders(formFile, expectedHeaders))
            {
                _logger.LogError($"File template headers are invalid");
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.IMPORT_FILE_TEMPLATE_MISMATCH, false, string.Empty));
            }

            var data = fileHandlingService.ProcessXlsx(formFile);
            if (data == null || !data.Any())
            {
                _logger.LogError("No data found in the uploaded file.");
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.IMPORT_FILE_EMPTY, false, string.Empty));
            }

            var stateDtos = (countryId == 0) ? fileDataConverterService.ConvertToStateDto(data) : fileDataConverterService.ConvertToStateDtoForCountry(data);
            if (stateDtos == null || !stateDtos.Any())
            {
                _logger.LogError("No valid state data found in the uploaded file.");
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.INVALID_DATA_FORMAT, false, string.Empty));
            }
            
            var command = new BulkInsertStatesCommand(stateDtos,countryId,formFile.FileName);
            //Need to pass Country Id since we are using it in the validator to check if call is with country Id or not
            var validationContext = new ValidationContext<BulkInsertStatesCommand>(command);
            validationContext.RootContextData["CountryId"] = command.countryId ?? 0; // Inject CountryId

            var validationResult = await validator.ValidateAsync(validationContext);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed for States bulk upload.");
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }

            var result = await mediator.Send(command);
            int statusCode = 200;
            string message = AppConstants.ErrorCodes.COMMON_MASTER_LIST_FILE_UPLOAD_SUCCESS;
            if (!result.IsSuccess)
            {
                if (result.IsPartialSuccess)
                {
                    statusCode = 207;
                    message = AppConstants.ErrorCodes.BULK_UPLOAD_PARTIALLY_SUCCESS;
                }
                statusCode = 422;
                message = AppConstants.ErrorCodes.BULK_UPLOAD_FAILED;//"Upload Failed! The file contains unsupported data. Please check your file and try again.";
            }


            _logger.LogInformation(message);
            return Results.Json(new ApiResponse<BulkUploadStatusDto>(statusCode, message, result.IsSuccess, result));

        }).DisableAntiforgery();

        endpoints.MapPost("api/v{version}/states/validate-duplicate", [Authorize] async (int version, CreateUpdateStateDto stateDto, IMediator mediator) =>
        {
            _logger.LogInformation("Validating duplicate state (API v{Version})", version);
            var query = new GetValidateDuplicateStatesQuery(stateDto);
            var isDuplicate = await mediator.Send(query);
            if (isDuplicate)
            {
                _logger.LogWarning("Duplicate state detected (API v{Version})", version);
                return Results.Json(new ApiResponse<bool>(409, AppConstants.ErrorCodes.STATE_DUPLICATE_ENTRY, false, true));
            }
            _logger.LogInformation("No duplicate state found (API v{Version})", version);
            return Results.Json(new ApiResponse<bool>(200, AppConstants.ErrorCodes.MASTER_NO_DUPLICATES, true, false));
        });
    }
}
