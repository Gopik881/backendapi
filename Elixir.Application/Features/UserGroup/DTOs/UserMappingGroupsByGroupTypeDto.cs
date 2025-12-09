using Elixir.Application.Features.User.DTOs;

namespace Elixir.Application.Features.UserGroup.DTOs;

public class UserMappingGroupsByGroupTypeDto
{
    public string GroupType { get; set; }
    public List<UserGroupDto> GroupNames { get; set; }
}
