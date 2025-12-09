using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Enums;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.GetUserRightsResponse.DTOs;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Features.UserRightsMetatadata.DTOs;
using Elixir.Application.Features.UserRightsMetatadata.Queries.GetUserRightsMetadata;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;

namespace Elixir.Infrastructure.Persistance.Repositories;
public class UserGroupsRepository : IUserGroupsRepository
{
    private readonly ElixirHRDbContext _dbContext;
    public UserGroupsRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<Company5TabUserGroupDto>> GetCompany5TabUserGroupsByCompanyIdAsync(int companyId)
    {
        // Get user IDs in AccountManager or Checker groups (to exclude from migration group)
        var excludedUserIds = await _dbContext.UserGroupMappings
            .Where(x => x.UserGroupId == (int)Roles.AccountManager || x.UserGroupId == (int)Roles.Checker)
            .Select(x => x.UserId)
            .Distinct()
            .ToListAsync();

        // Get custom groups (excluding AccountManager, Checker, DelegateAdmin) with users not mapped to the company
        var customGroups = await _dbContext.UserGroupMappings
            .Where(um => um.UserGroupId != (int)Roles.AccountManager
                      && um.UserGroupId != (int)Roles.Checker
                      && um.UserGroupId != (int)Roles.DelegateAdmin)
            .Where(um => !_dbContext.Company5TabOnboardingHistories
                .Any(cu => cu.UserId == um.UserId && cu.CompanyId == companyId))
            .Join(_dbContext.UserGroups,
                  um => um.UserGroupId,
                  ug => ug.Id,
                  (um, ug) => new { um.UserGroupId, ug.GroupName })
            .GroupBy(x => new { x.UserGroupId, x.GroupName })
            .Select(g => new Company5TabUserGroupDto
            {
                GroupId = g.Key.UserGroupId,
                GroupName = g.Key.GroupName ?? string.Empty
            })
            .ToListAsync();

        // Get migration group (AccountManager) with users not in excludedUserIds and not mapped to the company
        var migrationGroup = await _dbContext.UserGroupMappings
            .Where(ug => ug.UserGroupId == (int)Roles.AccountManager
                && !excludedUserIds.Contains(ug.UserId)
                && !_dbContext.Company5TabOnboardingHistories
                    .Any(cu => cu.UserId == ug.UserId && cu.CompanyId == companyId))
            .Join(_dbContext.UserGroups,
                  ug => ug.UserGroupId,
                  group => group.Id,
                  (ug, group) => new { ug.UserGroupId, group.GroupName })
            .GroupBy(x => new { x.UserGroupId, x.GroupName })
            .Select(g => new Company5TabUserGroupDto
            {
                GroupId = g.Key.UserGroupId,
                GroupName = g.Key.GroupName ?? string.Empty
            })
            .ToListAsync();

        // Combine and filter groups to ensure at least one mapped user is not in company users
        var allGroups = customGroups
            .Union(migrationGroup)
            .GroupBy(g => g.GroupId)
            .Select(g => g.First())
            .ToList();

        var result = new List<Company5TabUserGroupDto>();

        foreach (var group in allGroups)
        {
            var mappedUsers = await _dbContext.UserGroupMappings
                .Where(um => um.UserGroupId == group.GroupId)
                .Select(um => um.UserId)
                .ToListAsync();

            var hasEligibleUser = mappedUsers.Any(uid =>
                !_dbContext.Company5TabOnboardingHistories
                    .Any(cu => cu.UserId == uid && cu.CompanyId == companyId));

            if (hasEligibleUser)
            {
                result.Add(group);
            }
        }

        return result;
    }
    public async Task<List<CompanyUserDto>> GetUserGroupUsersByGroupIdAsync(int groupId)
    {
        return await _dbContext.UserGroups
            .Where(ug => ug.Id == groupId)
            .Join(_dbContext.Users,
                  ug => ug.CreatedBy,
                  user => user.Id,
                  (ug, user) => new CompanyUserDto
                  {
                      UserId = user.Id,
                      UserName = user.FirstName + " " + user.LastName,
                      Email = user.Email,
                  })
            .ToListAsync();
    }
    public async Task<bool> AddUserRightsAsync(int userGroupId, List<UserGroupMenuRights> userRights)
    {
        var menuMappings = userRights
            .SelectMany(ur => ur.Screens)
            .Where(screen => screen.Permissions.FirstOrDefault() != null)
            .SelectMany(screen =>
            {
                var permission = screen.Permissions.First();
                var mappings = new List<UserGroupMenuMapping>
                {
                    new()
                    {
                        SubMenuItemId = screen.MenuId,
                        IsAllCompanies = screen.ViewOptions?.AllCompanies ?? false,
                        ViewOnlyAccess = permission.ViewOnly,
                        EditAccess = permission.Edit,
                        ApproveAccess = permission.Approve,
                        CreateAccess = permission.Create,
                        UserGroupId = userGroupId,
                        IsEnabled = true
                    }
                };

                return mappings;
            })
            .ToList();

        if (menuMappings.Any())
        {
            await _dbContext.UserGroupMenuMappings.AddRangeAsync(menuMappings);
            await _dbContext.SaveChangesAsync();
        }

        return true;
    }
    public async Task<int> CreateUserGroupAsync(CreateUserGroupDto createUserGroupDto)
    {
        var userGroup = new UserGroup
        {
            GroupName = createUserGroupDto.UserGroupName,
            Description = createUserGroupDto.Description,
            GroupType = createUserGroupDto.GroupType,
            CreatedBy = createUserGroupDto.CreateBy,
            CreatedAt = DateTime.UtcNow,
            IsEnabled = createUserGroupDto.Status
            
        };

        await _dbContext.UserGroups.AddAsync(userGroup);
        await _dbContext.SaveChangesAsync();
        return userGroup.Id;

    }
    public async Task<bool> UpdateUserGroupAsync(CreateUserGroupDto userRightsDto)
    {
        var userGroup = await _dbContext.UserGroups.FindAsync(userRightsDto.UserGroupId);
        if (userGroup == null) return false;        

        // Update group details
        userGroup.GroupName = userRightsDto.UserGroupName;
        userGroup.Description = userRightsDto.Description;
        userGroup.GroupType = userRightsDto.GroupType;
        userGroup.IsEnabled = userRightsDto.Status;

        _dbContext.UserGroups.Update(userGroup);
        return await _dbContext.SaveChangesAsync() > 0;

    }
    public async Task<List<UserGroup>> CheckForDuplicateRightsAsync(List<UserGroupMenuRights> userRights, int? userGroupId = 0)
    {
        // Null-safe flattening of screens
        var provided = userRights
            .SelectMany(ur => ur.Screens ?? new List<UserGroupCreateSubMenuItemsDto>())
            .Where(s => s.MenuId != 14)
            .Select(s =>
                $"{s.MenuId}-{s.Permissions[0].Create}-{s.Permissions[0].ViewOnly}-{s.Permissions[0].Edit}-{s.Permissions[0].Approve}-{s.Permissions[0].IsAllCompanies}")
            .OrderBy(x => x)
            .ToHashSet();

        // Get all mappings for all groups except SubMenuItemId 14
        var groupMappingsQuery = _dbContext.UserGroupMenuMappings.Where(m => m.SubMenuItemId != 14);
        if (userGroupId > 0)
        {
            // Exclude current groupId from duplicate check during update
            groupMappingsQuery = groupMappingsQuery.Where(m => m.UserGroupId != userGroupId);
        }
        var groupMappings = await groupMappingsQuery.ToListAsync();

        // Group by UserGroupId and build permission sets in-memory
        var groupPermissions = groupMappings
            .GroupBy(m => m.UserGroupId)
            .Select(g => new
            {
                GroupId = g.Key,
                Permissions = g
                    .Select(m => $"{m.SubMenuItemId}-{m.CreateAccess}-{m.ViewOnlyAccess}-{m.EditAccess}-{m.ApproveAccess}-{m.IsAllCompanies}")
                    .OrderBy(x => x)
                    .ToHashSet()
            })
            .ToList();

        var matches = groupPermissions.Where(g => g.Permissions.SetEquals(provided)).ToList();
        var matchingGroupIds = matches.Select(m => m.GroupId.Value).ToList(); // Assuming GroupId is int?

        return await _dbContext.UserGroups
            .Where(u => matchingGroupIds.Contains(u.Id))
            .ToListAsync();
    }
    public async Task<string?> GetRoleNameByGroupId(int groupId)
    {
        return await _dbContext.UserGroups
            .Where(ug => ug.Id == groupId)
            .Join(_dbContext.Roles,
                  ug => ug.CreatedBy,
                  r => r.Id,
                  (ug, r) => r.RoleName)
            .FirstOrDefaultAsync();
    }
    public async Task<string> GetUserGroupNameByIdAsync(int groupId)
    {
        var userGroup = await _dbContext.UserGroups.FirstOrDefaultAsync(ug => ug.Id == groupId);
        return userGroup?.GroupName ?? string.Empty;
    }
    public async Task<UserGroupDto> GetUserGroupByIdAsync(int groupId)
    {
        var userGroup = await _dbContext.UserGroups.FirstOrDefaultAsync(ug => ug.Id == groupId);
        if (userGroup == null)
            return new UserGroupDto();

        return new UserGroupDto
        {
            GroupId = userGroup.Id,
            GroupName = userGroup.GroupName,
            GroupType = userGroup.GroupType,
            Status = userGroup.IsEnabled,
            CreatedBy = userGroup.CreatedBy,
            Description = userGroup.Description,
        };
    }
    public async Task<bool> DeleteUserGroupDetailsByUserGroupIdAsync(int groupId)
    {
        var userGroupUsers = _dbContext.UserGroupMappings.Where(u => u.UserGroupId == groupId).ToList();
        _dbContext.UserGroupMappings.RemoveRange(userGroupUsers);
        await _dbContext.SaveChangesAsync();

        var userGroupUsersfromCompanyUsersfromElixirUsersHistory = _dbContext.ElixirUsersHistories.Where(u => u.UserGroupId == groupId).ToList();
        if (userGroupUsersfromCompanyUsersfromElixirUsersHistory.Count == 0)
        _dbContext.ElixirUsersHistories.RemoveRange(userGroupUsersfromCompanyUsersfromElixirUsersHistory);
        await _dbContext.SaveChangesAsync();
        var userGroupUsersfromCompanyUsers = _dbContext.ElixirUsers.Where(u => u.UserGroupId == groupId).ToList();
        if(userGroupUsersfromCompanyUsers.Count == 0)
        _dbContext.ElixirUsers.RemoveRange(userGroupUsersfromCompanyUsers);
        await _dbContext.SaveChangesAsync();


        return true;
    }
    public async Task<object> GetUserRightsByUserGroupId(int userGroupId)
    {
        var userGroup = await _dbContext.UserGroups
            .Where(ug => ug.Id == userGroupId)
            .FirstOrDefaultAsync();

        if (userGroup == null)
        {
            return new
            {
                statusCode = 404,
                message = "User group not found.",
                success = false,
                data = (object?)null,
                timestamp = DateTime.UtcNow,
                errors = new List<string>(),
                pagination = (object?)null
            };
        }

        var generator = new UserRightsMetadataGenerator();
        var userRightsResult = generator.GetUserRightsMetadataForUserType1();

        var defaultcheckBoxPermissions = userRightsResult.Data.UserRights
            .SelectMany(ur => ur.Screens)
            .ToDictionary(
                s => s.ScreenID,
                s =>
                {
                    var checkbox = s.Checkbox.FirstOrDefault() ?? new PermissionDto();
                    return new PermissionDto
                    {
                        ViewOnly = checkbox.ViewOnly,
                        Edit = checkbox.Edit,
                        Approve = checkbox.Approve,
                        Create = checkbox.Create
                    };
                });

        var userRightsRaw = await (from ugm in _dbContext.UserGroupMenuMappings
                                   join smi in _dbContext.SubMenuItems on ugm.SubMenuItemId equals smi.Id
                                   join mi in _dbContext.MenuItems on smi.MenuItemId equals mi.Id
                                   where ugm.UserGroupId == userGroupId && ugm.IsEnabled == true && ugm.IsDeleted == false
                                       && smi.IsEnabled == true && smi.IsDeleted == false
                                       && mi.IsEnabled == true && mi.IsDeleted == false
                                   select new
                                   {
                                       mi.Id,
                                       ugm.SubMenuItemId,
                                       mi.MenuItemName,
                                       smi.SubMenuItemName,
                                       smi.MenuItemId,
                                       ugm.ViewOnlyAccess,
                                       ugm.EditAccess,
                                       ugm.ApproveAccess,
                                       ugm.CreateAccess,
                                       ugm.IsAllCompanies
                                   }).ToListAsync();

        // Group by MenuItem (module) and then by SubMenuItem (screen)
        var grouped = userRightsRaw
            .GroupBy(x => new { x.MenuItemId, x.MenuItemName })
            .Select(moduleGroup => new
            {
                roleID = userGroupId,
                moduleName = moduleGroup.Key.MenuItemName,
                screens = moduleGroup
                    .OrderBy(x => x.SubMenuItemId)
                    .Select(x => new
                    {
                        screenID = x.SubMenuItemId,
                        screenName = x.SubMenuItemName,
                        permissions = new List<object>
                        {
                            new
                            {
                                viewOnly = x.ViewOnlyAccess ?? false,
                                edit = x.EditAccess ?? false,
                                approve = x.ApproveAccess ?? false,
                                create = x.CreateAccess ?? false,
                                IsAllCompanies = x.IsAllCompanies ?? false
                            }
                        },
                        checkbox = new List<object>
                        {
                            defaultcheckBoxPermissions.ContainsKey(x.SubMenuItemId)
                                ? new
                                {
                                    viewOnly = defaultcheckBoxPermissions[x.SubMenuItemId].ViewOnly,
                                    edit = defaultcheckBoxPermissions[x.SubMenuItemId].Edit,
                                    approve = defaultcheckBoxPermissions[x.SubMenuItemId].Approve,
                                    create = defaultcheckBoxPermissions[x.SubMenuItemId].Create
                                }
                                : new
                                {
                                    viewOnly = false,
                                    edit = false,
                                    approve = false,
                                    create = false
                                }
                        },
                        dependencies = GetDependenciesDto(x.SubMenuItemId, x.MenuItemId)
                            .Select(d => new
                            {
                                screenID = d.ScreenId,
                                permissionType = d.PermissionType
                            }).ToList(),
                        parents = GetParentsDto(x.SubMenuItemId, x.MenuItemId)
                            .Select(p => new
                            {
                                screenID = p.ScreenId,
                                permissionType = p.PermissionType
                            }).ToList(),
                        // Replace the viewOptions assignment in GetUserRightsByUserGroupId with the following:
                        viewOptions = (x.MenuItemId == 6 || x.MenuItemId == 11 || x.MenuItemId == 12)
                            ? (
                                (x.ViewOnlyAccess == true || x.EditAccess == true || x.ApproveAccess == true || x.CreateAccess == true)
                                    ? new
                                    {
                                        allCompanies = x.IsAllCompanies ?? false,
                                        custom = !(x.IsAllCompanies ?? false)
                                    }
                                    : new
                                    {
                                        allCompanies = false,
                                        custom = false
                                    }
                              )
                            : null
                    }).ToList()
            }).ToList();

        return new
        {
            userRights = grouped
        };
    }
    // Helper methods for Dto mapping//GetDependenciesDto
    private static List<GetDependency> GetParentsDto(int subMenuItemId, int? menuItemId)
    {
        var dependencies = new List<GetDependency>(); // Changed type to match GetUserRightsResponse.DTOs.GetDependency

        if (subMenuItemId == 7)
            dependencies.Add(new GetDependency { ScreenId = "6", PermissionType = "Create" });
        else if (subMenuItemId == 9)
            dependencies.Add(new GetDependency { ScreenId = "8", PermissionType = "Create" });
        else if (subMenuItemId == 12 || subMenuItemId == 13)
        {
            dependencies.Add(new GetDependency { ScreenId = "11", PermissionType = "Create" });
            //dependencies.Add(new GetDependency { ScreenId = "12", PermissionType = "Create" });
        }
        else if (subMenuItemId == 2)
            dependencies.Add(new GetDependency { ScreenId = "1", PermissionType = "Create" });

        return dependencies;
    }
    private static List<GetParent> GetDependenciesDto(int subMenuItemId, int? menuItemId)
    {
        var parents = new List<GetParent>(); // Changed type to match GetUserRightsResponse.DTOs.GetParent

        if (subMenuItemId == 6)
            parents.Add(new GetParent { ScreenId = "7", PermissionType = "Edit" });
        else if (subMenuItemId == 8)
            parents.Add(new GetParent { ScreenId = "9", PermissionType = "Edit" });
        else if (subMenuItemId == 11)
        {
            parents.Add(new GetParent { ScreenId = "12", PermissionType = "Edit" });
            parents.Add(new GetParent { ScreenId = "13", PermissionType = "Edit" });
            parents.Add(new GetParent { ScreenId = "15", PermissionType = "Edit" });
        }        
        else if (subMenuItemId == 1)
            parents.Add(new GetParent { ScreenId = "2", PermissionType = "Edit" });

        return parents;
    }
    public List<GetHorizontal> GetHorizontals(int userGroupId)
    {
        return _dbContext.Horizontals
            .Where(h => h.UserGroupId == userGroupId)
            .Select(h => new GetHorizontal
            {
                HorizontalId = h.Id,
                HorizontalName = h.HorizontalName,
                Description = h.Description,
                CheckboxItems = new List<GetCheckboxItem>() // Always return an empty list
            })
            .ToList();
    }
    public async Task<bool> UpdateUserRightsAsync(List<UserGroupMenuRights> userRights, int groupId)
    {
        // Remove existing menu mappings for the group
        var existingMappings = _dbContext.UserGroupMenuMappings.Where(m => m.UserGroupId == groupId);
        _dbContext.UserGroupMenuMappings.RemoveRange(existingMappings);
        await _dbContext.SaveChangesAsync();

        var menuMappings = userRights
            .SelectMany(ur => ur.Screens ?? new List<UserGroupCreateSubMenuItemsDto>())
            .Where(screen => screen.Permissions != null && screen.Permissions.FirstOrDefault() != null)
            .Select(screen =>
            {
                var permission = screen.Permissions.First();
                return new UserGroupMenuMapping
                {
                    SubMenuItemId = screen.MenuId,
                    IsAllCompanies = permission.IsAllCompanies,
                    ViewOnlyAccess = permission.ViewOnly,
                    EditAccess = permission.Edit,
                    ApproveAccess = permission.Approve,
                    CreateAccess = permission.Create,
                    UserGroupId = groupId,
                    IsEnabled = true,
                };
            })
            .ToList();

        if (menuMappings.Any())
        {
            await _dbContext.UserGroupMenuMappings.AddRangeAsync(menuMappings);

            // Check if IsAllCompanies is true for both submenuitems 12 and 13
            bool isAllCompanies12 = menuMappings.Any(m => m.SubMenuItemId == AppConstants.SUBMENUITEM_ID_COMPANY_ONBOARDING_LIST && (m.IsAllCompanies == true || m.CreateAccess == false && m.ViewOnlyAccess == false && m.EditAccess == false && m.ApproveAccess == false));
            bool isAllCompanies13 = menuMappings.Any(m => m.SubMenuItemId == AppConstants.SUBMENUITEM_ID_COMPANY_LIST && (m.IsAllCompanies == true || m.CreateAccess == false && m.ViewOnlyAccess == false && m.EditAccess == false && m.ApproveAccess == false));

            // if company onboarding list and company list for both screens permissions are false then remove all entries from ElixirUsers table for that user group
            bool isAllCompanies12False = menuMappings.Any(m => m.SubMenuItemId == AppConstants.SUBMENUITEM_ID_COMPANY_ONBOARDING_LIST && m.CreateAccess == false && m.ViewOnlyAccess == false && m.EditAccess == false && m.ApproveAccess == false);
            bool isAllCompanies13False = menuMappings.Any(m => m.SubMenuItemId == AppConstants.SUBMENUITEM_ID_COMPANY_LIST && m.CreateAccess == false && m.ViewOnlyAccess == false && m.EditAccess == false && m.ApproveAccess == false);


            //if ((isAllCompanies12 && isAllCompanies13) || (isAllCompanies12False && isAllCompanies13False))
            //{
            //    // Get all user IDs mapped to the given user group
            //    var userIdsInGroup = await _dbContext.UserGroupMappings
            //        .Where(um => um.UserGroupId == groupId)
            //        .Select(um => um.UserId)
            //        .ToListAsync();

            //    // Find ElixirUsers entries where UserGroupId and UserId match
            //    var companyUsersToRemove = await _dbContext.ElixirUsers
            //        .Where(eu => eu.UserGroupId == groupId && userIdsInGroup.Contains(eu.UserId))
            //        .ToListAsync();
            //    _dbContext.ElixirUsers.RemoveRange(companyUsersToRemove);
            //}
            if (isAllCompanies13 || isAllCompanies13False)
            {
                // Get all user IDs mapped to the given user group
                var userIdsInGroup = await _dbContext.UserGroupMappings
                    .Where(um => um.UserGroupId == groupId)
                    .Select(um => um.UserId)
                    .ToListAsync();

                // Get company IDs where IsEnabled is true
                var enabledCompanyIds = await _dbContext.Companies
                    .Where(c => c.IsEnabled == true)
                    .Select(c => c.Id)
                    .ToListAsync();

                // Find ElixirUsers entries where UserGroupId and UserId match, and company is enabled
                var companyUsersToRemove = await _dbContext.ElixirUsers
                    .Where(eu => eu.UserGroupId == groupId
                        && userIdsInGroup.Contains(eu.UserId)
                        && enabledCompanyIds.Contains(eu.CompanyId))
                    .ToListAsync();

                _dbContext.ElixirUsers.RemoveRange(companyUsersToRemove);
            }

            if (isAllCompanies12 || isAllCompanies12False)
            {
                // Get all user IDs mapped to the given user group
                var userIdsInGroup = await _dbContext.UserGroupMappings
                    .Where(um => um.UserGroupId == groupId)
                    .Select(um => um.UserId)
                    .ToListAsync();

                var onboardingCompanyIds = await _dbContext.Companies
                    .Where(c => c.IsEnabled == false)
                    .Select(c => c.Id)
                    .ToListAsync();

                // Find ElixirUsers entries where UserGroupId and UserId match, and company is enabled
                var companyUsersToRemove = await _dbContext.ElixirUsers
                    .Where(eu => eu.UserGroupId == groupId
                        && userIdsInGroup.Contains(eu.UserId)
                        && onboardingCompanyIds.Contains(eu.CompanyId))
                    .ToListAsync();

                _dbContext.ElixirUsers.RemoveRange(companyUsersToRemove);
            }

            return await _dbContext.SaveChangesAsync() > 0;
        }
        return false;
    }
    public async Task<bool> IsGroupNameAvailableAsync(string groupName)
    {
        return !await _dbContext.UserGroups.AnyAsync(g => g.GroupName == groupName);
    }



