
namespace Elixir.Application.Features.Menu.DTOs;
public class MenuItemDto
{
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; }
    public string MenuItemsIcon { get; set; }
    public string MenuItemsURL { get; set; }
    public List<SubMenuItemDto> SubMenuItems { get; set; } = new List<SubMenuItemDto>();
}

