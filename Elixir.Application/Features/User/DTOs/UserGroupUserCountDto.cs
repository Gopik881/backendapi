namespace Elixir.Application.Features.User.DTOs;

public class UserGroupUserCountDto
{
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public string GroupType { get; set; }
    public int UserCount { get; set; }
    public string CreatedByRoleName {get; set;}
    public DateTime? CreatedOn { get; set; }
    public bool? Status { get; set; }
}
