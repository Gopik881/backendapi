namespace Elixir.Domain.Entities;

public partial class UserGroup
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string GroupName { get; set; } = null!;

    public string GroupType { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsEnabled { get; set; }

    public bool IsDeleted { get; set; }

}
