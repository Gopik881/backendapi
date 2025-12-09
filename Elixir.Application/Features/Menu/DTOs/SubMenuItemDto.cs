namespace Elixir.Application.Features.Menu.DTOs;
public class SubMenuItemDto
{
    public int SubMenuItemId { get; set; }
    public string SubMenuItemName { get; set; }
    public string SubMenuItemURL { get; set; }
    public bool? ViewOnlyAccess { get; set; }
    public bool? EditAccess { get; set; }
    public bool? ApproveAccess { get; set; }
    public bool? CreateAccess { get; set; }
}
