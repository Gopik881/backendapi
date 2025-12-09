namespace Elixir.Domain.Entities;

public partial class UserGroupMenuMapping
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int SubMenuItemId { get; set; }

    public int? UserGroupId { get; set; }

    public bool? IsAllCompanies { get; set; }

    public bool? CreateAccess { get; set; }

    public bool? ViewOnlyAccess { get; set; }

    public bool? EditAccess { get; set; }

    public bool? ApproveAccess { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsDeleted { get; set; }

}
