using Elixir.Application.Features.Menu.DTOs;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetHorizontalsByGroupId;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetReportingAdminsByGroupId;
using Elixir.Application.Features.UserRightsMetatadata.DTOs;
using Elixir.Domain.Entities;
using System.Text.Json.Serialization;

namespace Elixir.Application.Features.UserGroup.DTOs
{
    public class CreateUserGroupDto
    {
        [JsonPropertyName("roleID")]
        public int? UserGroupId { get; set; }

        [JsonPropertyName("userGroupName")]
        public string UserGroupName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("groupType")]
        public string GroupType { get; set; }
        [JsonPropertyName("CreatedBy")]
        public int? CreateBy { get; set; }

        [JsonPropertyName("status")]
        public bool? Status { get; set; }

        [JsonPropertyName("userRights")]
        public List<UserGroupMenuRights> UserGroupMenuRights { get; set; } = new List<UserGroupMenuRights>();

        [JsonPropertyName("userRightsData")]
        public object UserRightsData { get; set; } = new object();

        [JsonPropertyName("horizontals")]
        public List<UserGroupHorizontals> userGroupHorizontals { get; set; } = new List<UserGroupHorizontals>();

        [JsonPropertyName("reportingAdmins")]
        public List<UserGroupReportingAdmin> userGroupReportingAdmins { get; set; } = new List<UserGroupReportingAdmin>();

        [JsonPropertyName("reportAccesses")]
        public ReportingAccessDto reportingAccessDto { get; set; } = new ReportingAccessDto();
    }

    public class ReportingAccessDto
    {
        [JsonPropertyName("categories")]
        public List<SelectionItemDto> SubCategories { get; set; } = new List<SelectionItemDto>();
        [JsonPropertyName("reports")]
        public List<SelectionItemDto> Reports { get; set; } = new List<SelectionItemDto>();
       
        //[JsonPropertyName("categoryId")]
        //public int CategoryId { get; set; }

        //[JsonPropertyName("name")]
        //public string CategoryName { get; set; }

        //[JsonPropertyName("isSelected")]
        //public bool IsSelected { get; set; }
        [JsonPropertyName("accessType")]
        public string AccessType { get; set; }

        [JsonPropertyName("canDownloadReport")]
        public bool CanDownloadReports { get; set; } = false;
    }

    public class SelectionItemDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("categoryId")]
        public int? CategoryId { get; set; }
        [JsonPropertyName("isSelected")]
        public bool IsSelected { get; set; }
        [JsonPropertyName("subReports")]
        public List<SelectionItemDto> SubReports { get; set; } = new List<SelectionItemDto>();

        //[JsonPropertyName("canDownloadReport")]
        //public bool? canDownloadReport { get; set; }
    }

    public class UserGroupReportingAdmin
    {
        [JsonPropertyName("reportingAdminId")]
        public int ReportingAdminId { get; set; }

        [JsonPropertyName("name")]
        public string ReportingAdminName { get; set; }

        [JsonPropertyName("isSelected")]
        public bool IsSelected { get; set; }
    }

    public class UserGroupMenuRights
    {
        [JsonPropertyName("roleID")]
        public int MenuId { get; set; }
        public string? moduleName { get; set; }
        //[JsonPropertyName("Create")]
        //public bool CreateAccess { get; set; }

        //[JsonPropertyName("Edit")]
        //public bool EditAccess { get; set; }

        //[JsonPropertyName("ViewOnly")]
        //public bool ViewOnlyAccess { get; set; }

        //[JsonPropertyName("Approve")]
        //public bool ApproveAccess { get; set; }

        [JsonPropertyName("screens")]
        public List<UserGroupCreateSubMenuItemsDto>? Screens { get; set; }

        //[JsonPropertyName("ViewOptions")]
        //public ViewOptionsDto? ViewOptions { get; set; } = null;
    }

    public class UserGroupCreateSubMenuItemsDto
    {
        [JsonPropertyName("screenID")]
        public int MenuId { get; set; }

        [JsonPropertyName("screenName")]
        public string SubMenuItemName { get; set; }

        [JsonPropertyName("permissions")]
        public List<SubMenuItemPermissions> Permissions { get; set; }

        [JsonPropertyName("viewOptions")]
        public ViewOptionsDto? ViewOptions { get; set; } = null;
    }

    public class SubMenuItemPermissions
    {
        [JsonPropertyName("create")]
        public bool Create { get; set; }

        [JsonPropertyName("viewOnly")]
        public bool ViewOnly { get; set; }

        [JsonPropertyName("edit")]
        public bool Edit { get; set; }

        [JsonPropertyName("approve")]
        public bool Approve { get; set; }
        public bool IsAllCompanies { get; set; }
    }

    public class UserGroupHorizontals
    {
        [JsonPropertyName("horizontalID")]
        public int? Id { get; set; }

        [JsonPropertyName("horizontalName")]
        public string HorizontalName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("isSelected")]
        public bool IsSelected { get; set; }

        [JsonPropertyName("checkboxItems")]
        public List<HorizontalItem> HorizontalItems { get; set; } = new List<HorizontalItem>();
    }

    public class HorizontalItem
    {
        [JsonPropertyName("checkboxItemID")]
        public int? Id { get; set; }

        [JsonPropertyName("checkboxName")]
        public string ItemName { get; set; }

        [JsonPropertyName("isSelected")]
        public bool IsSelected { get; set; }
    }
}