namespace Elixir.Domain.Entities;

public partial class MenuItem
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public string MenuItemName { get; set; } = null!;

    public string? Description { get; set; }

    public string? MenuItemsUrl { get; set; }

    public bool? IsEnabled { get; set; }

    public string? MenuItemsIcon { get; set; }

    public bool IsDeleted { get; set; }

}
