namespace Elixir.Application.Features.UserGroup.DTOs;

public class UserListforUserMappingDto
{
    public int UserID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool? Status { get; set; }
    public string IsEligible { get; set; }
    public bool? IsSameGroup { get; set; }
    public bool? IsDefaultGroup { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? NotEligibleReason { get; set; }
}
