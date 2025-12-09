namespace Elixir.Application.Features.User.DTOs;

public class UserGroupDto
{
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public string GroupType { get; set; }
    public int? CreatedBy { get; set; }
    public bool? IsSuperAdminCreatedGroup { get; set; }
    public bool? Status { get; set; }
    public string? Description { get; set; }
}
