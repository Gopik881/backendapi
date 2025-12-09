using Elixir.Application.Common.Constants;
using Elixir.Application.Common.DTOs;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Telephone.Queries.GetAllTelephoneCodes;
using Elixir.Application.Features.User.Commands.BulkInsertUsers;
using Elixir.Application.Features.User.Commands.ChangePassword;
using Elixir.Application.Features.User.Commands.DeleteUser;
using Elixir.Application.Features.User.Commands.Login;
using Elixir.Application.Features.User.Commands.ResetPassword;
using Elixir.Application.Features.User.Commands.SendPasswordResetEmail;
using Elixir.Application.Features.User.Commands.UpdateProfile;
using Elixir.Application.Features.User.Commands.UpdateUser;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Features.User.Queries.CheckEmail;
using Elixir.Application.Features.User.Queries.GetMyProfile;
using Elixir.Application.Features.User.Queries.GetPagedNonAdminUsers;
using Elixir.Application.Features.User.Queries.GetUserCriticalGroups;
using Elixir.Application.Features.User.Queries.ResetLinkTokenValidation;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Elixir.Admin.API.Endpoints;

public static class UserModule
{
    private static ILogger _logger;

    public static void RegisterUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Get the ILoggerFactory from the service provider
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();

        // Create a logger for state
        _logger ??= loggerFactory.CreateLogger("UserApiRoutes");

        //User Login
        endpoints.MapPost("api/v{version}/users/login", async (int version, LoginRequestDto loginRequestDto, IMediator mediator, IValidator<LoginCommand> validator) =>
        {
            _logger.LogInformation("Login attempt for user: {UserName}", loginRequestDto.UserName);
            var command = new LoginCommand(loginRequestDto);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Login validation failed for user: {UserName}", loginRequestDto.UserName);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(command);
            if (!result.Success)
            {
                string Msg = string.Empty;
                if (!String.IsNullOrEmpty(result.Message))
                {
                    Msg = result.Message;
                }
                else
                {
                    Msg = AppConstants.ErrorCodes.INVALID_CREDENTIALS;
                }
                _logger.LogError("Login failed for user: {UserName} - {Message}", loginRequestDto.UserName, result.Message);
                return Results.Json(new ApiResponse<string>(401, /*AppConstants.ErrorCodes.INVALID_CREDENTIALS*/ Msg, false, result.Message));
            }
            _logger.LogInformation("Login successful for user: {UserName}", loginRequestDto.UserName);
            return Results.Json(new ApiResponse<LoginResponseDto>(200, AppConstants.ErrorCodes.LOGIN_SUCCESSFUL, true, result));
        });

