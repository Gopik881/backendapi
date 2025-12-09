using Elixir.Application.Common.DTOs;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Country.Queries.GetAllCountries;
using Elixir.Application.Features.Currency.Commands.BulkInsertCurrencies;
using Elixir.Application.Features.Currency.Commands.CreateCurrency;
using Elixir.Application.Features.Currency.Commands.DeleteCurrency;
using Elixir.Application.Features.Currency.Commands.UpdateCurrency;
using Elixir.Application.Features.Currency.DTOs;
using Elixir.Application.Features.Currency.Queries.GetAllCurrency;
using Elixir.Application.Features.Currency.Queries.GetPagedCurrenciesWithFilters;
using Elixir.Application.Interfaces.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Currency.Queries.GetValidateDuplicateCurrencies;

namespace Elixir.Admin.API.Endpoints;
public static class CurrencyModule
{
    private static ILogger _logger;

    public static void RegisterCurrencyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        _logger ??= loggerFactory.CreateLogger("CurrencyApiRoutes");

        endpoints.MapGet("api/v{version}/currencies", [Authorize] async (int version, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching all currencies (API v{Version})", version);
            var query = new GetAllCurrencyQuery();
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogError("No currencies found (API v{Version})", version);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.CURRENCY_MASTER_NO_RECORD_FOUND, false, string.Empty));
            }

            _logger.LogInformation("Currencies retrieved successfully. Total currencies: {Count} (API v{Version})", result.Count(), version);
            return Results.Json(new ApiResponse<IEnumerable<CurrencyDto>>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapPost("api/v{version}/currencies", [Authorize] async (int version, List<CreateUpdateCurrencyDto> createCurrencyDto, IMediator mediator, IValidator<CreateCurrencyCommand> validator) =>
        {
            _logger.LogInformation("Creating new currencies (API v{Version})", version);
            var command = new CreateCurrencyCommand(createCurrencyDto);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed while creating currencies (API v{Version})", version);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogError("Duplicate entry detected while creating currencies (API v{Version})", version);
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.CURRENCY_MASTER_ALREADY_MAPPED, false, string.Empty));
            }

            _logger.LogInformation("Currency created successfully (API v{Version})", version);
            return Results.Json(new ApiResponse<bool>(204, AppConstants.ErrorCodes.COMMON_MASTER_LIST_REFRESH_SUCCESS, true, result));
        });

        endpoints.MapPut("api/v{version}/currencies/{currencyId}", [Authorize] async (int version, int currencyId, CreateUpdateCurrencyDto updateCurrencyDto, IMediator mediator, IValidator<UpdateCurrencyCommand> validator) =>
        {
            _logger.LogInformation("Updating currency with ID {CurrencyId} (API v{Version})", currencyId, version);
            var command = new UpdateCurrencyCommand(currencyId, updateCurrencyDto);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed while updating currency with ID {CurrencyId} (API v{Version})", currencyId, version);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogError("Currency not found or invalid country ID while updating currency with ID {CurrencyId} (API v{Version})", currencyId, version);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.CURRENCY_MASTER_NOT_FOUND, false, string.Empty));
            }

            _logger.LogInformation("Currency updated successfully with ID {CurrencyId} (API v{Version})", currencyId, version);
            return Results.Json(new ApiResponse<bool>(204, AppConstants.ErrorCodes.USER_MAPPING_UPDATE_SUCCESS, true, true));
        });

        endpoints.MapDelete("api/v{version}/currencies/{currencyId}", [Authorize] async (int version, int currencyId, IMediator mediator, IValidator<DeleteCurrencyCommand> validator) =>
        {
            _logger.LogInformation("Deleting currency with ID {CurrencyId} (API v{Version})", currencyId, version);
            var command = new DeleteCurrencyCommand(currencyId);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed while deleting currency with ID {CurrencyId} (API v{Version})", currencyId, version);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogError("Currency cannot be deleted as it is referenced in other tables or does not exist. CurrencyId: {CurrencyId} (API v{Version})", currencyId, version);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.CURRENCY_MASTER_DELETE_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Currency deleted successfully with ID {CurrencyId} (API v{Version})", currencyId, version);
            return Results.Json(new ApiResponse<bool>(204, AppConstants.ErrorCodes.COMMON_MASTER_LIST_DELETE_SUCCESS, true, true));
        });

        endpoints.MapGet("api/v{version}/currencies/{pageNumber}/{pageSize}", [Authorize] async (int version, string? searchTerm, int pageNumber, int pageSize, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching paged currencies (API v{Version}) Page: {PageNumber}, Size: {PageSize}, SearchTerm: {SearchTerm}", version, pageNumber, pageSize, searchTerm);
            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var query = new GetPagedCurrenciesWithFiltersQuery(searchTerm, pageNumber, pageSize);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogError("Currencies not found for paged request (API v{Version})", version);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.CURRENCY_MASTER_NOT_FOUND, false, String.Empty));
            }

            _logger.LogInformation("Currencies fetched successfully for paged request (API v{Version})", version);
            return Results.Json(new ApiResponse<PaginatedResponse<CurrencyDto>>(200, AppConstants.ErrorCodes.COMMAN_MASTER_CURRENCY_RETRIEVAL_SUCCESS, true, result));
        });
        
        endpoints.MapGet("api/v{version}/currencies/template/Currencies.xlsx", [Authorize] async (int version, IFileHandlingService fileHandlingService, IMediator mediator) =>
        {
            _logger.LogInformation("Received request to get currencies bulk upload template.");
            var query = new GetAllCountriesQuery();
            var result = await mediator.Send(query);
            if (result == null || !result.Any())
            {
                _logger.LogError("No countries found.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.IMPORT_FILE_EMPTY, false, string.Empty));
            }

            var template = await fileHandlingService.GetCurrenciesBulkUploadTemplateAsync(result.Select(c=>c.CountryName).ToList());
            if (template == null)
            {
                _logger.LogError("Currencies bulk upload template not found.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.IMPORT_FILE_EMPTY, false, string.Empty));
            }
            _logger.LogInformation("Currencies bulk upload template fetched successfully.");
            return Results.File(template, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Currencies.xlsx");
        });

        endpoints.MapPost("api/v{version}/currencies/bulk-upload", [Authorize] async (int version, IFormFile formFile, IFileHandlingService fileHandlingService, IFileDataConverterService fileDataConverterService, IMediator mediator, IValidator<BulkInsertCurrencyCommand> validator) =>
        {
            _logger.LogInformation("Received request to upload currency in bulk.");

            if (!fileHandlingService.IsFileFormatSupportedForBulkUpload(formFile))
            {
                _logger.LogError("File format not supported.");
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.FILE_FORMAT_NOT_SUPPORTED, false, string.Empty));
            }

            List<string> expectedHeaders = new List<string>
            {
                "CountryName *",
                "CurrencyName *",
                "CurrencyShortName *",
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
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.CURRENCY_MASTER_NO_RECORD_FOUND, false, string.Empty));
            }

            var currencyDtos = fileDataConverterService.ConvertToCurrencyDto(data);
            if (currencyDtos == null || !currencyDtos.Any())
            {
                _logger.LogError("No valid currency data found in the uploaded file.");
                return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.INVALID_DATA_FORMAT, false, string.Empty));
            }

            var command = new BulkInsertCurrencyCommand(currencyDtos,formFile.FileName);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed for currency bulk upload.");
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

        endpoints.MapPost("api/v{version}/currencies/validate-duplicate", [Authorize] async (int version, CreateUpdateCurrencyDto currencyDto, IMediator mediator) =>
        {
            _logger.LogInformation("Validating duplicate currency (API v{Version})", version);
            var query = new GetValidateDuplicateCurrenciesQuery(currencyDto);
            var isDuplicate = await mediator.Send(query);
            if (isDuplicate)
            {
                _logger.LogWarning("Duplicate currency detected (API v{Version})", version);
                return Results.Json(new ApiResponse<bool>(409, AppConstants.ErrorCodes.CURRENCY_MASTER_DUPLICATE_FOUND, false, true));
            }
            _logger.LogInformation("No duplicate currency found (API v{Version})", version);
            return Results.Json(new ApiResponse<bool>(200, AppConstants.ErrorCodes.MASTER_NO_DUPLICATES, true, false));
        });
    }
}