    // Plan (pseudocode):
    // 1. Normalize provided inputs:
    //    - Build providedRights as HashSet<string> from userRights screens (skip menuId 14).
    //    - Build providedHorizontals as List of tuples (Name, IsSelected, Items) where Items is ordered list of selected checkbox item names.
    //    - Build providedReports as HashSet<string> of valid report entries that are selected.
    //    - Build providedAdmins as HashSet<string> of provided reporting admin entries.
    // 2. Query DB for all groups (excluding optional excludedUserGroupId) and gather their IDs.
    // 3. Query DB mappings:
    //    - rightsMappings: map each group's menu mappings (skip SubMenuItemId 14) into HashSet<string> permissions.
    //    - horizontalsMappings: load horizontals and related checkbox items, group into List<(Name, IsSelected, Items)> per group.
    //    - reportsAccesses: group per user group into HashSet<string> of selected valid reports.
    //    - adminsMappings: group per user group into HashSet<string> of admins.
    // 4. For each DB group ID:
    //    - Compare rights HashSet equality.
    //    - Compare reports and admins HashSet equality.
    //    - Compare horizontals by sorting/ordering and then element-wise compare Name, IsSelected and Items sequence-equality.
    //    - If all match, add groupId to matching set.
    // 5. Return UserGroup entities for matching group IDs.
    //
    // Notes:
    // - Use strongly-typed tuples for horizontals to avoid dynamic.
    // - Use ToDictionary overloads with proper selectors (no invalid 3-type generic).
    // - Guard for nulls and missing keys by defaulting to empty sets/lists.

