namespace Elixir.Application.Features.User.DTOs;

public class NonAdminUserDto
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool Status { get; set; }
    public DateTime? CreatedOn { get; set; }
}
