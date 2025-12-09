using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.Commands.CreateCompany.CreateCompany;
using Elixir.Application.Features.Company.Commands.CreateCompany.UpdateCompany;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.CheckCompanyCode;
using Elixir.Application.Features.Company.Queries.GetCompanyElixirUsersList;
using Elixir.Application.Features.Company.Queries.GetCompanyPopupDetails;
using Elixir.Application.Features.Company.Queries.GetElixirUsers;
using Elixir.Application.Features.Company.Queries.GetPagedAccountManagerByCompany;
using Elixir.Application.Features.Company.Queries.GetPagedCompaniesOnBoardingSummaryForAdminUsers;
using Elixir.Application.Features.Company.Queries.GetPagedCompaniesOnBoardingSummaryForTMIUsers;
using Elixir.Application.Features.Company.Queries.GetPagedCompaniesSummaryForAdminUsers;
using Elixir.Application.Features.Company.Queries.GetPagedCompaniesSummaryForTMIUsers;
using Elixir.Application.Features.Company.Queries.GetPagedCompanyUsers;
using Elixir.Application.Features.Company.Queries.GetSuperAdminDashBoardDetails;
using Elixir.Application.Features.Company.Queries.GetTMIAdminDashBoardDetails;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Features.Module.Queries.GetPagedCompanyModuleSubModules;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Elixir.Admin.API.Endpoints;

public static class CompanyModule
{
    private static ILogger _logger;

    public static void RegisterCompanyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        _logger ??= loggerFactory.CreateLogger("CompanyApiRoutes");

        endpoints.MapGet("api/v{version}/companies/{companyCode}/exists", async (int version, string companyCode, IMediator mediator, IValidator<CheckCompanyCodeExistsQuery> validator) =>
        {
            _logger.LogInformation($"Checking if company code '{companyCode}' exists.");
            var query = new CheckCompanyCodeExistsQuery(companyCode);
            var validationResult = await validator.ValidateAsync(query);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed for CheckCompanyCodeExistsQuery.");
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(query);
            if (result < 1)
            {
                _logger.LogError($"Company code '{companyCode}' not found.");
                return Results.Json(new ApiResponse<string>(401, AppConstants.ErrorCodes.COMPANY_CODE_INVALID, false, String.Empty));
            }
            _logger.LogInformation($"Company code '{companyCode}' exists.");
            return Results.Json(new ApiResponse<int>(200, AppConstants.ErrorCodes.COMPANY_CODE_VALID, true, result));
        });