    public async Task<List<UserGroup>> CheckForDuplicateGroupConfigurationsAsync(List<UserGroupMenuRights> userRights,List<UserGroupHorizontals> horizontals,ReportingAccessDto reportingAccessDto,List<UserGroupReportingAdmin> reportingAdmins,int? excludedUserGroupId = 0)
    {
        // Step 1: Normalize provided inputs

        // Menu Rights
        var providedRights = (userRights ?? new List<UserGroupMenuRights>())
            .SelectMany(ur => ur.Screens ?? new List<UserGroupCreateSubMenuItemsDto>())
            .Where(s => s.MenuId != 14)
            .Select(s =>
            {
                var perm = s.Permissions?.FirstOrDefault();
                return $"{s.MenuId}-{(perm?.Create ?? false)}-{(perm?.ViewOnly ?? false)}-{(perm?.Edit ?? false)}-{(perm?.Approve ?? false)}-{(perm?.IsAllCompanies ?? false)}";
            })
            .OrderBy(x => x)
            .ToHashSet();

        // Horizontals as strongly-typed tuples
        var providedHorizontals = (horizontals ?? new List<UserGroupHorizontals>())
            .Select(h => (
                Name: h.HorizontalName?.Trim() ?? string.Empty,
                IsSelected: h.IsSelected,
                Items: (h.HorizontalItems ?? new List<HorizontalItem>())
                    .Where(i => i.IsSelected)
                    .Select(i => i.ItemName?.Trim() ?? string.Empty)
                    .OrderBy(x => x)
                    .ToList()
            ))
            .OrderBy(x => x.Name)
            .ThenBy(x => x.IsSelected)
            .ToList();

        // Reports: only include valid report ids from DB
        var validReportIds = await _dbContext.Reports
            .Select(r => r.Id)
            .ToHashSetAsync();

        var providedReports = (reportingAccessDto?.Reports ?? new List<SelectionItemDto>())
            .Where(r => validReportIds.Contains(r.Id) && r.IsSelected)
            .Select(r => $"{r.Id}-true-{(reportingAccessDto?.CanDownloadReports ?? false)}")
            .OrderBy(x => x)
            .ToHashSet();

        // Reporting admins
        var providedAdmins = (reportingAdmins ?? new List<UserGroupReportingAdmin>())
            .Select(a => $"{a.ReportingAdminId}-{a.IsSelected}")
            .OrderBy(x => x)
            .ToHashSet();

        // Step 2: Fetch all group IDs (exclude optional group)
        var groupsQuery = _dbContext.UserGroups.AsQueryable();
        if (excludedUserGroupId > 0)
        {
            groupsQuery = groupsQuery.Where(u => u.Id != excludedUserGroupId);
        }
        var groupIds = await groupsQuery.Select(u => u.Id).ToHashSetAsync();

        // Step 3: Fetch configuration mappings from DB

        // Rights mappings (skip SubMenuItemId 14)
        var rightsQuery = _dbContext.UserGroupMenuMappings.Where(m => m.SubMenuItemId != 14);
        if (excludedUserGroupId > 0)
        {
            rightsQuery = rightsQuery.Where(m => m.UserGroupId != excludedUserGroupId);
        }
        var rightsMappings = await rightsQuery.ToListAsync();

        var groupRights = rightsMappings
            .GroupBy(m => m.UserGroupId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(m => $"{m.SubMenuItemId}-{m.CreateAccess}-{m.ViewOnlyAccess}-{m.EditAccess}-{m.ApproveAccess}-{m.IsAllCompanies}")
                      .OrderBy(x => x)
                      .ToHashSet()
            );

        // Horizontals and checkbox items
        var horizontalsQuery = _dbContext.Horizontals.AsQueryable();
        if (excludedUserGroupId > 0)
        {
            horizontalsQuery = horizontalsQuery.Where(h => h.UserGroupId != excludedUserGroupId);
        }
        var horizontalsMappings = await horizontalsQuery.ToListAsync();
        var horizontalIds = horizontalsMappings.Select(h => h.Id).ToList();

        var checkboxItems = horizontalIds.Any()
            ? await _dbContext.WebQueryHorizontalCheckboxItems
                .Where(ci => horizontalIds.Contains(ci.HorizontalId))
                .ToListAsync()
            : new List<WebQueryHorizontalCheckboxItem>();

        var groupHorizontals = horizontalsMappings
            .GroupBy(h => h.UserGroupId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(h => (
                        Name: h.HorizontalName?.Trim() ?? string.Empty,
                        IsSelected: h.IsSelected,
                        Items: checkboxItems
                            .Where(ci => ci.HorizontalId == h.Id && (ci.IsSelected ?? false))
                            .Select(ci => ci.CheckboxItemName?.Trim() ?? string.Empty)
                            .OrderBy(x => x)
                            .ToList()
                    ))
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.IsSelected)
                    .ToList()
            );

