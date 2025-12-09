using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Enums;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Clients.Commands.CreateClient.CompositeClient;
using Elixir.Application.Features.Clients.Commands.DeleteClient.DeleteClientComposite;
using Elixir.Application.Features.Clients.Commands.UpdateClient.UpdateClient.UpdateClientCompositeCommand;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Features.Clients.Queries.GetClientAccountManagers;
using Elixir.Application.Features.Clients.Queries.GetClientById.GetClientCompositeCommand;
using Elixir.Application.Features.Clients.Queries.GetClientUnmappedCompanies;
using Elixir.Application.Features.Clients.Queries.GetPagedClientCompanies;
using Elixir.Application.Features.Clients.Queries.GetPagedClientsWithFilters;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Infrastructure.Persistance.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Elixir.Admin.API.Endpoints;

public static class ClientModule
{
    private static ILogger _logger;
    public static void RegisterClientEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        _logger ??= loggerFactory.CreateLogger("Client");

        // Pseudocode:
        // 1. Extract client name and code from createClientDto.
        // 2. Check if client name exists. If yes, return error response with message "Client name already exists."
        // 3. If client code is provided, check if it exists. If yes, return error response with message "Client code already exists."
        // 4. Extract company IDs from clientCompanyMappingDtos.
        // 5. If company IDs exist, check if any are already mapped to a client. If yes, return error response with message "One or more companies are already mapped to another client."
        // 6. If all validations pass, continue with the rest of the logic.

        endpoints.MapPost("api/v{version}/clients", [Authorize] async (int version, CreateClientDto createClientDto, IMediator mediator, ClaimsPrincipal principal, IValidator<CompositeClientCommand> validator, IClientsRepository clientsRepository, ISuperUsersRepository superUsersRepository) =>
        {
            _logger.LogInformation("Received request to create client.");
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            bool IsSuperUser = String.Equals(principal.FindFirst(AppConstants.IS_SUPER_ADMIN)?.Value, AppConstants.IS_SUPER_ADMIN_TRUE);
            if (createClientDto == null || createClientDto.ClientInfo == null)
            {
                _logger.LogError("CreateClientDto or ClientInfo is null.");
                return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.EMAIL_INVALID_REQUEST, false, string.Empty));
            }
            var companyNameWithClientCode = await clientsRepository.GetCompanyNameByClientCodeAsync(createClientDto.ClientInfo.ClientCode);
            if (!string.IsNullOrEmpty(companyNameWithClientCode))
            {
                _logger.LogError("Client code already exists for company: {CompanyName}", companyNameWithClientCode);
                return Results.Json(new ApiResponse<string>(
                    409,
                    $"{AppConstants.ErrorCodes.CLIENT_CODE_ALREADY_EXIST} for company '{companyNameWithClientCode}'",
                    false,
                    $"Client code already exists for company '{companyNameWithClientCode}'"
                ));
            }

            // Check if ClientAdmin email is reserved (matches a superuser email)
            if (createClientDto.ClientAdminInfo != null && !string.IsNullOrWhiteSpace(createClientDto.ClientAdminInfo.Email))
            {
                var superUserEmail = await superUsersRepository.GetSuperUserEmailAsync((int)Roles.SuperAdmin);
                if (!string.IsNullOrEmpty(superUserEmail) &&
                    string.Equals(createClientDto.ClientAdminInfo.Email.Trim(), superUserEmail.Trim(), StringComparison.OrdinalIgnoreCase) ||
                    createClientDto.ClientContactInfo != null && createClientDto.ClientContactInfo.Any(c =>
                                !string.IsNullOrWhiteSpace(c.Email) &&
                                string.Equals(c.Email.Trim(), superUserEmail.Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    _logger.LogError("Company admin email is reserved and cannot be used: {Email}", createClientDto.ClientAdminInfo.Email);
                    return Results.Json(new ApiResponse<string>(409, $"Client Admin/ClientContact Details Email '{createClientDto.ClientAdminInfo.Email}' is reserved and cannot be used.", false, $"Client Admin/ClientContact Details Email '{createClientDto.ClientAdminInfo.Email}' is reserved and cannot be used."));
                }
            }

            // 1. Check if client name already exists
            if (await clientsRepository.ExistsWithClientNameAsync(createClientDto.ClientInfo.ClientName))
            {
                _logger.LogError("Client name already exists: {ClientName}", createClientDto.ClientInfo.ClientName);
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.CLIENT_NAME_ALREADY_EXIST, false, string.Empty));
            }

