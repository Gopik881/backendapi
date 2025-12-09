using Elixir.Application.Common.Constants;
using Elixir.Application.Common.DTOs;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Country.Commands.BulkInsert;
using Elixir.Application.Features.Country.Commands.CreateCountry;
using Elixir.Application.Features.Country.Commands.DeleteCountry;
using Elixir.Application.Features.Country.Commands.UpdateCountry;
using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Features.Country.Queries.GetAllCountries;
using Elixir.Application.Features.Country.Queries.GetCountryWithStates;
using Elixir.Application.Features.Country.Queries.GetPagedCountriesWithFilters;
using Elixir.Application.Features.Country.Queries.GetValidateDuplicateCountries;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;



namespace Elixir.Admin.API.Endpoints;

public static class CountryModule
{
    private static ILogger _logger;

    public static void RegisterCountryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        _logger ??= loggerFactory.CreateLogger("CountryApiRoutes");

        endpoints.MapGet("api/v{version}/countries/{countryId}/states/{pageNumber}/{pageSize}", [Authorize] async (int version, string? searchTerm, int pageNumber, int pageSize, int countryId, IMediator mediator, IValidator<GetCountryWithStatesQuery> validator) =>
        {
            _logger.LogInformation($"Received request to get states for countryId: {countryId}.");
            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var query = new GetCountryWithStatesQuery(searchTerm, pageNumber, pageSize, countryId);
            var validationResult = await validator.ValidateAsync(query);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed for GetCountryWithStatesQuery.");
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogError($"No states found for the given Country ID {countryId}.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.STATE_NOT_FOUND_FOR_COUNTRY, false, String.Empty));
            }

            _logger.LogInformation($"States for Country ID {countryId} fetched successfully.");
            return Results.Json(new ApiResponse<PaginatedResponse<CountryWithStatesDto>>(200, AppConstants.ErrorCodes.COUNTRY_MASTER_RETRIEVAL_SUCCESS, true, result));
        });