        // Reports accesses
        var reportsQuery = _dbContext.ReportAccesses.Where(ra => !ra.IsDeleted);
        if (excludedUserGroupId > 0)
        {
            reportsQuery = reportsQuery.Where(ra => ra.UserGroupId != excludedUserGroupId);
        }
        var reportsAccesses = await reportsQuery.ToListAsync();

        var groupReports = reportsAccesses
            .GroupBy(ra => ra.UserGroupId)
            .ToDictionary(
                g => g.Key,
                g => g.Where(ra => validReportIds.Contains(ra.ReportId) && (ra.IsSelected ?? false))
                      .Select(ra => $"{ra.ReportId}-true-{ra.CanDownload}")
                      .OrderBy(x => x)
                      .ToHashSet()
            );

        // Reporting admins
        var adminsQuery = _dbContext.ReportingAdmins.AsQueryable();
        if (excludedUserGroupId > 0)
        {
            adminsQuery = adminsQuery.Where(ra => ra.UserGroupId != excludedUserGroupId);
        }
        var adminsMappings = await adminsQuery.ToListAsync();

        var groupAdmins = adminsMappings
            .GroupBy(ra => ra.UserGroupId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(ra => $"{ra.ReportingAdminId}-{ra.IsSelected}")
                      .OrderBy(x => x)
                      .ToHashSet()
            );

