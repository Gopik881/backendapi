using Elixir.Application.Common.DTOs;
    using Elixir.Application.Common.Models;
    using Elixir.Application.Interfaces.Services;
    using Elixir.Application.Common.Constants;
    using Microsoft.AspNetCore.Authorization;

    namespace Elixir.Admin.API.Endpoints;
    public static class EmailModule
    {
        private static ILogger _logger;

        public static void RegisterEmailEndpoints(this IEndpointRouteBuilder endpoints)
        {
            // Get the ILoggerFactory from the service provider
            var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();

            // Create a logger for state
            _logger ??= loggerFactory.CreateLogger("EmailApiRoutes");

            endpoints.MapPost("api/v{version}/email", [Authorize] async (int version, EmailRequestDto emailRequestDto, IEmailService emailService) =>
            {
                _logger.LogInformation("Received email send request for API version {Version}.", version);

                if (emailRequestDto == null)
                {
                    _logger.LogError("Email request DTO is null.");
                    return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.EMAIL_INVALID_REQUEST, false, string.Empty));
                }

                var result = await emailService.SendEmailAsync(emailRequestDto);

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
