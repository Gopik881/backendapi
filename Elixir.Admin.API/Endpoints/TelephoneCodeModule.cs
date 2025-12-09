using Elixir.Application.Common.DTOs;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Country.Queries.GetAllCountries;
using Elixir.Application.Features.Telephone.Commands.BulkInsertTelephoneCodes;
using Elixir.Application.Features.Telephone.Commands.CreateTelephone;
using Elixir.Application.Features.Telephone.Commands.DeleteTelephone;
using Elixir.Application.Features.Telephone.Commands.UpdateTelephone;
using Elixir.Application.Features.Telephone.DTOs;
using Elixir.Application.Features.Telephone.Queries.GetAllTelephoneCodes;
using Elixir.Application.Features.Telephone.Queries.GetPagedTelephoneCodeWithFilters;
using Elixir.Application.Interfaces.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Telephone.Queries.GetValidateDuplicateTelephoneCodes;

namespace Elixir.Admin.API.Endpoints;

public static class TelephoneCodeModule
{
    private static ILogger _logger;

    public static void RegisterTelephoneCodeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        _logger ??= loggerFactory.CreateLogger("TelephoneCodeApiRoutes");

        endpoints.MapGet("api/v{version}/telephone-codes", [Authorize] async (int version, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching all telephone codes for API version {Version}", version);
            var query = new GetAllTelephoneCodesQuery();
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogInformation("No telephone codes found for API version {Version}", version);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.CLIENT_ACCOUNT_MANAGERS_RETRIEVAL_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Successfully retrieved {Count} telephone codes for API version {Version}", result.Count(), version);
            return Results.Json(new ApiResponse<IEnumerable<TelephoneCodeMasterDto>>(200, AppConstants.ErrorCodes.TELEPOHONE_CODES_FETECHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapPost("api/v{version}/telephone-codes", [Authorize] async (int version, List<CreateUpdateTelephoneCodeDto> createTelephoneCodeDto, IMediator mediator, IValidator<CreateTelephoneCommand> validator) =>
        {
            _logger.LogInformation("Creating telephone codes for API version {Version}", version);
            var command = new CreateTelephoneCommand(createTelephoneCodeDto);

            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation failed while creating telephone codes for API version {Version}", version);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogInformation("Duplicate entry detected while creating telephone codes for API version {Version}", version);
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.COMMON_MASTER_LIST_REFRESH_SUCCESS, false, String.Empty));
            }

            _logger.LogInformation("Telephone codes created successfully for API version {Version}", version);
            return Results.Json(new ApiResponse<bool>(204, AppConstants.ErrorCodes.COMMON_MASTER_LIST_REFRESH_SUCCESS, true, result));
        });

        endpoints.MapPut("api/v{version}/telephone-codes/{telephoneCodeId}", [Authorize] async (int version, int telephoneCodeId, CreateUpdateTelephoneCodeDto updateTelephoneCodeDto, IMediator mediator, IValidator<UpdateTelephoneCommand> validator) =>
        {
            // Validate telephone code format: must start with '+' and contain only digits after '+'
            var rawCode = updateTelephoneCodeDto?.TelephoneCode?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(rawCode) || !rawCode.StartsWith('+'))
            {
                _logger.LogWarning("Invalid telephone code format (missing '+') (API v{Version})", version);
                return Results.Json(new ApiResponse<bool>(409, AppConstants.ErrorCodes.INVALID_TELEPHONE_CODE_FORMAT, false, true));
            }

            _logger.LogInformation("Updating telephone code with ID {TelephoneCodeId} for API version {Version}", telephoneCodeId, version);
            var command = new UpdateTelephoneCommand(telephoneCodeId, updateTelephoneCodeDto);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation failed while updating telephone code with ID {TelephoneCodeId} for API version {Version}", telephoneCodeId, version);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogInformation("Telephone code with ID {TelephoneCodeId} not found or invalid country ID for API version {Version}", telephoneCodeId, version);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.STATES_NOT_FOUND, false, string.Empty));
            }

            _logger.LogInformation("Telephone code with ID {TelephoneCodeId} updated successfully for API version {Version}", telephoneCodeId, version);
            return Results.Json(new ApiResponse<bool>(200, AppConstants.ErrorCodes.COMPANY_LIST_UPDATE_SUCCESS, true, true));
        });

        endpoints.MapDelete("api/v{version}/telephone-codes/{telephoneCodeId}", [Authorize] async (int version, int telephoneCodeId, IMediator mediator, IValidator<DeleteTelephoneCommand> validator) =>
        {
            _logger.LogInformation("Deleting telephone code with ID {TelephoneCodeId} for API version {Version}", telephoneCodeId, version);
            var command = new DeleteTelephoneCommand(telephoneCodeId);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation failed while deleting telephone code with ID {TelephoneCodeId} for API version {Version}", telephoneCodeId, version);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(command);            
            if (!result)
            {
                _logger.LogInformation("Telephone code with ID {TelephoneCodeId} cannot be deleted as it is referenced in other tables or does not exist for API version {Version}", telephoneCodeId, version);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.TELEPHONE_CODE_MASTER_DELETE_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Telephone code with ID {TelephoneCodeId} deleted successfully for API version {Version}", telephoneCodeId, version);
            return Results.Json(new ApiResponse<bool>(204, AppConstants.ErrorCodes.COMPANY_LIST_UPDATE_SUCCESS, true, true));
        });

        endpoints.MapGet("api/v{version}/telephone-codes/{pageNumber}/{pageSize}", [Authorize] async (int version, string? searchTerm, int pageNumber, int pageSize, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching paged telephone codes for API version {Version}, PageNumber: {PageNumber}, PageSize: {PageSize}, SearchTerm: {SearchTerm}", version, pageNumber, pageSize, searchTerm);
            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var query = new GetPagedTelephoneWithFiltersQuery(searchTerm, pageNumber, pageSize);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogInformation("No telephone codes found for API version {Version} with the given filters", version);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.TELEPHONE_CODE_MASTER_RECORD_NOT_FOUND, false, String.Empty));
            }

            _logger.LogInformation("Successfully fetched paged telephone codes for API version {Version}, PageNumber: {PageNumber}, PageSize: {PageSize}", version, pageNumber, pageSize);
            return Results.Json(new ApiResponse<PaginatedResponse<TelephoneCodeMasterDto>>(200, AppConstants.ErrorCodes.TELEPOHONE_CODES_FETECHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/telephone-codes/template/Telephone_Codes.xlsx", [Authorize] async (int version, IFileHandlingService fileHandlingService, IMediator mediator) =>
        {
            _logger.LogInformation("Received request to get telephone codes bulk upload template.");
            var query = new GetAllCountriesQuery();
            var result = await mediator.Send(query);
            if (result == null || !result.Any())
            {
                _logger.LogError("No countries found.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.COUNTRY_MASTER_NO_RECORDS_FOUND, false, string.Empty));
            }
            var template = await fileHandlingService.GetTelephoneCodeBulkUploadTemplateAsync(result.Select(c => c.CountryName).ToList());
            if (template == null)
            {
                _logger.LogError("Telephone codes bulk upload template not found.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.EXPORT_DATA_FAILED, false, string.Empty));
            }
            _logger.LogInformation("Telephone codes bulk upload template fetched successfully.");
            return Results.File(template, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Telephone_Codes.xlsx");
        });

        endpoints.MapPost("api/v{version}/telephone-codes/bulk-upload", [Authorize] async (int version, IFormFile formFile, IFileHandlingService fileHandlingService, IFileDataConverterService fileDataConverterService, IMediator mediator, IValidator<BulkInsertTelephoneCodesCommand> validator) =>
        {
            _logger.LogInformation("Received request to upload telephone codes in bulk.");

            if (!fileHandlingService.IsFileFormatSupportedForBulkUpload(formFile))
            {
                _logger.LogError("File format not supported.");
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.FILE_FORMAT_NOT_SUPPORTED, false, string.Empty));
            }

            List<string> expectedHeaders = new List<string>
            {
                "CountryName *",
                "TelephoneCode *",
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
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.IMPORT_FILE_EMPTY_OR_NOT_PROVIDED, false, string.Empty));
            }

            var telephonecodesDtos = fileDataConverterService.ConvertToTelephoneCodeDto(data);
            if (telephonecodesDtos == null || !telephonecodesDtos.Any())
            {
                _logger.LogError("No valid telephone code data found in the uploaded file.");
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.INVALID_DATA_FORMAT, false, string.Empty));
            }

            var command = new BulkInsertTelephoneCodesCommand(telephonecodesDtos, formFile.FileName);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed for telephone code bulk upload.");
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
                message = AppConstants.ErrorCodes.BULK_UPLOAD_FAILED;
            }

            _logger.LogInformation(message);
            return Results.Json(new ApiResponse<BulkUploadStatusDto>(statusCode, message, result.IsSuccess, result));

        }).DisableAntiforgery();

        endpoints.MapPost("api/v{version}/telephone-codes/validate-duplicate", [Authorize] async (int version, CreateUpdateTelephoneCodeDto telephoneCodeDto, IMediator mediator) =>
        {
            // Validate telephone code format: must start with '+' and contain only digits after '+'
            var rawCode = telephoneCodeDto?.TelephoneCode?.Trim() ?? string.Empty;
            // Validate that only digits exist after '+'
            var digitsPart = rawCode.Length > 1 ? rawCode.Substring(1) : string.Empty;          
            
            if (string.IsNullOrEmpty(rawCode) || !rawCode.StartsWith('+') || string.IsNullOrEmpty(digitsPart.Trim()) || !digitsPart.Trim().All(char.IsDigit))
            {
                _logger.LogWarning("Invalid telephone code format (missing '+') (API v{Version})", version);
                return Results.Json(new ApiResponse<bool>(409, AppConstants.ErrorCodes.INVALID_TELEPHONE_CODE_FORMAT, false, true));
            }
            _logger.LogInformation("Validating duplicate telephone code (API v{Version})", version);
            var query = new GetValidateDuplicateTelephoneCodesQuery(telephoneCodeDto);
            var isDuplicate = await mediator.Send(query);
            if (isDuplicate)
            {
                _logger.LogWarning("Duplicate telephone code detected (API v{Version})", version);
                return Results.Json(new ApiResponse<bool>(409, AppConstants.ErrorCodes.TELEPHONE_CODE_MASTER_DUPLICATE_FOUND, false, true));
            }
            _logger.LogInformation("No duplicate telephone code found (API v{Version})", version);
            return Results.Json(new ApiResponse<bool>(200, AppConstants.ErrorCodes.MASTER_NO_DUPLICATES, true, false));
        });
    }


}
