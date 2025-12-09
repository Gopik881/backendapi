using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Features.UserGroup.DTOs;

namespace Elixir.Application.Interfaces.Persistance;

public interface IUserGroupMappingsRepository
{
    Task<int> GetUserCustomUserGroupUserForMenuListingAsync(int userId);
    Task<int> GetUserDefaultUserGroupUserForMenuListingAsync(int userId);
    Task<string> GetUserMappedToUserGroupWithHighestPrivelageAsync(int userId);
    Task<IEnumerable<UserGroupDto>> GetUserAssociatedGroupAsync(int userId);
    Task<Tuple<List<UserGroupUserCountDto>, int>> GetFilteredUserAssociatedGroupAsync(bool IsSuperAdmin, string searchTerm, bool IsDefaultUserGroupType, int pageNumber, int pageSize);
    Task<List<CompanyUserDto>> GetCompany5TabUserGroupUsersAsync(int groupId, int companyId);
    Task<IEnumerable<CompanyUserDto>> GetAllUserGroupUsersAsync();
    Task<List<UserListforUserMappingDto>> GetAllUsersforUserMappingAsync();
    Task<IEnumerable<UserMappingGroupsByGroupTypeDto>> GetUserMappingGroupNamesByGroupTypeAsync();
    Task<UserMappedGroupNamesWithCompaniesDto> GetUserMappedGroupNamesWithCompaniesAsync(int userId);
    Task<List<CompanyUserDto>> GetUserMappingUsersByGroupIdAsync(int groupId);
    Tuple<List<UserListforUserMappingDto>, int> GetFilteredUserGroupMappingUsersListAsync(bool IsEligibleToBeRemoved, int groupId, int pageNumber, int pageSize, string searchTerm);
    Task<bool> AddUserGroupMappingAsync(int groupId, List<int> userIds);
    Task<bool> RemoveUserGroupMappingAsync(int groupId, List<int> userIds);
    Tuple<List<CompanyUserDto>, int> GetFilteredUserGroupUsers(int userGroupId, int pageNumber, int pageSize, string searchTerm);
}