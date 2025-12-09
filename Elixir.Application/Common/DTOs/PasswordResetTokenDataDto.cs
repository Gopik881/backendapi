
namespace Elixir.Application.Common.DTOs;

public class PasswordResetTokenDataDto
{
    public string Email { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
}
