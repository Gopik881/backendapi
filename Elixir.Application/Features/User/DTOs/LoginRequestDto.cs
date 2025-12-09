namespace Elixir.Application.Features.User.DTOs;

public class LoginRequestDto
{
    public string UserName { get; set; }
    public string Password { get; set; }

    public string? CompanyCode { get; set; }
}
