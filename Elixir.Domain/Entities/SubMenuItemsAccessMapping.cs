namespace Elixir.Domain.Entities;

public partial class SubMenuItemsAccessMapping
{
    public int Id { get; set; }

    public int SubMenuItemsId { get; set; }

    public int AccessToSubMenuItemsId { get; set; }

    public bool IsDeleted { get; set; }

}
