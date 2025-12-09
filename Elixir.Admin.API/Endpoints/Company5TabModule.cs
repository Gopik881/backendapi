using Elixir.Application.Common.Constants;
using Elixir.Application.Common.DTOs;
using Elixir.Application.Common.Enums;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Features.Company.Commands.ApproveCompany.CompositeApproveCompany5Tab;
using Elixir.Application.Features.Company.Commands.CreateCompany.CompositeCompany5Tab;
using Elixir.Application.Features.Company.Commands.NotifyCompanyAdmin;
using Elixir.Application.Features.Company.Commands.RejectCompany;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.CompositeWithdrawCompany5Tab;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabCustomUsers;
using Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetCompositeCompany5Tab;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistoryByVersionNumber.Company5TabHistoryByVersionCompositeCommandHandler;
using Elixir.Application.Features.Company.Queries.GetCompany5TabOnboardingHistoryByCompanyId;
using Elixir.Application.Features.Company.Queries.GetCompany5TabUserGroupUsers;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Elixir.Infrastructure.Persistance.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;

namespace Elixir.Admin.API.Endpoints;

public static class Company5TabModule
{
    private static ILogger _logger;

    public static void RegisterCompany5TabEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        _logger ??= loggerFactory.CreateLogger("Company5TabApiRoutes");

        endpoints.MapPost("api/v{version}/company5tabs/{companyId}", [Authorize] async (int version, int companyId, Company5TabDto company5TabDto, IMediator mediator, ClaimsPrincipal principal, IValidator<CompositeCompany5TabCommand> validator, ICompanyOnboardingStatusRepository _companyOnboardingStatusRepository, ICompaniesRepository _compniesRepository, IUsersRepository _usersRepository, ICompanyAdminUsersRepository _companyAdminRepository, IAccountHistoryRepository _accountHistoryRepository, ICryptoService _cryptoService, ISuperUsersRepository superUsersRepository, IClientsRepository clientsRepository) =>
        {
            _logger.LogInformation("Received request to create Company5Tab for CompanyId: {CompanyId}", companyId);
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            if (company5TabDto == null || company5TabDto.Company5TabCompanyDto == null || company5TabDto.Company5TabCompanyDto.CompanyId != companyId)
            {
                _logger.LogError("Company5TabDto or Company5TabCompanyDto is null.");
                return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.COMPANY_REQUEST_DATA_INVALID, false, string.Empty));
            }

            // Check that CompanyCode is not present in Clients table as ClientCode
            if (!string.IsNullOrEmpty(company5TabDto.Company5TabCompanyDto.CompanyCode))
            {
                // Assuming _compniesRepository.ExistsWithCompanyCodeAsync checks Companies table,
                // and you need to check Clients table for ClientCode.
                // You need a repository method like IClientsRepository.ExistsWithClientCodeAsync(string clientCode)
                // For demonstration, let's assume you have IClientsRepository injected as _clientsRepository.

                //var clientsRepository = endpoints.ServiceProvider.GetRequiredService<IClientsRepository>();
                bool clientCodeExists = await clientsRepository.ExistsWithClientCodeAsync(company5TabDto.Company5TabCompanyDto.CompanyCode);
                if (clientCodeExists)
                {
                    _logger.LogError("CompanyCode '{CompanyCode}' already exists as ClientCode in Clients table.", company5TabDto.Company5TabCompanyDto.CompanyCode);
                    return Results.Json(new ApiResponse<string>(
                        400,
                        "CompanyCode should not exist as ClientCode in Clients.",
                        false,
                        string.Empty
                    ));
                }
            }

