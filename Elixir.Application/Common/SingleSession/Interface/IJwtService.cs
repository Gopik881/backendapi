using System.Security.Claims;

namespace Elixir.Application.Common.SingleSession.Interface;

public interface IJwtService
{
    ClaimsPrincipal ValidateToken(string token);
}
