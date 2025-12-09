namespace Elixir.Application.Features.User.DTOs;

public class ResetPasswordRequestDto
{
    public string? UserName { get; set; }
    public string? NewPassword { get; set; }
    public string? Token { get; set; }
}
