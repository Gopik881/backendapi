using Elixir.Application.Common.DTOs;
using Elixir.Application.Common.SingleSession.Interface;
using Elixir.Application.Interfaces.Services;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Azure;

namespace Elixir.Infrastructure.Middleware;

public class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    public SessionValidationMiddleware(RequestDelegate next, IJwtService jwtService, IEmailService emailService)
    {
        _next = next;
        _jwtService = jwtService;
        _emailService = emailService;
    }

    public async Task Invoke(HttpContext context, ISessionService sessionService)
    {
        // Skip session validation for specific public endpoints
        var path = context.Request.Path.Value;
        if (!string.IsNullOrEmpty(path))
        {
            // Normalize for comparison; path contains version segment like /api/v1/...
            if (path.IndexOf("/users/password/reset-link/validate", StringComparison.OrdinalIgnoreCase) >= 0
                && HttpMethods.IsGet(context.Request.Method))
            {
                await _next(context);
                return;
            }

            // Exclude the companies-exists check route: .../companies/{companyCode}/exists (GET)
            if (path.IndexOf("/companies/", StringComparison.OrdinalIgnoreCase) >= 0
                && path.EndsWith("/exists", StringComparison.OrdinalIgnoreCase)
                && HttpMethods.IsGet(context.Request.Method))
            {
                await _next(context);
                return;
            }

            // Exclude the login route: api/v{version}/users/login (POST)
            if (path.IndexOf("/users/login", StringComparison.OrdinalIgnoreCase) >= 0
                && HttpMethods.IsPost(context.Request.Method))
            {
                await _next(context);
                return;
            }

            // Exclude the menu route: api/v{version}/menus (GET)
            if (path.IndexOf("/menus", StringComparison.OrdinalIgnoreCase) >= 0
                && path.EndsWith("/menus", StringComparison.OrdinalIgnoreCase)
                && HttpMethods.IsGet(context.Request.Method))
            {
                await _next(context);
                return;
            }
        }

        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (!string.IsNullOrEmpty(token))
        {
            var principal = _jwtService.ValidateToken(token);
            var sessionIdClaim = principal?.FindFirst("SessionId")?.Value;

            if (Guid.TryParse(sessionIdClaim, out var sessionId))
            {
                if (!await sessionService.IsSessionActive(sessionId))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "SESSION_TERMINATED",
                        message = "This session has been terminated due to another login"
                    });
                    var userDetails = await sessionService.GetUserDetailsBySessionIdAsync(sessionId);
                    string email = string.Empty;
                    string firstName = string.Empty;
                    if (userDetails is User user)
                    {
                        email = user.Email;
                        firstName = user.FirstName + " " + user.LastName;
                    }
                    else if (userDetails is SuperUser superUser)
                    {
                        email = superUser.Email;
                        firstName = superUser.FirstName + " " + superUser.LastName;
                    }
                    _ = Task.Run(() =>
                        _emailService.SendEmailAsync(new EmailRequestDto
                        {
                            To = email,
                            Subject = "Login Attempt Notification",
                            HtmlBody = $"Dear {firstName},<br/><br/>Your previous session has been terminated due to another login for your account on {DateTime.UtcNow}. If this was not you, please secure your account immediately.<br/><br/>Thank you,<br/>Elixir Team",
                        })
                    );
                    return;
                }
            }
        }
        await _next(context);
    }
}

