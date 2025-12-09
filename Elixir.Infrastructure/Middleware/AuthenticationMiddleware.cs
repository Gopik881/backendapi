using Elixir.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Elixir.Infrastructure.Middleware;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint(); // Get current endpoint metadata
        var requiresAuth = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>() != null;
        
        if (requiresAuth && !context.User.Identity.IsAuthenticated)
        {
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>();
            response.Success = false;
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            response.StatusCode = StatusCodes.Status401Unauthorized;
            response.Message = "Unauthorized: Please log in or provide a valid token.";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            return;
        }

        await _next(context); // Continue request processing
    }
}