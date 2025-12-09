using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Domain.Entities;

namespace Elixir.Application.Interfaces.Persistance;

public interface IUserGroupsRepository
{
    Task<IEnumerable<Company5TabUserGroupDto>> GetCompany5TabUserGroupsByCompanyIdAsync(int companyId);
    Task<List<CompanyUserDto>> GetUserGroupUsersByGroupIdAsync(int groupId);
    Task<int> CreateUserGroupAsync(CreateUserGroupDto createUserGroupDto);
    Task<bool> AddUserRightsAsync(int userGroupId, List<UserGroupMenuRights> userRights);
    Task<bool> UpdateUserRightsAsync(List<UserGroupMenuRights> userRights, int groupId);
    Task<bool> UpdateUserGroupAsync(CreateUserGroupDto userRightsDto);
    Task<string?> GetRoleNameByGroupId(int groupId);
    Task<UserGroupDto> GetUserGroupByIdAsync(int groupId);
    Task<bool> DeleteUserGroupDetailsByUserGroupIdAsync(int groupId);
    Task<object> GetUserRightsByUserGroupId(int userGroupId);
    Task<bool> IsGroupNameAvailableAsync(string groupName);
    Task<List<UserGroup>> CheckForDuplicateRightsAsync(List<UserGroupMenuRights> userRights, int? userGroupId = 0);
    Task<string> GetUserGroupNameByIdAsync(int groupId);

    Task<List<UserGroup>> CheckForDuplicateGroupConfigurationsAsync(List<UserGroupMenuRights> userRights, List<UserGroupHorizontals> horizontals, ReportingAccessDto reportingAccessDto, List<UserGroupReportingAdmin> reportingAdmins, int? excludedUserGroupId = 0);
    
    }