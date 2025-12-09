
namespace Elixir.Application.Features.UserRightsMetatadata.DTOs;
public class UserRightsMetedataResponseDto
{
    public string Message { get; set; }
    public UserRightsDataDto Data { get; set; }
}

public class UserRightsDataDto
{
    public List<UserRightDto> UserRights { get; set; }
    public List<HorizontalDto> Horizontals { get; set; }
    public List<object> ReportingAdmins { get; set; }
    public List<ReportAccessDto> ReportAccesses { get; set; }
}

public class UserRightDto
{
    public int RoleID { get; set; }
    public string ModuleName { get; set; }
    public List<ScreenDto> Screens { get; set; }
}

public class ScreenDto
{
    public int ScreenID { get; set; }
    public string ScreenName { get; set; }
    public List<PermissionDto> Permissions { get; set; }
    public List<PermissionDto> Checkbox { get; set; }
    public List<DependencyDto> Dependencies { get; set; }
    public List<ParentDto> Parents { get; set; }
    public ViewOptionsDto ViewOptions { get; set; }
}

public class PermissionDto
{
    public bool ViewOnly { get; set; }
    public bool Edit { get; set; }
    public bool Approve { get; set; }
    public bool Create { get; set; }
    public bool IsAllCompanies { get; set; }   
}

public class DependencyDto
{
   public string ScreenID { get; set; }
    public string PermissionType { get; set; }
}

public class ParentDto
{
    public string ScreenID { get; set; }
    public string PermissionType { get; set; }
}

public class ViewOptionsDto
{
    public bool? AllCompanies { get; set; }
    public bool? Custom { get; set; }
}

public class HorizontalDto
{
    public int HorizontalID { get; set; }
    public string HorizontalName { get; set; }
    public string Description { get; set; }
    public bool IsSelected { get; set; }
    public List<CheckboxItemDto> CheckboxItems { get; set; }
}

public class CheckboxItemDto
{
    public int CheckboxItemID { get; set; }
    public string CheckboxName { get; set; }
    public bool IsSelected { get; set; }
}

public class ReportAccessDto
{
    public List<ReportCategoryDto> Categories { get; set; }
    public List<ReportDto> Reports { get; set; }
    public bool CanDownloadReports { get; set; }
}

public class ReportCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public bool IsSelected { get; set; }
    //public bool? CanDownloadReport { get; set; }
    public List<object> SubReports { get; set; }
}

public class ReportDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public bool IsSelected { get; set; }
    //public bool? CanDownloadReport { get; set; }
    public List<SubReportDto> SubReports { get; set; }
}

public class SubReportDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public bool IsSelected { get; set; }
    //public bool? CanDownloadReport { get; set; }
    public List<object> SubReports { get; set; }
}
