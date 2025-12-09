namespace Elixir.Domain.Entities;

public partial class Horizontal
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int UserGroupId { get; set; }

    public string HorizontalName { get; set; } = null!;

    public string? Description { get; set; }
    public bool IsSelected { get; set; }

    public bool IsDeleted { get; set; }

}
