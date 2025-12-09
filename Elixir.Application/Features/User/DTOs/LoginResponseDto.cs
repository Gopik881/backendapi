
namespace Elixir.Application.Features.User.DTOs;
public class LoginResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string Token { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string UserProfilePicture { get; set; }
    public int SessionTimeout { get; set; }
    public bool IsSuperAdmin { get; set; }
    public string UserMappedToUserGroup { get; set; } //Give the highest previlage Access
    public bool IsPolicyChanged { get; set; } //Indicates if there are any policy changes that require user attention
}


