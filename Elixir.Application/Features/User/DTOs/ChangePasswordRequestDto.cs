
namespace Elixir.Application.Features.User.DTOs;

public class ChangePasswordRequestDto
{
    // All properties are nullable, so they are already optional for model binding.
    // To ensure the API does not fail if properties are missing, no further changes are needed here.
    // If you want to ensure the API does not throw validation errors, do not add [Required] attributes.

    public string? UserName { get; set; }
    public string? OldPassword { get; set; }
    public string? NewPassword { get; set; }
}
//public class ResetPasswordDto
//{
   
//}
//public class ResetTokenDto
//{
//    public int Id { get; set; }
//    public string PassKey { get; set; }
//    public string Email { get; set; }
//    public DateTime Expiry { get; set; }
//}