        endpoints.MapGet("api/v{version}/companies/{companyId}/users/{pageNumber}/{pageSize}", [Authorize] async (int version, string? searchTerm, int companyId, int pageNumber, int pageSize, IMediator mediator) =>
        {
            _logger.LogInformation($"Fetching users for companyId: {companyId}, pageNumber: {pageNumber}, pageSize: {pageSize}, searchTerm: '{searchTerm}'.");
            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var query = new GetPagedCompanyUsersQuery(companyId, searchTerm, pageNumber, pageSize);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogError($"Company Users not found for companyId: {companyId}.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, String.Empty));
            }
            _logger.LogInformation($"Company Users fetched successfully for companyId: {companyId}.");
            return Results.Json(new ApiResponse<PaginatedResponse<CompanyUserDto>>(200, AppConstants.ErrorCodes.USER_GROUP_USERS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/companies/{companyId}/accountmanagers/{pageNumber}/{pageSize}", [Authorize] async (int version, string? searchTerm, int companyId, int pageNumber, int pageSize, IMediator mediator) =>
        {
            _logger.LogInformation($"Fetching account managers for companyId: {companyId}, pageNumber: {pageNumber}, pageSize: {pageSize}, searchTerm: '{searchTerm}'.");
            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var query = new GetPagedAccountManagerByCompnayQuery(companyId, searchTerm, pageNumber, pageSize);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogError($"Account Managers not found for companyId: {companyId}.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, String.Empty));
            }
            _logger.LogInformation($"Account Managers fetched successfully for companyId: {companyId}.");
            return Results.Json(new ApiResponse<PaginatedResponse<CompanyUserDto>>(200, AppConstants.ErrorCodes.USER_GROUP_USERS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/companies/{companyId}/modules/{pageNumber}/{pageSize}", [Authorize] async (int version, string? searchTerm, int companyId, int pageNumber, int pageSize, IMediator mediator) =>
        {
            _logger.LogInformation($"Fetching modules for companyId: {companyId}, pageNumber: {pageNumber}, pageSize: {pageSize}, searchTerm: '{searchTerm}'.");
            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var query = new GetPagedModuleInfoByCompany(companyId, searchTerm, pageNumber, pageSize);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogError($"Company Module data not found for companyId: {companyId}.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.MODULE_MANAGEMENT_GET_MODULE_MAPPING_FAILED, false, String.Empty));
            }
            _logger.LogInformation($"Company Module data fetched successfully for companyId: {companyId}.");
            return Results.Json(new ApiResponse<PaginatedResponse<CompanyModuleDto>>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/companies/{pageNumber}/{pageSize}", [Authorize] async (int version, bool IsUnderEdit, int userType, string? searchTerm, int pageNumber, int pageSize, IMediator mediator, ClaimsPrincipal principal) =>
        {
            _logger.LogInformation($"Fetching companies summary for userType: {userType}, pageNumber: {pageNumber}, pageSize: {pageSize}, searchTerm: '{searchTerm}'.");
            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            if (userType == 1)
            {
                bool IsSuperUser = String.Equals(principal.FindFirst(AppConstants.IS_SUPER_ADMIN)?.Value, AppConstants.IS_SUPER_ADMIN_TRUE);
                var query = new GetPagedCompaniesSummaryForAdminUsersQuery(userId, IsUnderEdit, IsSuperUser, searchTerm, pageNumber, pageSize);
                var result = await mediator.Send(query);
                if (result == null)
                {
                    _logger.LogError("Companies data not found for admin user.");
                    return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.COMPANY_NO_COMPANY_FOUND, false, String.Empty));
                }
                _logger.LogInformation("Companies data fetched successfully for admin user.");
                return Results.Json(new ApiResponse<PaginatedResponse<CompanySummaryDto>>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
            }
            else
            {
                var query = new GetPagedCompaniesSummaryForTMIUsersQuery(userId, IsUnderEdit, searchTerm, pageNumber, pageSize);
                var result = await mediator.Send(query);
                if (result == null)
                {
                    _logger.LogError("Companies data not found for TMI user.");
                    return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.COMPANY_NO_COMPANY_FOUND, false, String.Empty));
                }
                _logger.LogInformation("Companies data fetched successfully for TMI user.");
                return Results.Json(new ApiResponse<PaginatedResponse<CompanyTMISummaryDto>>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
            }
        });

        endpoints.MapGet("api/v{version}/companies/onboarding/{pageNumber}/{pageSize}", [Authorize] async (int version, int userType, string? searchTerm, int pageNumber, int pageSize, IMediator mediator, ClaimsPrincipal principal, bool isDashBoard = false) =>
        {
            _logger.LogInformation($"Fetching onboarding companies summary for userType: {userType}, pageNumber: {pageNumber}, pageSize: {pageSize}, searchTerm: '{searchTerm}'.");
            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            if (userType == 1)
            {
                bool IsSuperUser = String.Equals(principal.FindFirst(AppConstants.IS_SUPER_ADMIN)?.Value, AppConstants.IS_SUPER_ADMIN_TRUE);
                var query = new GetPagedCompaniesOnBoardingSummaryForAdminUsersQuery(userId, IsSuperUser, searchTerm, pageNumber, pageSize);
                var result = await mediator.Send(query);
                if (result == null)
                {
                    _logger.LogError("Companies onboarding data not found for admin user.");
                    return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.COMPANY_NO_COMPANY_FOUND, false, String.Empty));
                }
                _logger.LogInformation("Companies onboarding data fetched successfully for admin user.");
                return Results.Json(new ApiResponse<PaginatedResponse<CompanyOnBoardingSummaryDto>>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
            }
            else
            {
                var query = new GetPagedCompaniesOnBoardingSummaryForTMIUsersQuery(userId, searchTerm, pageNumber, pageSize, isDashBoard);
                var result = await mediator.Send(query);
                if (result == null)
                {
                    _logger.LogError("Companies onboarding data not found for TMI user.");
                    return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.COMPANY_NO_COMPANY_FOUND, false, String.Empty));
                }
                _logger.LogInformation("Companies onboarding data fetched successfully for TMI user.");
                return Results.Json(new ApiResponse<PaginatedResponse<CompanyTMIOnBoardingSummaryDto>>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
            }
        });

        endpoints.MapPost("api/v{version}/companies", [Authorize] async (int version, CreateCompanyDto createCompanyDto, IMediator mediator, ClaimsPrincipal principal, IValidator<CreateCompanyCommand> validator) =>
        {
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            if (createCompanyDto == null)
            {
                _logger.LogError("CreateCompanyDto is null.");
                return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.COMPANY_REQUEST_DATA_INVALID, false, string.Empty));
            }

            var command = new CreateCompanyCommand(userId, createCompanyDto);

            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed for CreateCompanyCommand.");
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }

            var result = await mediator.Send(command);
            if (result == null || !result.Success)
            {
                _logger.LogError("Failed to create company. {Error}", result?.Message ?? "Unknown error");
                return Results.Json(new ApiResponse<string>(409, result?.Message ?? AppConstants.ErrorCodes.COMPANY_SAVE_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Company created successfully.");
            return Results.Json(new ApiResponse<bool>(200, AppConstants.ErrorCodes.COMMON_MASTER_LIST_REFRESH_SUCCESS, true, true));
        });

        endpoints.MapPut("api/v{version}/companies/{companyId}", [Authorize] async (int version, int companyId, CreateCompanyDto editCompanyDto, IMediator mediator, ClaimsPrincipal principal, IValidator<UpdateCompanyCommand> validator) =>
        {
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            if (editCompanyDto == null || editCompanyDto.CompanyName == null)
            {
                _logger.LogError("EditCompanyDto is null.");
                return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.COMPANY_REQUEST_DATA_INVALID, false, string.Empty));
            }

            var command = new UpdateCompanyCommand(companyId, userId, editCompanyDto);

            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed for EditCompanyCommand.");
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }

            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogError("Failed to edit company.");
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.COMPANY_EDIT_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Company updated successfully.");
            return Results.Json(new ApiResponse<string>(200, AppConstants.ErrorCodes.COMPANY_LIST_UPDATE_SUCCESS, true, string.Empty));
        });

        endpoints.MapGet("api/v{version}/companies/{companyId}/elixir-users", [Authorize] async (int version, int companyId, IMediator mediator) =>
        {
            _logger.LogInformation($"Fetching Elixir users for companyId: {companyId}.");
            var query = new GetElixirUsersQuery(companyId);
            var result = await mediator.Send(query);
            if (result == null || result.CompanyId == null)
            {
                _logger.LogError($"No Elixir users found for CompanyId '{companyId}'.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.CLIENT_ACCOUNT_MANAGERS_RETRIEVAL_FAILED, false, string.Empty));
            }
            _logger.LogInformation($"Elixir users fetched successfully for companyId: {companyId}.");
            return Results.Json(new ApiResponse<ElixirUserListDto>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/companies/elixir-users-list", [Authorize] async (int version, IMediator mediator, string? ScreenName = "") =>
        {
            _logger.LogInformation("Fetching Elixir users from user group mapping.");
            var query = new GetCompanyElixirUsersListQuery(ScreenName);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogError("No Elixir users found in user group mapping.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, string.Empty));
            }
            _logger.LogInformation("Elixir users fetched successfully from user group mapping.");
            return Results.Json(new ApiResponse<ElixirUserListDto>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/companies/{companyId}/popup-details", [Authorize] async (int version, int companyId, IMediator mediator) =>
        {
            _logger.LogInformation($"Fetching popup details for companyId: {companyId}.");
            var query = new GetCompanyPopupDetailsByCompanyIdQuery(companyId);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogError($"No popup details found for CompanyId '{companyId}'.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.COMPANY_NO_DETAILS_FOUND, false, string.Empty));
            }
            _logger.LogInformation($"Popup details fetched successfully for companyId: {companyId}.");
            return Results.Json(new ApiResponse<object>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/companies/superadmin/dashboard", [Authorize] async (int version, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching Super Admin dashboard details.");

            var query = new GetSuperAdminDashBoardDetailsQuery();
            var result = await mediator.Send(query);

            if (result == null)
            {
                _logger.LogError("No Super Admin dashboard details found.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.PROFILE_NOT_FOUND, false, string.Empty));
            }

            _logger.LogInformation("Super Admin dashboard details fetched successfully.");
            return Results.Json(new ApiResponse<SuperAdminDashBoardDetailsDto>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/companies/tmiadmin/dashboard/{userId}", [Authorize] async (int version, int userId, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching TMI Admin dashboard details for user ID {UserId}.", userId);

            var query = new GetTMIAdminDashBoardDetailsQuery(userId);
            var result = await mediator.Send(query);

            if (result == null)
            {
                _logger.LogError("No TMI Admin dashboard details found for user ID {UserId}.", userId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.CLIENT_ACCOUNT_MANAGERS_RETRIEVAL_FAILED, false, string.Empty));
            }

            _logger.LogInformation("TMI Admin dashboard details fetched successfully for user ID {UserId}.", userId);
            return Results.Json(new ApiResponse<TmiDashBoardDetailsDto>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        });

    }
}