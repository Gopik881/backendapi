using Elixir.Application.Features.Menu.DTOs;

namespace Elixir.Application.Interfaces.Persistance;
public interface IMenuItemsRepository
{
    Task<List<MenuItemDto>> GetMenuItemWithSubMenuGroupingByUserGroup(int userGroupId, bool IsSuperUser);
}