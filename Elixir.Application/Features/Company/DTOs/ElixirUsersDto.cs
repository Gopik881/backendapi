namespace Elixir.Application.Features.Company.DTOs;

public class ElixirUserListDto
{
    public int? CompanyId { get; set; }
    public string CompanyName { get; set; }
    public List<DefaultUserGroupUserDto> AccountManagerUsersList { get; set; }
    public List<DefaultUserGroupUserDto> CheckerUsersList { get; set; }
    public List<CustomUserGroupUserDto> CustomUserGroupList { get; set; }

    public bool? Status { get; set; }
    public string? OnboardingStatus { get; set; }
    public int? GroupsCount { get; set; }
    public bool? isUnderEdit { get; set; }
    public bool? CompanyStatus { get; set; }
}

public class DefaultUserGroupUserDto
{
    public int RoleId { get; set; }
    public int UserGroupId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}

public class  CustomUserGroupUserDto
{
    public int UserGroupId { get; set; }
    public string UserGroupName { get; set; }
    public int? UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}