        //Check Email Exists
        endpoints.MapGet("api/v{version}/users/{email}/exists",  async (int version, string email, IMediator mediator, IValidator<CheckEmailExistsQuery> validator, IUsersRepository usersRepository, ICryptoService cryptoService) =>
        {
            _logger.LogInformation("Checking if email exists: {Email}", email);
            var query = new CheckEmailExistsQuery(email);
            var validationResult = await validator.ValidateAsync(query);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Email existence validation failed for: {Email}", email);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(query);
            if (!result)
            {
                _logger.LogError("Email not found: {Email}", email);
                return Results.Json(new ApiResponse<string>(401, AppConstants.ErrorCodes.EMAIL_NOT_FOUND, false, String.Empty));
            }
            var emailHash = cryptoService.GenerateIntegerHashForString(email);
            var user = await usersRepository.GetUserByEmailAsync(email, emailHash);
            if (user == null)
            {
                _logger.LogError("User not found after successful exists check: {Email}", email);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, string.Empty));
            }
            _logger.LogInformation("Email exists: {Email}", email);
            return Results.Json(new ApiResponse<int>(200, AppConstants.ErrorCodes.EMAIL_EXIST, true, user.Id));
        });

        //Change Password
        endpoints.MapPut("api/v{version}/users/password/change", [Authorize] async (int version, ChangePasswordRequestDto changePasswordRequestDto, IMediator mediator, IValidator<ChangePasswordCommand> validator) =>
        {
            _logger.LogInformation("Change password attempt for user: {UserName}", changePasswordRequestDto.UserName);
            var command = new ChangePasswordCommand(changePasswordRequestDto);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Change password validation failed for user: {UserName}", changePasswordRequestDto.UserName);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(409, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(command);
            if (result.Errors != null && result.Errors.Count != 0 || !result.Status)
            {
                string Msg = string.Empty;
                if (!string.IsNullOrEmpty(result.Message))
                {
                    Msg = result.Message;
                }
                else
                {
                    Msg = AppConstants.ErrorCodes.PASSWORD_CHANGE_FAILED;
                }
                _logger.LogError("Change password failed for user: {UserName}", changePasswordRequestDto.UserName);
                return Results.Json(new ApiResponse<ChangePasswordResponseDto>(400, Msg, result.Status, result));
            }
            _logger.LogInformation("Password changed successfully for user: {UserName}", changePasswordRequestDto.UserName);
            return Results.Json(new ApiResponse<string>(200, AppConstants.ErrorCodes.PASSWORD_CHANGE_SUCCESSFUL, result.Status, String.Empty));
        });

        //Reset Password
        endpoints.MapPut("api/v{version}/users/password/reset", async (int version, ResetPasswordRequestDto resetPasswordRequestDto, IMediator mediator, IValidator<ChangePasswordCommand> validator, ClaimsPrincipal principal) =>
        {
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            _logger.LogInformation("Change password attempt for user: {UserName}", resetPasswordRequestDto.UserName);
            var command = new ResetPasswordCommand(userId, resetPasswordRequestDto);
            var result = await mediator.Send(command);
            if (result.Errors != null && result.Errors.Count != 0 || !result.Status)
            {
                string Msg = string.Empty;
                if (!string.IsNullOrEmpty(result.Message))
                {
                    Msg = result.Message;
                }
                else
                {
                    Msg = AppConstants.ErrorCodes.PASSWORD_CHANGE_FAILED;
                }
                _logger.LogError("Change password failed for user: {UserName}", resetPasswordRequestDto.UserName);
                return Results.Json(new ApiResponse<ResetPasswordResponseDto>(400, Msg, result.Status, result));
            }
            _logger.LogInformation("Password changed successfully for user: {UserName}", resetPasswordRequestDto.UserName);
            return Results.Json(new ApiResponse<string>(200, AppConstants.ErrorCodes.PASSWORD_CHANGE_SUCCESSFUL, result.Status, String.Empty));
        });

        //Validate Reset Link Token
        endpoints.MapGet("api/v{version}/users/password/reset-link/validate", async (int version, string token, IMediator mediator) =>
        {
            _logger.LogInformation("Validating reset link token: {Token}", token);
            var result = await mediator.Send(new ResetLinkTokenCommand(token));
            if (!result)
            {
                _logger.LogError("Invalid reset link token: {Token}", token);
                return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.RESET_PASSWORD_INVALID_OR_MISSING_TOKEN, false, string.Empty));
            }
            _logger.LogInformation("Reset link token validated successfully: {Token}", token);
            return Results.Json(new ApiResponse<bool>(200, AppConstants.ErrorCodes.RESET_LINK_VALID, true, true));
        });

        //Get Non-Admin Users
        endpoints.MapGet("api/v{version}/users/non-admin/{pageNumber}/{pageSize}", [Authorize] async (int version, string? searchTerm, int pageNumber, int pageSize, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching non-admin users. Page: {PageNumber}, Size: {PageSize}, SearchTerm: {SearchTerm}", pageNumber, pageSize, searchTerm);
            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var query = new GetPagedNonAdminUsersQuery(searchTerm, pageNumber, pageSize);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogError("No non-admin users found for search term: {SearchTerm}", searchTerm);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, String.Empty));
            }
            _logger.LogInformation("Non-admin users fetched successfully.");
            return Results.Json(new ApiResponse<PaginatedResponse<NonAdminUserDto>>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        });

        //Get User Profile by EmailId
        endpoints.MapGet("api/v{version}/users/profile/{emailId}", [Authorize] async (int version, string emailId, IMediator mediator, ClaimsPrincipal principal) =>
        {
            var IsSuperAdmin = String.Equals(principal.FindFirst(AppConstants.IS_SUPER_ADMIN)?.Value, AppConstants.IS_SUPER_ADMIN_TRUE);
            _logger.LogInformation("Fetching user profile for: {EmailId}", emailId);
            var query = new GetUserProfileQuery(emailId, IsSuperAdmin);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogError("User profile not found for: {EmailId}", emailId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_PROFILE_NOTFOUND, false, string.Empty));
            }
            _logger.LogInformation("User profile fetched successfully for: {EmailId}", emailId);
            return Results.Json(new ApiResponse<UserProfileDto>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        });

        //Update Profile
        endpoints.MapPut("api/v{version}/users/profile", [Authorize] async (int version, UserProfileDto updateProfileDto, IMediator mediator, IValidator<UpdateUserProfileCommand> validator, ClaimsPrincipal principal) =>
        {
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            var IsSuperAdmin = String.Equals(principal.FindFirst(AppConstants.IS_SUPER_ADMIN)?.Value, AppConstants.IS_SUPER_ADMIN_TRUE);
            _logger.LogInformation("Updating user profile for: {EmailId}", updateProfileDto.EmailId);
            var command = new UpdateUserProfileCommand(userId, updateProfileDto, IsSuperAdmin);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Update user profile validation failed for: {EmailId}", updateProfileDto.EmailId);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogError("User profile update failed for: {EmailId}", updateProfileDto.EmailId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.PROFILE_NOT_FOUND, false, string.Empty));
            }
            _logger.LogInformation("User profile updated successfully for: {EmailId}", updateProfileDto.EmailId);
            return Results.Json(new ApiResponse<bool>(200, AppConstants.ErrorCodes.USER_MAPPING_UPDATE_SUCCESS, true, true));
        });

        //Create User
        endpoints.MapPost("api/v{version}/users", [Authorize] async (int version, UserProfileDto userProfileDto, IMediator mediator, IValidator<CreateUserCommand> validator) =>
        {
            _logger.LogInformation("Creating user: {EmailId}", userProfileDto.EmailId);
            var command = new CreateUserCommand(userProfileDto);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("User creation validation failed for: {EmailId}", userProfileDto.EmailId);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogError("User creation failed for: {EmailId}", userProfileDto.EmailId);
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.EMAIL_EXIST, false, string.Empty));
            }
            _logger.LogInformation("User created successfully: {EmailId}", userProfileDto.EmailId);
            return Results.Json(new ApiResponse<bool>(201, AppConstants.ErrorCodes.USER_MAPPING_UPDATE_SUCCESS, true, true));
        });

        //Get User Profile by UserId
        endpoints.MapGet("api/v{version}/users/profile/{userId:int}", [Authorize] async (int version, int userId, IMediator mediator, ClaimsPrincipal principal) =>
        {
            var IsSuperAdmin = String.Equals(principal.FindFirst(AppConstants.IS_SUPER_ADMIN)?.Value, AppConstants.IS_SUPER_ADMIN_TRUE);
            _logger.LogInformation("Fetching user profile by userId: {UserId}", userId);
            var query = new GetUserProfileByUserIdQuery(userId, IsSuperAdmin);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogError("No users found for userId: {UserId}", userId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, string.Empty));
            }
            _logger.LogInformation("User fetched successfully for userId: {UserId}", userId);
            return Results.Json(new ApiResponse<UserProfileDto>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        });

        //Update User
        endpoints.MapPut("api/v{version}/users", [Authorize] async (int version, UserProfileDto updateUserDto, IMediator mediator, IValidator<UpdateUserCommand> validator, ClaimsPrincipal principal) =>
        {
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            _logger.LogInformation("Updating user: {EmailId}", updateUserDto.EmailId);
            var command = new UpdateUserCommand(userId, updateUserDto);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("User update validation failed for: {EmailId}", updateUserDto.EmailId);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogError("User update failed for: {EmailId}", updateUserDto.EmailId);
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.EMAIL_EXIST, false, string.Empty));
            }
            _logger.LogInformation("User updated successfully: {EmailId}", updateUserDto.EmailId);
            return Results.Json(new ApiResponse<bool>(204, AppConstants.ErrorCodes.USER_MAPPING_UPDATE_SUCCESS, true, true));
        });

        //Delete User(Soft)
        endpoints.MapDelete("api/v{version}/users/{userId}", [Authorize] async (int version, int userId, IMediator mediator) =>
        {
            _logger.LogInformation("Deleting user with userId: {UserId}", userId);
            var command = new DeleteUserCommand(userId);
            var result = await mediator.Send(command);

            if (!result)
            {
                _logger.LogError("User could not be deleted. UserId: {UserId}", userId);
                return Results.Json(new ApiResponse<bool>(400, AppConstants.ErrorCodes.COMMON_MASTER_LIST_LINKED_ITEM_DELETE_ATTEMPT, false, false));
            }
            _logger.LogInformation("User deleted successfully. UserId: {UserId}", userId);
            return Results.Json(new ApiResponse<bool>(200, AppConstants.ErrorCodes.USER_MAPPING_UPDATE_SUCCESS, true, true));
        });

        //Get If User is part of Critical Groups
        endpoints.MapGet("api/v{version}/users/{userId}/criticalgroups", [Authorize] async (int version, int userId, IMediator mediator) =>
        {
            _logger.LogInformation($"Get critical groups list by UserId: {userId}");
            var query = new GetUserCriticalGroupsQuery(userId);
            var result = await mediator.Send(query);
            if (result == null || !result.Any())
            {
                _logger.LogError("No critical groups found for userId: {UserId}", userId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.CRITICAL_GROUPS_NOT_FOUND, false, string.Empty));
            }
            _logger.LogInformation("Critical groups fetched successfully for userId: {UserId}", userId);
            return Results.Json(new ApiResponse<List<string>>(200, AppConstants.ErrorCodes.USER_GROUPS_FETCHED_SUCCESSFULLY, true, result));

        });

        endpoints.MapGet("api/v{version}/users/template/Users.xlsx", [Authorize] async (int version, IFileHandlingService fileHandlingService, IMediator mediator) =>
        {
            _logger.LogInformation("Received request to get users bulk upload template.");
            var query = new GetAllTelephoneCodesQuery();
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogInformation("No Telephone Codes found for API version {Version}", version);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.TELEPHONE_CODE_MASTER_RECORD_NOT_FOUND, false, string.Empty));
            }

            var template = await fileHandlingService.GetUsersBulkUploadTemplateAsync(result.Select(c => c.TelephoneCode).ToList());
            if (template == null)
            {
                _logger.LogError("Users bulk upload template not found.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.EXPORT_DATA_FAILED, false, string.Empty));
            }
            _logger.LogInformation("Users bulk upload template fetched successfully.");
            return Results.File(template, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Users.xlsx");
        });

        endpoints.MapPost("api/v{version}/users/bulk-upload", [Authorize] async (int version, string passwordResetUrl, IFormFile formFile, IFileHandlingService fileHandlingService, IFileDataConverterService fileDataConverterService, IWebHostEnvironment webHostEnvironment, IMediator mediator, IValidator<BulkInsertUsersCommand> validator) =>
        {

            if (String.IsNullOrEmpty(passwordResetUrl) || !passwordResetUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError("Invalid URL location provided for bulk upload.");
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.INVALID_URL_LOCATION_PROVIDED, false, string.Empty));
            }
            _logger.LogInformation("Received request to upload users in bulk.");

            if (!fileHandlingService.IsFileFormatSupportedForBulkUpload(formFile))
            {
                _logger.LogError("File format not supported.");
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.FILE_FORMAT_NOT_SUPPORTED, false, string.Empty));
            }

            var data = fileHandlingService.ProcessXlsx(formFile);
            if (data == null || !data.Any())
            {
                _logger.LogError("No data found in the uploaded file.");
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.IMPORT_FILE_EMPTY, false, string.Empty));
            }

            var usersDtos = fileDataConverterService.ConvertToUserDto(data);
            if (usersDtos == null || !usersDtos.Any())
            {
                _logger.LogError("No valid user data found in the uploaded file.");
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.INVALID_DATA_FORMAT, false, string.Empty));
            }

            string htmlContent = File.ReadAllText($"{webHostEnvironment.ContentRootPath}//EmailTemplates//resetpassword.html"); // Ensure the template file exists
            var command = new BulkInsertUsersCommand(usersDtos, passwordResetUrl, htmlContent, formFile.FileName);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed for users bulk upload.");
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

        endpoints.MapPost("api/v{version}/users/email/set-password", async (int version, string passwordResetUrl, EmailRequestDto emailRequestDto, IWebHostEnvironment webHostEnvironment, IMediator mediator) =>
        {
            _logger.LogInformation("Received email send request for API version {Version}.", version);

            if (emailRequestDto == null)
            {
                _logger.LogError("Email request DTO is null.");
                return Results.Json(new ApiResponse<string>(400, "Invalid email request.", false, string.Empty));
            }
            string htmlContent = File.ReadAllText($"{webHostEnvironment.ContentRootPath}//EmailTemplates//resetpassword.html"); // Ensure the template file exists
            string updatehtmlresetpasswordContent = File.ReadAllText($"{webHostEnvironment.ContentRootPath}//EmailTemplates//updateresetpassword.html");
            var result = await mediator.Send(new EmailUserSetPasswordCommand(emailRequestDto, passwordResetUrl, htmlContent, updatehtmlresetpasswordContent));

            if (result != null || !result.IsSuccess)
            {
                var errorMessage = new List<string> { result?.Message ?? AppConstants.ErrorCodes.EMAIL_SENDING_FAILED };
                return Results.Json(new ApiResponse<List<string>>(400, result?.Message ?? result.Message, result.IsSuccess, errorMessage));
            }
            else if (result == null)
            {
                _logger.LogError("Email sending failed for recipient: {Recipient}.", emailRequestDto.To);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.EMAIL_SENDING_FAILED, false, string.Empty));
            }

            _logger.LogInformation("Email successfully sent to {Recipient}.", emailRequestDto.To);
            return Results.Json(new ApiResponse<EmailResponseDto>(200, AppConstants.ErrorCodes.EMAIL_SEND_SUCCESS, true, result));
        }).AllowAnonymous();

    }
}
