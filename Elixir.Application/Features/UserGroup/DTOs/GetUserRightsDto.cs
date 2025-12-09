namespace Elixir.Application.Features.UserGroup.DTOs;
public class GetUserRightsDto
{
    public int RoleID { get; set; }
    public string ModuleName { get; set; }
    public List<UserRigthsScreenDto> Screens { get; set; }
}

public class UserRigthsScreenDto
{
    public int ScreenID { get; set; }
    public string ScreenName { get; set; }
    //public List<PermissionDto> Permissions { get; set; }
    //public List<PermissionDto> Checkbox { get; set; }
    //public List<DependencyDto> Dependencies { get; set; }
    //public List<ParentDto> Parents { get; set; }
    //public ViewOptionsDto ViewOptions { get; set; }
}
