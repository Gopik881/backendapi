using Elixir.Application.Features.Menu.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class MenuItemsRepository : IMenuItemsRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public MenuItemsRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<MenuItemDto>> GetMenuItemWithSubMenuGroupingByUserGroup(int userGroupId, bool IsSuperUser)
    {
        // Fetch Menu Items
        var menuItems = _dbContext.MenuItems.Where(m => !m.IsDeleted).ToList();
        // Fetch SubMenu Items
        var subMenuItems = _dbContext.SubMenuItems.Where(sm => !sm.IsDeleted).ToList();
        // Fetch Role-based Mapping
        List<MenuItemDto> result = null;

        if (!IsSuperUser)
        {
            var roleMappingDict = _dbContext.UserGroupMenuMappings.Where(r => r.UserGroupId == userGroupId && !r.IsDeleted && 
                                    ((r.ViewOnlyAccess ?? false)
                                    || (r.EditAccess ?? false)
                                    || (r.ApproveAccess ?? false)
                                    || (r.CreateAccess ?? false)))
                .ToDictionary(rm => rm.SubMenuItemId, rm => rm);
            var menuItemIds = _dbContext.SubMenuItems.Where(item => roleMappingDict.Keys.Contains(item.Id)).Select(item => item.MenuItemId)
                .Distinct() .ToList();

            //As per the Frontend requirement, we need to show only those menu items which have sub menu items mapped to the user group.
            //Except DashBoard so Adding Dashboard Menu Item Id to the menuItemIds list.
            if (!menuItemIds.Contains(1)) // Assuming 1 is the ID for Dashboard
            {
                menuItemIds.Add(1);
            }

            // Assemble Data in Memory
            result = menuItems.Where(m=> menuItemIds.Contains(m.Id)).Select(menu => new MenuItemDto
            {
                MenuItemId = menu.Id,
                MenuItemName = menu.MenuItemName,
                MenuItemsIcon = menu.MenuItemsIcon,
                MenuItemsURL = menu.MenuItemsUrl,
                SubMenuItems = subMenuItems.Where(subMenu => subMenu.MenuItemId == menu.Id && 
                roleMappingDict.Keys.Contains(subMenu.Id))
                    
                .Select(subMenu => new SubMenuItemDto
                    {
                        SubMenuItemId = subMenu.Id,
                        SubMenuItemName = subMenu.SubMenuItemName,
                        SubMenuItemURL = subMenu.SubMenuItemsUrl,
                        ViewOnlyAccess = roleMappingDict.ContainsKey(subMenu.Id) ? roleMappingDict[subMenu.Id].ViewOnlyAccess ?? false : false,
                        EditAccess = roleMappingDict.ContainsKey(subMenu.Id) ? roleMappingDict[subMenu.Id].EditAccess ?? false : false,
                        ApproveAccess = roleMappingDict.ContainsKey(subMenu.Id) ? roleMappingDict[subMenu.Id].ApproveAccess ?? false : false,
                        CreateAccess = roleMappingDict.ContainsKey(subMenu.Id) ? roleMappingDict[subMenu.Id].CreateAccess ?? false : false
                    })
                    .ToList()
            }).ToList();
        }
        else
        {
            result = menuItems.Select(menu => new MenuItemDto
            {
                MenuItemId = menu.Id,
                MenuItemName = menu.MenuItemName,
                MenuItemsIcon = menu.MenuItemsIcon,
                MenuItemsURL = menu.MenuItemsUrl,
                SubMenuItems = subMenuItems.Where(subMenu => subMenu.MenuItemId == menu.Id)
                    .Select(subMenu => new SubMenuItemDto
                    {
                        SubMenuItemId = subMenu.Id,
                        SubMenuItemName = subMenu.SubMenuItemName,
                        SubMenuItemURL = subMenu.SubMenuItemsUrl,
                        ViewOnlyAccess = true,
                        EditAccess = true,
                        ApproveAccess = true,
                        CreateAccess = true
                    }).ToList()
            }).ToList();
        }

        return result;
    }
}