        endpoints.MapPost("api/v{version}/countries", [Authorize] async (int version, List<CreateUpdateCountryDto> CreateCountryDto, IMediator mediator, IValidator<CreateCountryCommand> validator) =>
        {
            _logger.LogInformation("Received request to create country.");
            var command = new CreateCountryCommand(CreateCountryDto);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed for CreateCountryCommand.");
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogError("Duplicate entry detected. The data you're trying to insert already exists.");
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.COUNTRY_DUPLICATE_ENTRY, false, String.Empty));
            }
            _logger.LogInformation("Country created successfully.");
            return Results.Json(new ApiResponse<bool>(204, AppConstants.ErrorCodes.COMMON_MASTER_LIST_REFRESH_SUCCESS, true, result));
        });

        endpoints.MapPut("api/v{version}/countries/{countryId}", [Authorize] async (int version, int countryId, CreateUpdateCountryDto UpdateCountryDto, IMediator mediator, IValidator<UpdateCountryCommand> validator) =>
        {
            _logger.LogInformation($"Received request to update country with countryId: {countryId}.");
            var command = new UpdateCountryCommand(countryId, UpdateCountryDto);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed for UpdateCountryCommand.");
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogError($"Country not found for countryId: {countryId}.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.COUNTRY_NOT_FOUND, false, String.Empty));
            }

            _logger.LogInformation($"Country with countryId: {countryId} updated successfully.");
            return Results.Json(new ApiResponse<bool>(204, AppConstants.ErrorCodes.COMPANY_LIST_UPDATE_SUCCESS, true, true));
        });

        endpoints.MapDelete("api/v{version}/countries/{countryId}", [Authorize] async (int version, int countryId, IMediator mediator, IValidator<DeleteCountryCommand> validator) =>
        {
            _logger.LogInformation($"Received request to delete country with countryId: {countryId}.");
            var command = new DeleteCountryCommand(countryId);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed for DeleteCountryCommand.");
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogError($"Country cannot be deleted as it is referenced in other tables or does not exist. countryId: {countryId}.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.COMMON_MASTER_LIST_LINKED_ITEM_DELETE_ATTEMPT, false, String.Empty));
            }

            _logger.LogInformation($"Country with countryId: {countryId} deleted successfully.");
            return Results.Json(new ApiResponse<bool>(204, AppConstants.ErrorCodes.COMMON_MASTER_LIST_DELETE_SUCCESS, true, true));
        });

        endpoints.MapGet("api/v{version}/countries/{pageNumber}/{pageSize}", [Authorize] async (int version, string? searchTerm, int pageNumber, int pageSize, IMediator mediator) =>
        {
            _logger.LogInformation("Received request to get paged countries.");
            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var query = new GetPagedCountriesWithFiltersQuery(searchTerm, pageNumber, pageSize);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogError("Countries not found.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.GET_5TAB_ONBOARDING_HISTORY_FAILED, false, String.Empty));
            }

            _logger.LogInformation("Countries fetched successfully.");
            return Results.Json(new ApiResponse<PaginatedResponse<CountryDto>>(200, AppConstants.ErrorCodes.COUNTRY_MASTER_RETRIEVAL_SUCCESS, true, result));
        });

        endpoints.MapGet("api/v{version}/countries/all", [Authorize] async (int version, IMediator mediator) =>
        {
            _logger.LogInformation("Received request to get all countries.");
            var query = new GetAllCountriesQuery();
            var result = await mediator.Send(query);
            if (result == null || !result.Any())
            {
                _logger.LogError("No countries found.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.COUNTRY_NOT_FOUND, false, string.Empty));
            }

            _logger.LogInformation("Countries fetched successfully.");
            return Results.Json(new ApiResponse<IEnumerable<CountryDto>>(200, AppConstants.ErrorCodes.COUNTRY_MASTER_RETRIEVAL_SUCCESS, true, result));
        });

        endpoints.MapGet("api/v{version}/countries/template/Countries.xlsx", [Authorize] async (int version, IFileHandlingService fileHandlingService) =>
        {
            _logger.LogInformation("Received request to get countries bulk upload template.");
            var template = await fileHandlingService.GetCountriesBulkUploadTemplateAsync();
            if (template == null)
            {
                _logger.LogError("Countries bulk upload template not found.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.EXPORT_DATA_FAILED, false, string.Empty));
            }
            _logger.LogInformation("Countries bulk upload template fetched successfully.");
            return Results.File(template, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Countries.xlsx");
        });

        endpoints.MapPost("api/v{version}/countries/bulk-upload", [Authorize] async (int version, IFormFile formFile, IFileHandlingService fileHandlingService, IFileDataConverterService fileDataConverterService, IMediator mediator, IValidator<BulkInsertCountriesCommand> validator) =>
        {
            _logger.LogInformation("Received request to upload countries in bulk.");

            if (!fileHandlingService.IsFileFormatSupportedForBulkUpload(formFile))
            {
                _logger.LogError("File format not supported.");
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.FILE_FORMAT_NOT_SUPPORTED, false, string.Empty));
            }

            List<string> expectedHeaders = new List<string>()
            {
                "CountryName", "CountryShortName", "Description"
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
                return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.IMPORT_FILE_EMPTY, false, string.Empty));
            }
            var countryDtos = fileDataConverterService.ConvertToCountryDto(data);
            if (countryDtos == null || !countryDtos.Any())
            {
                _logger.LogError("No valid country data found in the uploaded file.");
                return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.INVALID_DATA_FORMAT, false, string.Empty));
            }

            var command = new BulkInsertCountriesCommand(countryDtos, formFile.FileName);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed for countries bulk upload.");
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


        endpoints.MapPost("api/v{version}/countries/validate-duplicate", [Authorize] async (int version, CreateUpdateCountryDto countryDto, IMediator mediator) =>
        {
            _logger.LogInformation("Received request to validate duplicate country.");
            var query = new GetValidateDuplicateCountriesQuery(countryDto);
            var isDuplicate = await mediator.Send(query);
            if (isDuplicate)
            {
                _logger.LogError("Duplicate country detected.");
                return Results.Json(new ApiResponse<bool>(409, AppConstants.ErrorCodes.COUNTRY_DUPLICATE_ENTRY, false, true));
            }
            _logger.LogInformation("No duplicate country found.");
            return Results.Json(new ApiResponse<bool>(200, AppConstants.ErrorCodes.MASTER_NO_DUPLICATES, true, false));
        });
    }
}
