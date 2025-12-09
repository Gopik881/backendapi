using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Enums;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Features.Clients.Queries.GetPagedClientAccountManagers;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetPagedCompanyByUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Elixir.Admin.API.Endpoints;

public static class CompanyUserModule
{
    private static ILogger _logger;
    public static void RegisterCompanyUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Get the ILoggerFactory from the service provider
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();

        // Create a logger for state
        _logger ??= loggerFactory.CreateLogger("CompanyUserApiRoutes");

        endpoints.MapGet("api/v{version}/accountmanagers/companies/{userId}/{pageNumber}/{pageSize}", [Authorize] async (int version, int userId, int groupId, string? searchTerm, int pageNumber, int pageSize, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching companies for account manager. UserId: {UserId}, PageNumber: {PageNumber}, PageSize: {PageSize}", userId, pageNumber, pageSize);
            try
            {
                searchTerm = searchTerm?.Trim() ?? string.Empty;
                pageNumber = pageNumber <= 0 ? 1 : pageNumber;
                pageSize = pageSize <= 0 ? 1 : pageSize;
                var query = new GetPagedCompanyByUsersQuery(userId, (int)UserGroupRoles.AccountManager, AppConstants.ACCOUNT_MANAGERS, searchTerm, pageNumber, pageSize);
                var result = await mediator.Send(query);
                if (result.Data.Count < 1)
                {
                    _logger.LogInformation("No companies found for account manager. UserId: {UserId}", userId);
                    return Results.Json(new ApiResponse<PaginatedResponse<CompanyBasicInfoDto>>(200, AppConstants.ErrorCodes.COMPANY_NO_COMPANY_FOUND, false, result));
                }

                _logger.LogInformation("Companies fetched successfully for account manager. UserId: {UserId}, Count: {Count}", userId, result.Data.Count);
                return Results.Json(new ApiResponse<PaginatedResponse<CompanyBasicInfoDto>>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
            }
            catch
            {
                _logger.LogError("Error occurred while fetching companies for account manager. UserId: {UserId}", userId);
                return Results.Json(new ApiResponse<string>(500, AppConstants.ErrorCodes.COMPANY_RETRIEVAL_FAILED, false, string.Empty));
            }
        });

        endpoints.MapGet("api/v{version}/accountmanagers/clients/{userId}/{pageNumber}/{pageSize}", [Authorize] async (int version, int userId, int groupId, string? searchTerm, int pageNumber, int pageSize, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching clients for account manager. UserId: {UserId}, PageNumber: {PageNumber}, PageSize: {PageSize}", userId, pageNumber, pageSize);
            try
            {
                searchTerm = searchTerm?.Trim() ?? string.Empty;
                pageNumber = pageNumber <= 0 ? 1 : pageNumber;
                pageSize = pageSize <= 0 ? 1 : pageSize;
                var query = new GetPagedClientsByUsersQuery(userId, groupId, AppConstants.ACCOUNT_MANAGERS, searchTerm, pageNumber, pageSize);
                var result = await mediator.Send(query);
                if (result.Data.Count < 1)
                {
                    _logger.LogInformation("No clients found for account manager. UserId: {UserId}", userId);
                    return Results.Json(new ApiResponse<PaginatedResponse<ClientBasicInfoDto>>(200, AppConstants.ErrorCodes.CLIENT_NO_CLIENT_FOUND, false, result));
                }

                _logger.LogInformation("Clients fetched successfully for account manager. UserId: {UserId}, Count: {Count}", userId, result.Data.Count);
                return Results.Json(new ApiResponse<PaginatedResponse<ClientBasicInfoDto>>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
            }
            catch
            {
                _logger.LogError("Error occurred while fetching clients for account manager. UserId: {UserId}", userId);
                return Results.Json(new ApiResponse<string>(500, AppConstants.ErrorCodes.CLIENT_RETRIEVAL_FAILED, false, string.Empty));
            }
        });

        endpoints.MapGet("api/v{version}/checkers/companies/{userId}/{pageNumber}/{pageSize}", [Authorize] async (int version, int userId, int groupId, string? searchTerm, int pageNumber, int pageSize, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching companies for checker. UserId: {UserId}, PageNumber: {PageNumber}, PageSize: {PageSize}", userId, pageNumber, pageSize);
            try
            {
                searchTerm = searchTerm?.Trim() ?? string.Empty;
                pageNumber = pageNumber <= 0 ? 1 : pageNumber;
                pageSize = pageSize <= 0 ? 1 : pageSize;
                var query = new GetPagedCompanyByUsersQuery(userId, (int)UserGroupRoles.Checker, AppConstants.CHECKERS, searchTerm, pageNumber, pageSize);
                var result = await mediator.Send(query);
                if (result.Data.Count < 1)
                {
                    _logger.LogInformation("No companies found for checker. UserId: {UserId}", userId);
                    return Results.Json(new ApiResponse<PaginatedResponse<CompanyBasicInfoDto>>(404, AppConstants.ErrorCodes.COMPANY_NO_COMPANY_FOUND, false, result));
                }

                _logger.LogInformation("Companies fetched successfully for checker. UserId: {UserId}, Count: {Count}", userId, result.Data.Count);
                return Results.Json(new ApiResponse<PaginatedResponse<CompanyBasicInfoDto>>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
            }
            catch
            {
                _logger.LogError("Error occurred while fetching companies for checker. UserId: {UserId}", userId);
                return Results.Json(new ApiResponse<string>(500, AppConstants.ErrorCodes.COMPANY_RETRIEVAL_FAILED, false, string.Empty));
            }
        });

        endpoints.MapGet("api/v{version}/elixirusers/companies/{userId}/{groupId}/{pageNumber}/{pageSize}", [Authorize] async (int version, int userId, int groupId, string? searchTerm, int pageNumber, int pageSize, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching companies for elixir user. UserId: {UserId}, GroupId: {GroupId}, PageNumber: {PageNumber}, PageSize: {PageSize}", userId, groupId, pageNumber, pageSize);
            try
            {
                searchTerm = searchTerm?.Trim() ?? string.Empty;
                pageNumber = pageNumber <= 0 ? 1 : pageNumber;
                pageSize = pageSize <= 0 ? 1 : pageSize;
                var query = new GetPagedCompanyByUsersQuery(userId, groupId, AppConstants.ELIXIR_USERS, searchTerm, pageNumber, pageSize);
                var result = await mediator.Send(query);
                if (result.Data.Count < 1)
                {
                    _logger.LogInformation("No companies found for elixir user. UserId: {UserId}, GroupId: {GroupId}", userId, groupId);
                    return Results.Json(new ApiResponse<PaginatedResponse<CompanyBasicInfoDto>>(404, AppConstants.ErrorCodes.COMPANY_NO_COMPANY_FOUND, false, result));
                }

                _logger.LogInformation("Companies fetched successfully for elixir user. UserId: {UserId}, GroupId: {GroupId}, Count: {Count}", userId, groupId, result.Data.Count);
                return Results.Json(new ApiResponse<PaginatedResponse<CompanyBasicInfoDto>>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
            }
            catch
            {
                _logger.LogError("Error occurred while fetching companies for elixir user. UserId: {UserId}, GroupId: {GroupId}", userId, groupId);
                return Results.Json(new ApiResponse<string>(500, AppConstants.ErrorCodes.COMPANY_RETRIEVAL_FAILED, false, string.Empty));
            }
        });
    
        }
}