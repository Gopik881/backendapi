namespace Elixir.Application.Features.GetUserRightsResponse.DTOs;


// Models
public class GetUserRightsResponse
{
    public string Message { get; set; }
    public GetUserRightsData Data { get; set; }
}

public class GetUserRightsData
{
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public string RoleName { get; set; }
    public string Description { get; set; }
    public bool? Status { get; set; }
    public int CreatedBy { get; set; }
    public string GroupType { get; set; }
    public List<GetUserRight> UserRights { get; set; }
    public List<GetHorizontal> Horizontals { get; set; }
    public List<object> ReportingAdmins { get; set; }
    public List<GetReportAccess> ReportAccesses { get; set; }
}

public class GetUserRight
{
    public int RoleId { get; set; }
    public string ModuleName { get; set; }
    public List<GetScreen> Screens { get; set; }
}

public class GetScreen
{
    public int ScreenId { get; set; }
    public string ScreenName { get; set; }
    public List<GetPermission> Permissions { get; set; }
    public List<GetCheckbox> Checkbox { get; set; }
    public List<GetDependency> Dependencies { get; set; }
    public List<GetParent> Parents { get; set; }
    public GetViewOptions ViewOptions { get; set; }
}

public class GetPermission
{
    public bool ViewOnly { get; set; }
    public bool Edit { get; set; }
    public bool Approve { get; set; }
    public bool Create { get; set; }
}

public class GetCheckbox
{
    public bool ViewOnly { get; set; }
    public bool Edit { get; set; }
    public bool Approve { get; set; }
    public bool Create { get; set; }
}

public class GetDependency
{
    public string ScreenId { get; set; }
    public string PermissionType { get; set; }
}

public class GetParent
{
    public string ScreenId { get; set; }
    public string PermissionType { get; set; }
}

public class GetViewOptions
{
    public bool AllCompanies { get; set; }
    public bool Custom { get; set; }
}

public class GetHorizontal
{
    public int HorizontalId { get; set; }
    public string HorizontalName { get; set; }
    public string Description { get; set; }
    public bool IsSelected { get; set; }
    public List<GetCheckboxItem> CheckboxItems { get; set; }
}

public class GetCheckboxItem
{
    public int CheckboxItemId { get; set; }
    public string CheckboxName { get; set; }
    public bool IsSelected { get; set; }
}

public class GetReportAccess
{
    public List<GetCategory> Categories { get; set; }
    public List<GetReport> Reports { get; set; }
    public bool CanDownloadReports { get; set; }
}

public class GetCategory
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public bool IsSelected { get; set; }
    public bool? CanDownloadReport { get; set; }
    public List<GetSubReport> SubReports { get; set; }
}

public class GetReport
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public bool IsSelected { get; set; }
    public bool? CanDownloadReport { get; set; }
    public List<GetSubReport> SubReports { get; set; }
}

public class GetSubReport
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public bool IsSelected { get; set; }
    public bool? CanDownloadReport { get; set; }
    public List<GetSubReport> SubReports { get; set; }
}
