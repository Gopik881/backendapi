namespace Elixir.Domain.Entities;

public partial class SubMenuItem
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public string SubMenuItemName { get; set; } = null!;

    public string? Description { get; set; }

    public string? SubMenuItemsUrl { get; set; }

    public bool? IsEnabled { get; set; }

    public int? MenuItemId { get; set; }

    public bool IsDeleted { get; set; }

}