        // Step 4: Compare provided configuration with each group's configuration
        var matchingGroupIds = new HashSet<int>();
        foreach (var groupId in groupIds)
        {
            // rights comparison
            var rightsSet = groupRights.ContainsKey(groupId) ? groupRights[groupId] : new HashSet<string>();
            var rightsMatch = providedRights.SetEquals(rightsSet);

            // reports comparison
            var reportsSet = groupReports.ContainsKey(groupId) ? groupReports[groupId] : new HashSet<string>();
            var reportsMatch = providedReports.SetEquals(reportsSet);

            // admins comparison
            var adminsSet = groupAdmins.ContainsKey(groupId) ? groupAdmins[groupId] : new HashSet<string>();
            var adminsMatch = providedAdmins.SetEquals(adminsSet);

            // horizontals comparison
            var groupHorz = groupHorizontals.ContainsKey(groupId) ? groupHorizontals[groupId] : new List<(string Name, bool IsSelected, List<string> Items)>();
            var horizontalsMatch = providedHorizontals.Count == groupHorz.Count &&
                providedHorizontals.Zip(groupHorz, (p, g) =>
                    p.Name == g.Name &&
                    p.IsSelected == g.IsSelected &&
                    p.Items.SequenceEqual(g.Items)
                ).All(x => x);

            if (rightsMatch && horizontalsMatch && reportsMatch && adminsMatch)
            {
                matchingGroupIds.Add(groupId);
            }
        }

        // Step 5: Return matching UserGroups
        return await _dbContext.UserGroups
            .Where(u => matchingGroupIds.Contains(u.Id))
            .ToListAsync();
    }
}