            // Check if ClientAdmin email is reserved (matches a superuser email)
            if (company5TabDto.Company5TabCompanyAdminDto.CompanyAdminEmailId != null && !string.IsNullOrWhiteSpace(company5TabDto.Company5TabCompanyAdminDto.CompanyAdminEmailId))
            {
                var superUserEmail = await superUsersRepository.GetSuperUserEmailAsync((int)Roles.SuperAdmin);
                if (!string.IsNullOrEmpty(superUserEmail) &&
                    string.Equals(company5TabDto.Company5TabCompanyAdminDto.CompanyAdminEmailId.Trim(), superUserEmail.Trim(), StringComparison.OrdinalIgnoreCase) ||
                    company5TabDto.Company5TabEscalationContactDto != null && company5TabDto.Company5TabEscalationContactDto.Any(c =>
                                !string.IsNullOrWhiteSpace(c.EmailId) &&
                                string.Equals(c.EmailId.Trim(), superUserEmail.Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    _logger.LogError("Company admin/Escalation Contacts email is reserved and cannot be used: {Email}", superUserEmail);
                    return Results.Json(new ApiResponse<string>(409, $"Company admin/Escalation Contacts Email '{superUserEmail}' is reserved and cannot be used.", false, $"Company admin/Escalation Contacts Email '{superUserEmail}' is reserved and cannot be used."));
                }
            }

            // Custom duplicate checks
            var errorMessages = new List<string>();
            var companyDto = company5TabDto.Company5TabCompanyDto;
            var adminDto = company5TabDto.Company5TabCompanyAdminDto;
            var accountDto = company5TabDto.Company5TabAccountDto;

            // Get the current onboarding status
            var onboardingStatus = await _companyOnboardingStatusRepository.GetCompanyOnBoardingStatus(companyDto.CompanyId);

            var companyEntity = await _compniesRepository.GetCompanyByIdAsync(companyDto.CompanyId);
            bool? isUnderEdit = companyEntity?.IsUnderEdit;
            if ((isUnderEdit ?? false))
            {
                _logger.LogWarning("Data is already submitted for CompanyId: {CompanyId}", companyId);
                return Results.Json(new ApiResponse<string>(
                    409, // Conflict
                    AppConstants.ErrorCodes.UPDATE_COMPANY_5TAB_FAILED,
                    false,
                    string.Empty
                ));
            }
            else if (onboardingStatus == AppConstants.ONBOARDING_STATUS_NEW)
            {
                if (await _companyOnboardingStatusRepository.IsCompanyOnboardingStatusDataExistsAsync(companyDto.CompanyId))
                    errorMessages.Add("Company onboarding status data already exists.");

                if (!string.IsNullOrEmpty(companyDto.CompanyCode) && await _compniesRepository.ExistsWithCompanyCodeAsync(companyDto.CompanyCode))
                    errorMessages.Add("Company code already exists.");

                if (!string.IsNullOrEmpty(adminDto?.CompanyAdminEmailId) &&
                    (await _usersRepository.ExistsUserByEmailAsync(adminDto.CompanyAdminEmailId, _cryptoService.GenerateIntegerHashForString(adminDto.CompanyAdminEmailId)) ||
                     await _companyAdminRepository.ExistsCompanyAdminByEmailAsync(adminDto.CompanyAdminEmailId)))
                {
                    errorMessages.Add("Company admin email already exists.");
                }

                if (!string.IsNullOrEmpty(accountDto?.Pan) && await _accountHistoryRepository.IsPanExistsAsync(accountDto.Pan, companyDto.CompanyId))
                    errorMessages.Add("PAN already exists.");

                if (!string.IsNullOrEmpty(accountDto?.Tan) && await _accountHistoryRepository.IsTanExistsAsync(accountDto.Tan, companyDto.CompanyId))
                    errorMessages.Add("TAN already exists.");

                if (!string.IsNullOrEmpty(accountDto?.Gstn) && await _accountHistoryRepository.IsGstInExistsAsync(accountDto.Gstn, companyDto.CompanyId))
                    errorMessages.Add("GSTIN already exists.");

                if (!string.IsNullOrEmpty(accountDto?.ContractId) && await _accountHistoryRepository.IsContractIdExistsAsync(accountDto.ContractId, companyDto.CompanyId))
                    errorMessages.Add("ContractId already exists.");
            }
            else if (onboardingStatus == AppConstants.ONBOARDING_STATUS_APPROVED || onboardingStatus == AppConstants.ONBOARDING_STATUS_REJECTED)
            {
                // Only validate for new values, skip existing IDs for validation
                if (!string.IsNullOrEmpty(companyDto.CompanyCode))
                {
                    var existingCompanyId = await _compniesRepository.FindCompanyByCodeAsync(companyDto.CompanyCode);
                    if (existingCompanyId > 0 && existingCompanyId != companyDto.CompanyId)
                        errorMessages.Add("Company code already exists.");
                }

                if (!string.IsNullOrEmpty(adminDto?.CompanyAdminEmailId))
                {
                    //int existingCompanyId = 0;
                    //if (!string.IsNullOrEmpty(companyDto.CompanyCode))
                    //{
                    //    existingCompanyId = await _compniesRepository.FindCompanyByCodeAsync(companyDto.CompanyCode);
                    //}
                    var emailHash = _cryptoService.GenerateIntegerHashForString(adminDto.CompanyAdminEmailId);
                    var user = await _usersRepository.GetUserByEmailAsync(adminDto.CompanyAdminEmailId, emailHash);
                    var CompanyAdmin = await _companyAdminRepository.UpdateExistsCompanyAdminByEmailAsync(adminDto.CompanyAdminEmailId, companyDto.CompanyId);
                    if (user != null || CompanyAdmin)
                        errorMessages.Add("Company admin email already exists.");
                }

                if (!string.IsNullOrEmpty(accountDto?.Pan))
                {
                    var panExists = await _accountHistoryRepository.IsPanExistsAsync(accountDto.Pan, companyDto.CompanyId);
                    if (panExists)
                        errorMessages.Add("PAN already exists.");
                }

                if (!string.IsNullOrEmpty(accountDto?.Tan))
                {
                    var tanExists = await _accountHistoryRepository.IsTanExistsAsync(accountDto.Tan, companyDto.CompanyId);
                    if (tanExists)
                        errorMessages.Add("TAN already exists.");
                }

                if (!string.IsNullOrEmpty(accountDto?.Gstn))
                {
                    var gstnExists = await _accountHistoryRepository.IsGstInExistsAsync(accountDto.Gstn, companyDto.CompanyId);
                    if (gstnExists)
                        errorMessages.Add("GSTIN already exists.");
                }

                if (!string.IsNullOrEmpty(accountDto?.ContractId))
                {
                    var contractIdExists = await _accountHistoryRepository.IsContractIdExistsAsync(accountDto.ContractId, companyDto.CompanyId);
                    if (contractIdExists)
                        errorMessages.Add("ContractId already exists.");
                }
            }
            else if (onboardingStatus == AppConstants.ONBOARDING_STATUS_PENDING)
            {
                _logger.LogWarning("Data is already submitted for CompanyId: {CompanyId}", companyId);
                return Results.Json(new ApiResponse<string>(
                    409, // Conflict
                    AppConstants.ErrorCodes.UPDATE_COMPANY_5TAB_FAILED,
                    false,
                    string.Empty
                ));
            }
            if (errorMessages.Any())
            {
                _logger.LogError("Duplicate data found: {Errors}", string.Join(", ", errorMessages));
                return Results.Json(new ApiResponse<List<string>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, errorMessages));
            }

            var command = new CompositeCompany5TabCommand(userId, companyId, company5TabDto);
            _logger.LogInformation("Validating CompositeCompany5TabCommand for CompanyId: {CompanyId}", companyId);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed for CompositeCompany5TabCommand for CompanyId: {CompanyId}", companyId);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }

            _logger.LogInformation("Sending CompositeCompany5TabCommand to mediator for CompanyId: {CompanyId}", companyId);
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogError("Failed to create Company5Tab.");
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.UPDATE_COMPANY_5TAB_FAILED, false, string.Empty));
            }
            _logger.LogInformation("Company5Tab created successfully for CompanyId: {CompanyId}", companyId);
            return Results.Json(new ApiResponse<string>(200, AppConstants.ErrorCodes.COMPANY_LIST_UPDATE_SUCCESS, true, string.Empty));
        });

        endpoints.MapPost("api/v{version}/company5tabs/approve/{companyId}", [Authorize] async (int version, int companyId, Company5TabDto company5TabDto, IMediator mediator, ClaimsPrincipal principal, IValidator<CompositeCompany5TabCommand> validator, ISuperUsersRepository superUsersRepository) =>
        {
            _logger.LogInformation("Received request to approve/reject Company5Tab for CompanyId: {CompanyId}", companyId);
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            if (company5TabDto == null || company5TabDto.Company5TabCompanyDto == null && company5TabDto.Company5TabCompanyDto.CompanyId != companyId)
            {
                _logger.LogError("Company5TabDto or Company5TabCompanyDto is null.");
                return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.COMPANY_REQUEST_DATA_INVALID, false, string.Empty));
            }
            if (company5TabDto.isApproved)
            {
                _logger.LogInformation("Approving Company5Tab for CompanyId: {CompanyId}", companyId);
                var command = new CompositeApproveCompany5TabCommand(userId, companyId, company5TabDto);
                var result = await mediator.Send(command);
                if (!result)
                {
                    _logger.LogError("Failed to approve Company.");
                    return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.APPROVAL_CHECK_FAILED, false, string.Empty));
                }
                _logger.LogInformation("Company Approved successfully for CompanyId: {CompanyId}", companyId);
                return Results.Json(new ApiResponse<string>(200, AppConstants.ErrorCodes.COMPANY_LIST_UPDATE_SUCCESS, true, string.Empty));
            }
            else
            {
                _logger.LogInformation("Rejecting Company5Tab for CompanyId: {CompanyId}", companyId);
                var command = new RejectCompany5TabCommand(companyId, userId, company5TabDto.RejectedReason);
                var result = await mediator.Send(command);

                if (!result)
                {
                    _logger.LogError("Failed to reject Company.");
                    return Results.Json(new ApiResponse<string>(409, $"{AppConstants.ErrorCodes.APPROVAL_CHECK_FAILED + company5TabDto.Company5TabCompanyDto.CompanyName}", false, string.Empty));
                }
                _logger.LogInformation("Company Rejected successfully for CompanyId: {CompanyId}", companyId);
                return Results.Json(new ApiResponse<string>(200, $"{AppConstants.ErrorCodes.APPROVAL_CHECK_REJECTED + company5TabDto.Company5TabCompanyDto.CompanyName}", true, string.Empty));
            }

        });

        endpoints.MapGet("api/v{version}/company5tabs/{companyId}/{IsPrevious}", [Authorize] async (int version, int companyId, bool IsPrevious, IMediator mediator, ClaimsPrincipal principal) =>
        {
            _logger.LogInformation("Fetching Company5Tab data for CompanyId: {CompanyId}, IsPrevious: {IsPrevious}", companyId, IsPrevious);
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            var query = new GetCompositeCompany5TabCommand(companyId, userId, IsPrevious);
            var result = await mediator.Send(query);

            if (result == null)
            {
                _logger.LogError("CompanyData not found for CompanyId: {CompanyId}", companyId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.GET_COMPANY_5TAB_FAILED, false, string.Empty));
            }

            _logger.LogInformation("CompanyData fetched successfully for CompanyId: {CompanyId}", companyId);
            return Results.Json(new ApiResponse<Company5TabDto>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapDelete("api/v{version}/company5tabs/withdraw/{companyId}", [Authorize] async (int version, int companyId, IMediator mediator, ClaimsPrincipal principal) =>
        {
            _logger.LogInformation("Received request to withdraw Company5Tab for CompanyId: {CompanyId}", companyId);
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            var command = new CompositeWithDrawCompany5TabCommand(companyId, userId);
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogError("Failed to withdraw Company5Tab for CompanyId: {CompanyId}", companyId);
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.WITHDRAW_COMPANY_VERSION_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Company withdrawn successfully for CompanyId: {CompanyId}", companyId);
            return Results.Json(new ApiResponse<string>(200, AppConstants.ErrorCodes.COMPANY_LIST_UPDATE_SUCCESS, true, string.Empty));
        });

        endpoints.MapGet("api/v{version}/company5tabs/history/{userId}/{companyId}/{versionNumber}", [Authorize] async (int version, int userId, int companyId, int versionNumber, IMediator mediator, ClaimsPrincipal principal) =>
        {
            _logger.LogInformation("Fetching Company5Tab history for CompanyId: {CompanyId}, VersionNumber: {VersionNumber}, UserId: {UserId}", companyId, versionNumber, userId);
            var query = new GetCompositeCompany5TabHistoryCommand(userId, companyId, versionNumber);
            var result = await mediator.Send(query);

            if (result == null)
            {
                _logger.LogError("Company5Tab history not found for CompanyId: {CompanyId}, VersionNumber: {VersionNumber}", companyId, versionNumber);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.GET_LAST_TWO_VERSIONS_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Company5Tab history fetched successfully for CompanyId: {CompanyId}, VersionNumber: {VersionNumber}", companyId, versionNumber);
            return Results.Json(new ApiResponse<object>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/company5tabs/onboarding-history/{companyId}", [Authorize] async (int version, int companyId, IMediator mediator, ClaimsPrincipal principal) =>
        {
            _logger.LogInformation("Fetching Company5Tab onboarding history for CompanyId: {CompanyId}", companyId);
            var query = new GetCompany5TabOnboardingHistoryByCompanyIdQuery(companyId);
            var result = await mediator.Send(query);

            if (result == null || !result.Any())
            {
                _logger.LogError("Company5Tab onboarding history not found for CompanyId: {CompanyId}", companyId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.GET_5TAB_ONBOARDING_HISTORY_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Company5Tab onboarding history fetched successfully for CompanyId: {CompanyId}", companyId);
            return Results.Json(new ApiResponse<IEnumerable<Company5TabOnboardingHistoryDto>>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/company5tabs/usergroup-users/{companyId}/{groupId}", [Authorize] async (int version, int companyId, int groupId, IMediator mediator, ClaimsPrincipal principal) =>
        {
            _logger.LogInformation("Fetching users for CompanyId: {CompanyId}, GroupId: {GroupId}", companyId, groupId);
            var query = new GetCompany5TabUserGroupUsersQuery(groupId, companyId);
            var result = await mediator.Send(query);

            if (result == null || !result.Any())
            {
                _logger.LogError("No users found for CompanyId: {CompanyId}, GroupId: {GroupId}", companyId, groupId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.GET_5TAB_ONBOARDING_HISTORY_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Users fetched successfully for CompanyId: {CompanyId}, GroupId: {GroupId}", companyId, groupId);
            return Results.Json(new ApiResponse<List<CompanyUserDto>>(200, AppConstants.ErrorCodes.USER_GROUP_USERS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/company5tabs/custom-user-groups/{companyId}", [Authorize] async (int version, int companyId, IMediator mediator, ClaimsPrincipal principal, string? ScreenName = "") =>
        {
            _logger.LogInformation("Fetching custom user groups for CompanyId: {CompanyId}", companyId);
            var query = new GetCompany5TabCustomUserGroupsQuery(companyId, ScreenName);
            var result = await mediator.Send(query);

            if (result == null || !result.Any())
            {
                _logger.LogError("No custom user groups found for CompanyId: {CompanyId}", companyId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.GET_5TAB_ONBOARDING_HISTORY_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Custom user groups fetched successfully for CompanyId: {CompanyId}", companyId);
            return Results.Json(new ApiResponse<List<UserGroupDto>>(200, AppConstants.ErrorCodes.USER_GROUPS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapPost("api/v{version}/notify/company-admin", [Authorize] async (int version, string passwordResetUrl, EmailRequestDto emailRequestDto, IWebHostEnvironment webHostEnvironment, IMediator mediator) =>
        {
            _logger.LogInformation("Received email send request for API version {Version}.", version);

            if (emailRequestDto == null)
            {
                _logger.LogError("Email request DTO is null.");
                return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.EMAIL_INVALID_REQUEST, false, string.Empty));
            }
            string htmlContent = File.ReadAllText($"{webHostEnvironment.ContentRootPath}//EmailTemplates//resetpassword.html"); // Ensure the template file exists
            var result = await mediator.Send(new NotifyCompanyAdminCommand(emailRequestDto, passwordResetUrl, htmlContent));

            if (result == null)
            {
                _logger.LogError("Email sending failed for recipient: {Recipient}.", emailRequestDto.To);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.EMAIL_SENDING_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Email successfully sent to {Recipient}.", emailRequestDto.To);
            return Results.Json(new ApiResponse<EmailResponseDto>(200, AppConstants.ErrorCodes.EMAIL_SEND_SUCCESS, true, result));
        });
    }
}