            // 2. Check if client code already exists (if provided)
            if (!string.IsNullOrEmpty(createClientDto.ClientInfo.ClientCode) &&
                await clientsRepository.ExistsWithClientCodeAsync(createClientDto.ClientInfo.ClientCode))
            {
                _logger.LogError("Client code already exists: {ClientCode}", createClientDto.ClientInfo.ClientCode);
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.CLIENT_CODE_ALREADY_EXIST, false, string.Empty));
            }

            var clientId = createClientDto.ClientInfo.ClientId;
            var command = new CompositeClientCommand(userId, clientId, createClientDto, IsSuperUser);

            _logger.LogInformation("Validating CompositeClientCommand.");
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed for CompositeClientCommand.");
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }

            _logger.LogInformation("Sending CompositeClientCommand to mediator.");
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogError("Failed to create client.");
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.CLIENT_DUPLICATEDATA_FOUND, false, string.Empty));
            }

            _logger.LogInformation("Client created successfully.");
            return Results.Json(new ApiResponse<string>(200, AppConstants.ErrorCodes.COMMON_MASTER_LIST_REFRESH_SUCCESS, true, string.Empty));
        });

        endpoints.MapGet("api/v{version}/clients/{pageNumber}/{pageSize}", [Authorize] async (int version, string? searchTerm, int pageNumber, int pageSize, IMediator mediator, ClaimsPrincipal principal) =>
        {
            _logger.LogInformation("Fetching paged clients for API version {Version}. Page: {PageNumber}, Size: {PageSize}, SearchTerm: {SearchTerm}", version, pageNumber, pageSize, searchTerm);
            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var query = new GetPagedClientsWithFiltersQuery(pageNumber, pageSize, searchTerm);
            var result = await mediator.Send(query);

            _logger.LogInformation("Clients fetched successfully for API version {Version}.", version);
            return Results.Json(new ApiResponse<PaginatedResponse<ClientDto>>(200, AppConstants.ErrorCodes.USER_GROUPS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/clients/{clientId}/companies/{pageNumber}/{pageSize}", [Authorize] async (int version, int clientId, int pageNumber, int pageSize, string? searchTerm, IMediator mediator, ClaimsPrincipal principal) =>
        {
            _logger.LogInformation("Fetching paged companies for client {ClientId}, API version {Version}. Page: {PageNumber}, Size: {PageSize}, SearchTerm: {SearchTerm}", clientId, version, pageNumber, pageSize, searchTerm);
            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var query = new GetPagedClientCompaniesQuery(clientId, pageNumber, pageSize, searchTerm);
            var result = await mediator.Send(query);

            if (result == null || result.Metadata == null || result.Metadata.PageSize == 0 || result.Metadata.CurrentPage == 0)
            {
                _logger.LogInformation("Companies not found for client {ClientId}, API version {Version} with search term '{SearchTerm}'.", clientId, version, searchTerm);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.COMPANY_NO_COMPANY_FOUND, false, string.Empty));
            }

            _logger.LogInformation("Companies fetched successfully for client {ClientId}, API version {Version}.", clientId, version);
            return Results.Json(new ApiResponse<PaginatedResponse<CompanyDto>>(200, AppConstants.ErrorCodes.USER_GROUPS_FETCHED_SUCCESSFULLY, true, result, null, result.Metadata));
        });

        endpoints.MapGet("api/v{version}/clients/account-managers", [Authorize] async (int version, IMediator mediator, ClaimsPrincipal principal) =>
        {
            _logger.LogInformation("Received request to get client account managers.");
            var query = new GetClientAccountManagersQuery();
            _logger.LogInformation("Sending GetClientAccountManagersQuery to mediator.");
            var result = await mediator.Send(query);

            if (result == null)
            {
                _logger.LogError("No client account managers found.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.CLIENT_ACCOUNT_MANAGERS_RETRIEVAL_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Client account managers fetched successfully.");
            return Results.Json(new ApiResponse<IEnumerable<ClientGroupswithAccountManagersDto>>(200, AppConstants.ErrorCodes.USER_GROUPS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/clients/unmapped-companies", [Authorize] async (int version, IMediator mediator, ClaimsPrincipal principal) =>
        {
            _logger.LogInformation("Received request to get unmapped companies.");
            var query = new GetClientUnmappedCompaniesQuery();
            _logger.LogInformation("Sending GetClientUnmappedCompaniesQuery to mediator.");
            var result = await mediator.Send(query);

            if (result == null)
            {
                _logger.LogError("No unmapped companies found.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.CLIENT_UNMAPPED_COMPANIES_RETRIEVAL_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Unmapped companies fetched successfully.");
            return Results.Json(new ApiResponse<IEnumerable<ClientUnmappedCompaniesDto>>(200, AppConstants.ErrorCodes.USER_GROUPS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapDelete("api/v{version}/clients/{clientId}", [Authorize] async (int version, int clientId, IMediator mediator, ClaimsPrincipal principal) =>
        {
            _logger.LogInformation("Received request to delete client with ClientId: {ClientId}", clientId);
            var command = new DeleteClientCompositeCommand(clientId);
            _logger.LogInformation("Sending DeleteClientCompositeCommand to mediator.");
            var result = await mediator.Send(command);

            if (!result)
            {
                _logger.LogError("Failed to delete client with ClientId: {ClientId}", clientId);
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.CLIENT_DELETION_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Client deleted successfully with ClientId: {ClientId}", clientId);
            return Results.Json(new ApiResponse<string>(200, AppConstants.ErrorCodes.USER_GROUP_USERS_ADD_MAPPING_SUCCESS, true, string.Empty));
        });

        endpoints.MapPut("api/v{version}/clients/{clientId}", [Authorize] async (int version, int clientId, CreateClientDto updateClientDto, IMediator mediator, ClaimsPrincipal principal, IValidator<UpdateClientCompositeCommand> validator, IClientsRepository clientsRepository, ISuperUsersRepository superUsersRepository) =>
        {
            _logger.LogInformation("Received request to update client with ClientId: {ClientId}", clientId);
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            bool IsSuperUser = String.Equals(principal.FindFirst(AppConstants.IS_SUPER_ADMIN)?.Value, AppConstants.IS_SUPER_ADMIN_TRUE);
            if (updateClientDto == null || updateClientDto.ClientInfo == null)
            {
                _logger.LogError("UpdateClientDto or ClientInfo is null.");
                return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.EMAIL_INVALID_REQUEST, false, string.Empty));
            }

            var companyNameWithClientCode = await clientsRepository.GetCompanyNameByClientCodeAsync(updateClientDto.ClientInfo.ClientCode);
            if (!string.IsNullOrEmpty(companyNameWithClientCode))
            {
                _logger.LogError("Client code already exists for company: {CompanyName}", companyNameWithClientCode);
                return Results.Json(new ApiResponse<string>(
                    409,
                    $"{AppConstants.ErrorCodes.CLIENT_CODE_ALREADY_EXIST} for company '{companyNameWithClientCode}'",
                    false,
                    $"Client code already exists for company '{companyNameWithClientCode}'"
                ));
            }

            // Check if ClientAdmin email is reserved (matches a superuser email)
            if (updateClientDto.ClientAdminInfo != null && !string.IsNullOrWhiteSpace(updateClientDto.ClientAdminInfo.Email))
            {
                var superUserEmail = await superUsersRepository.GetSuperUserEmailAsync((int)Roles.SuperAdmin);
                if (!string.IsNullOrEmpty(superUserEmail) &&
                    string.Equals(updateClientDto.ClientAdminInfo.Email.Trim(), superUserEmail.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogError("Company admin email is reserved and cannot be used: {Email}", updateClientDto.ClientAdminInfo.Email);
                    return Results.Json(new ApiResponse<string>(409, $"Client Admin Email '{updateClientDto.ClientAdminInfo.Email}' is reserved and cannot be used.", false, $"Client Admin Email '{updateClientDto.ClientAdminInfo.Email}' is reserved and cannot be used."));
                }
            }

            var clientInfo = updateClientDto.ClientInfo;
            // Fetch client name based on clientId
            var clientDetails = await clientsRepository.GetClientDetailsByIdAsync(clientId);
            if (clientDetails == null)
            {
                _logger.LogError("No client found for ClientId: {ClientId}", clientId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.CLIENT_DETAILS_NOT_FOUND, false, string.Empty));
            }
            var clientName = clientDetails.ClientName;
            // 2. Check if client code already exists (excluding current client)
            // 1. Get all client IDs with the same client name (excluding the current client)
            List<int> clientIdsWithSameName = await clientsRepository.GetListOfClientIdsByCompanyNameAsync(clientName);

            // 1. Check if client name already exists (excluding current client)
            if (await clientsRepository.ExistsWithClientNameIdsAsync(clientInfo.ClientName, clientIdsWithSameName))
            {
                _logger.LogError("Client name already exists: {ClientName}", clientName);
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.CLIENT_NAME_ALREADY_EXIST, false, string.Empty));
            }

            // 2. Check if client code exists for any client not in the above list
            if (!string.IsNullOrEmpty(clientInfo.ClientCode))
            {
                var clientCodeExists = await clientsRepository.ExistsWithUpdateClientCodeAsync(clientInfo.ClientCode, clientIdsWithSameName);
                // Only treat as duplicate if the found client is not in the ignore list
                if (clientCodeExists)
                {
                    _logger.LogError("Client code already exists: {ClientCode}", clientInfo.ClientCode);
                    return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.CLIENT_CODE_ALREADY_EXIST, false, string.Empty));
                }
            }

            var command = new UpdateClientCompositeCommand(userId, clientId, updateClientDto, IsSuperUser);

            _logger.LogInformation("Validating UpdateClientCompositeCommand.");
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed for UpdateClientCompositeCommand.");
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }

            _logger.LogInformation("Sending UpdateClientCompositeCommand to mediator.");
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogError("Failed to update client with ClientId: {ClientId}", clientId);
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.CLIENT_UPDATE_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Client updated successfully with ClientId: {ClientId}", clientId);
            return Results.Json(new ApiResponse<string>(200, AppConstants.ErrorCodes.COMPANY_LIST_UPDATE_SUCCESS, true, string.Empty));
        });

        endpoints.MapGet("api/v{version}/clients/{clientId}", [Authorize] async (int version, int clientId, IMediator mediator, ClaimsPrincipal principal) =>
        {
            _logger.LogInformation("Received request to get client data for ClientId: {ClientId}", clientId);
            var query = new GetClientCompositeCommand(clientId);
            _logger.LogInformation("Sending GetClientCompositeCommand to mediator.");
            var result = await mediator.Send(query);

            if (result == null)
            {
                _logger.LogError("No client data found for ClientId: {ClientId}", clientId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.CLIENT_DETAILS_FETCHED_UNSUCCESSFULLY, false, string.Empty));
            }

            _logger.LogInformation("Composite client data fetched successfully for ClientId: {ClientId}", clientId);
            return Results.Json(new ApiResponse<CreateClientDto>(200, AppConstants.ErrorCodes.USER_GROUPS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/modulemappings/clientpopup/{clientId}", [Authorize] async (int version, int clientId, IMediator mediator) =>
        {
            _logger.LogInformation("Received request to get client popup details for ClientId: {ClientId}", clientId);
            var result = await mediator.Send(new GetCleintpopupDetailsQuery(clientId));
            if (result == null)
            {
                _logger.LogError("No client popup details found for ClientId {ClientId}.", clientId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.CLIENT_DETAILS_NOT_FOUND, false, string.Empty));
            }

            _logger.LogInformation("Client popup details retrieved successfully for ClientId: {ClientId}", clientId);
            return Results.Json(new ApiResponse<ClientPopupDetailsDto>(200, AppConstants.ErrorCodes.USER_GROUPS_FETCHED_SUCCESSFULLY, true, result));
        });
    }
}