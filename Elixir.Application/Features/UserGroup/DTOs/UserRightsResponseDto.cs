using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Features.UserGroup.DTOs
{
    public class UserRightsResponseDto
    {
        public List<UserRightDto> UserRights { get; set; }
        public List<HorizontalDto>? Horizontals { get; set; }
        public List<ReportingAdminDto>? ReportingAdmins { get; set; }
        public List<ReportingAccessDto>? ReportAccesses { get; set; }
    }

    public class UserRightDto
    {
        public int RoleID { get; set; }
        public string ModuleName { get; set; }
        public List<UserRigthsScreenDto> Screens { get; set; }
    }

   

    public class HorizontalDto
    {
        public int HorizontalID { get; set; }
        public string HorizontalName { get; set; }
        public string? Description { get; set; }
        public bool IsSelected { get; set; }
        public List<CheckboxItemDto> CheckboxItems { get; set; }
    }

    public class CheckboxItemDto
    {
        public int CheckboxItemID { get; set; }
        public string CheckboxName { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ReportingAdminDto
    {
        public int ReportingAdminID { get; set; }
        public int UserID { get; set; }
        public string? AdminRole { get; set; }
        public DateTime? AssignedDate { get; set; }
        public bool IsSelected { get; set; }
        public bool Active { get; set; }
    }

   
}
