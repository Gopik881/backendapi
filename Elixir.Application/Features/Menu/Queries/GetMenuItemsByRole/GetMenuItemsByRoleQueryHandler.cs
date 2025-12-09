using Elixir.Application.Features.Menu.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Menu.Queries.GetMenuItemsByRole;
public record GetMenuItemsByRoleQuery(int userId,bool ISuperUser) : IRequest<List<MenuItemDto>>;
public class GetMenuItemsByRoleQueryHandler : IRequestHandler<GetMenuItemsByRoleQuery, List<MenuItemDto>>
{
    IMenuItemsRepository _menuItemsRepository;
    IUserGroupMappingsRepository _userGroupMappingsRepository;
    public GetMenuItemsByRoleQueryHandler(IMenuItemsRepository menuItemsRepository, IUserGroupMappingsRepository userGroupMappingsRepository)
    {
        _menuItemsRepository = menuItemsRepository;
        _userGroupMappingsRepository = userGroupMappingsRepository;
    }
    public async Task<List<MenuItemDto>> Handle(GetMenuItemsByRoleQuery request, CancellationToken cancellationToken)
    {
        //bool IsSuperUser = false;
        //Check if the user is part of custom Group, a user can be part if only one custom group in the entire system.
        var userGroupId = 0;
        if (!request.ISuperUser)
        {
            userGroupId = await _userGroupMappingsRepository.GetUserCustomUserGroupUserForMenuListingAsync(request.userId);
            if (userGroupId < 1)
                userGroupId = await _userGroupMappingsRepository.GetUserDefaultUserGroupUserForMenuListingAsync(request.userId);
        }
        if (userGroupId == 0 && !request.ISuperUser) return null;

        return await _menuItemsRepository.GetMenuItemWithSubMenuGroupingByUserGroup(userGroupId,request.ISuperUser);
    }
}
