
namespace Elixir.Application.Features.Authentication.Models;

public class JwtSettings
{
    public string RsaKey { get; set; }
    public string SecretKey { get; set; }
    public int ExpirationMinutes { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}
