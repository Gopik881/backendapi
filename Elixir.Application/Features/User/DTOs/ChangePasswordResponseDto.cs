
namespace Elixir.Application.Features.User.DTOs;

public class ChangePasswordResponseDto
{
    public bool Status { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; } = new List<string>();

}
