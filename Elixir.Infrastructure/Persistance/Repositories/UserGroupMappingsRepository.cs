using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Common.Enums;
using Elixir.Application.Features.UserGroup.DTOs;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class UserGroupMappingsRepository : IUserGroupMappingsRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public UserGroupMappingsRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> AddUserGroupMappingAsync(int groupId, List<int> userIds)
    {
        //if given groupId is disabled throw exception saying not allowed to add for disabled group
        var isDisableGroup = await _dbContext.UserGroups.FirstOrDefaultAsync(g => g.Id == groupId && (g.IsEnabled ?? true));
        if (isDisableGroup == null)
            throw new Exception(AppConstants.ErrorCodes.USER_GROUP_NOT_FOUND_OR_DISABLED);

        if (userIds == null || userIds.Count == 0)
            return false;

        var group = await _dbContext.UserGroups.FirstOrDefaultAsync(g => g.Id == groupId);
        if (group == null)
            return false;

        // Check for existing mappings to avoid duplicates
        var existingMappings = await _dbContext.UserGroupMappings.Where(ugm => ugm.UserGroupId == groupId && userIds.Contains(ugm.UserId))
            .Select(ugm => ugm.UserId)
            .ToListAsync();

        //Exclude the Ids already present and create new mappings only for those not already mapped
        var newMappings = userIds
            .Where(um => !existingMappings.Contains(um))
            .Select(um => new UserGroupMapping
            {
                UserGroupId = groupId,
                UserId = um,
                CreatedAt = DateTime.UtcNow
            })
            .ToList();

        if (newMappings.Count == 0)
            return false;
        await _dbContext.UserGroupMappings.AddRangeAsync(newMappings);
        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<bool> RemoveUserGroupMappingAsync(int groupId, List<int> userIds)
    {
        if (userIds == null || userIds.Count == 0)
            return false;

        ////if given groupId is disabled throw exception saying not allowed to add for disabled group
        //var isDisableGroup = await _dbContext.UserGroups.FirstOrDefaultAsync(g => g.Id == groupId && (g.IsEnabled ?? true));
        //if (isDisableGroup == null)
        //    throw new Exception(AppConstants.ErrorCodes.USER_GROUP_NOT_FOUND_OR_DISABLED);

        // Check for Account Manager group (client-level)
        if (groupId == (int)UserGroupRoles.AccountManager)
        {
            // Get all clients where at least one of the selected users is mapped as AM
            var clientsWithSelectedUsers = await _dbContext.ElixirUsers
                .Where(eu => eu.UserGroupId == groupId && userIds.Contains(eu.UserId))
                .Select(eu => eu.ClientId)
                .Distinct()
                .ToListAsync();

            foreach (var clientId in clientsWithSelectedUsers)
            {
                // Get all AMs for this client
                var amsForClient = await _dbContext.ElixirUsers
                    .Where(eu => eu.UserGroupId == groupId && eu.ClientId == clientId)
                    .Select(eu => eu.UserId)
                    .ToListAsync();

                // If all AMs for this client are being removed, block the operation
                if (amsForClient.All(userId => userIds.Contains(userId)))
                {
                    // Fetch client name from Clients table
                    var clientName = await _dbContext.Clients
                        .Where(c => c.Id == clientId)
                        .Select(c => c.ClientName)
                        .FirstOrDefaultAsync();

                    throw new Exception($"Cannot remove all Account Managers for client '{clientName ?? clientId.ToString()}'. Each client must have at least one Account Manager.");
                }

            }
        }

        // Check for Account Manager or Checker group (company-level)
        if (groupId == (int)UserGroupRoles.AccountManager || groupId == (int)UserGroupRoles.Checker)
        {
            // Get all companies where at least one of the selected users is mapped as AM or Checker
            var companiesWithSelectedUsers = await _dbContext.ElixirUsers
                .Where(eu => eu.UserGroupId == groupId && userIds.Contains(eu.UserId))
                .Select(eu => eu.CompanyId)
                .Distinct()
                .ToListAsync();

            // For each company, check if removing these users would leave the company with zero AMs or Checkers
            foreach (var companyId in companiesWithSelectedUsers)
            {
                // Get all AMs or Checkers for this company
                var usersForCompany = await _dbContext.ElixirUsers
                    .Where(eu => eu.UserGroupId == groupId && eu.CompanyId == companyId)
                    .Select(eu => eu.UserId)
                    .ToListAsync();

                // If all AMs or Checkers for this company are being removed, block the operation
                if (usersForCompany.All(userId => userIds.Contains(userId)))
                {
                    var roleName = groupId == (int)UserGroupRoles.AccountManager ? "Account Manager" : "Checker";
                    var companyName = await _dbContext.Companies
                        .Where(c => c.Id == companyId)
                        .Select(c => c.CompanyName)
                        .FirstOrDefaultAsync();

                    throw new Exception($"Cannot remove all {roleName}s for company '{companyName ?? companyId.ToString()}'. Each company must have at least one {roleName}.");
                }
            }
        }

        var elixirUserGroupMappings = await _dbContext.UserGroupMappings
            .Where(eu => eu.UserGroupId == groupId && userIds.Contains(eu.UserId))
            .ToListAsync();

        if (elixirUserGroupMappings.Count == 0)
            return false;

        // Remove from UserGroupMappings
        _dbContext.UserGroupMappings.RemoveRange(elixirUserGroupMappings);

        // Remove from ElixirUsers
        var elixirUsers = await _dbContext.ElixirUsers
            .Where(eu => eu.UserGroupId == groupId && userIds.Contains(eu.UserId))
            .ToListAsync();
        if (elixirUsers.Count > 0)
        {
            _dbContext.ElixirUsers.RemoveRange(elixirUsers);
        }

        // Remove only those users who are not the single Account Manager for this client
        if (groupId == (int)UserGroupRoles.AccountManager)
        {
            var clientsWithSelectedUsers = await _dbContext.ElixirUsers
                .Where(eu => eu.UserGroupId == groupId && userIds.Contains(eu.UserId))
                .Select(eu => eu.ClientId)
                .Distinct()
                .ToListAsync();

            foreach (var clientId in clientsWithSelectedUsers)
            {
                var amsForClient = await _dbContext.ElixirUsers
                    .Where(eu => eu.UserGroupId == groupId && eu.ClientId == clientId)
                    .Select(eu => eu.UserId)
                    .ToListAsync();

                var removableUserIds = amsForClient.Where(userId => userIds.Contains(userId)).ToList();

                // Only remove if not the single Account Manager for this client
                if (amsForClient.Count > 1)
                {
                    foreach (var userId in removableUserIds)
                    {
                        var elixirUser = await _dbContext.ElixirUsers
                            .FirstOrDefaultAsync(eu => eu.UserGroupId == groupId && eu.ClientId == clientId && eu.UserId == userId);
                        if (elixirUser != null)
                        {
                            _dbContext.ElixirUsers.Remove(elixirUser);
                        }
                    }
                }
            }
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<int> GetUserCustomUserGroupUserForMenuListingAsync(int userId)
    {
        // Fetch User Group IDs where user is mapped
        var groupIds = await _dbContext.UserGroupMappings.Where(m => m.UserId == userId && !m.IsDeleted).Select(ugm => ugm.UserGroupId).ToListAsync();
        // Fetch the first matching UserGroup based on the retrieved IDs
        return await _dbContext.UserGroups.Where(ug => !ug.IsDeleted && groupIds.Contains(ug.Id) && EF.Functions.Like(ug.GroupType, AppConstants.USER_GROUP_TYPE_CUSTOM))
            .Select(ug => ug.Id).FirstOrDefaultAsync();
    }
    public async Task<int> GetUserDefaultUserGroupUserForMenuListingAsync(int userId)
    {
        // Fetch User Group Mappings Items
        var groupIds = _dbContext.UserGroupMappings.Where(m => m.UserId == userId && !m.IsDeleted).Select(ugm => ugm.UserGroupId).ToList();
        // Fetch Group Names
        return await _dbContext.UserGroups.Where(ug => !ug.IsDeleted && groupIds.Contains(ug.Id) && EF.Functions.Like(ug.GroupType, AppConstants.USER_GROUP_TYPE_DEFAULT))
            .OrderBy(ug => ug.Id) // Ensures lowest Id comes first, so We get the Account Manager in case he is part of multiple groups
            .Select(ug => ug.Id).FirstOrDefaultAsync();
    }
    public async Task<string> GetUserMappedToUserGroupWithHighestPrivelageAsync(int userId)
    {
        var groupIds = await _dbContext.UserGroupMappings.Where(m => m.UserId == userId && !m.IsDeleted).Select(ugm => ugm.UserGroupId).ToListAsync();
        // Fetch the first matching UserGroup based on the retrieved IDs
        var userGroupName = await _dbContext.UserGroups.Where(ug => !ug.IsDeleted && (ug.IsEnabled ?? false) && groupIds.Contains(ug.Id) && EF.Functions.Like(ug.GroupType, AppConstants.USER_GROUP_TYPE_CUSTOM))
            .Select(ug => ug.GroupName).FirstOrDefaultAsync();
        if (String.IsNullOrEmpty(userGroupName))
            userGroupName = await _dbContext.UserGroups.Where(ug => !ug.IsDeleted && (ug.IsEnabled ?? false) && groupIds.Contains(ug.Id) && EF.Functions.Like(ug.GroupType, AppConstants.USER_GROUP_TYPE_DEFAULT))
            .OrderBy(ug => ug.Id) // Ensures lowest Id comes first, so We get the Account Manager in case he is part of multiple groups
            .Select(ug => ug.GroupName).FirstOrDefaultAsync();
        return userGroupName;
    }
    public async Task<IEnumerable<UserGroupDto>> GetUserAssociatedGroupAsync(int userId)
    {
        try
        {
            // Fetch User Group Mappings Items
            var groupIds = _dbContext.UserGroupMappings.Where(m => m.UserId == userId && !m.IsDeleted).Select(ugm => ugm.UserGroupId).ToList();
            // Fetch Group Names
            return _dbContext.UserGroups.Where(ug => !ug.IsDeleted && groupIds.Contains(ug.Id) && !EF.Functions.Like(ug.GroupName.Trim(), AppConstants.SUPER_ADMIN_ROLE)).Select(ug => new UserGroupDto
            {
                GroupId = ug.Id,
                GroupName = ug.GroupName,
                GroupType = ug.GroupType

            }).AsEnumerable();
        }
        catch
        {
            throw new Exception(AppConstants.ErrorCodes.CLIENT_ACCOUNT_MANAGERS_RETRIEVAL_FAILED);
        }
    }
    //public async Task<Tuple<List<UserGroupUserCountDto>, int>> GetFilteredUserAssociatedGroupAsync(bool IsSuperAdmin, string searchTerm, bool IsDefaultUserGroupType, int pageNumber, int pageSize)
    //{
    //    try
    //    {
    //        var groupTypeToFilter = IsDefaultUserGroupType ? AppConstants.USER_GROUP_TYPE_DEFAULT : AppConstants.USER_GROUP_TYPE_CUSTOM;
    //        int totalCount = 0;
    //        List<UserGroupUserCountDto> userGroups = new List<UserGroupUserCountDto>();

    //        // Prepare base query
    //        var baseQuery = _dbContext.UserGroups
    //            .Where(ug => !ug.IsDeleted && ug.GroupType == groupTypeToFilter);

    //        if (IsSuperAdmin)
    //        {
    //            baseQuery = baseQuery.Where(ug => ug.CreatedBy == (int)Roles.SuperAdmin);
    //        }

    //        // Try to parse searchTerm as a date
    //        DateTime searchDate = default;
    //        bool isDateSearch = !string.IsNullOrWhiteSpace(searchTerm) &&
    //            DateTime.TryParseExact(searchTerm, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out searchDate);

    //        if (isDateSearch)
    //        {
    //            baseQuery = baseQuery.Where(ug => ug.CreatedAt.Date == searchDate.Date);
    //        }

    //        // Materialize to memory for CreatedAt and string comparison
    //        var groupList = await baseQuery
    //            .Select(ug => new
    //            {
    //                ug.Id,
    //                ug.GroupType,
    //                ug.GroupName,
    //                ug.CreatedAt,
    //                ug.CreatedBy,
    //                ug.IsEnabled
    //            })
    //            .ToListAsync();

    //        // Get user counts for each group
    //        var groupIds = groupList.Select(g => g.Id).ToList();
    //        var userMappingCounts = await _dbContext.UserGroupMappings
    //            .Where(ugm => !ugm.IsDeleted && groupIds.Contains(ugm.UserGroupId))
    //            .Join(
    //                _dbContext.Users.Where(u => !u.IsDeleted),
    //                ugm => ugm.UserId,
    //                u => u.Id,
    //                (ugm, u) => ugm
    //            )
    //            .GroupBy(ugm => ugm.UserGroupId)
    //            .Select(g => new { GroupId = g.Key, Count = g.Count() })
    //            .ToListAsync();

    //        userGroups = groupList
    //            .OrderByDescending(g => g.CreatedAt)
    //            .Select(g =>
    //            {
    //                var userCount = userMappingCounts.FirstOrDefault(x => x.GroupId == g.Id)?.Count ?? 0;
    //                return new UserGroupUserCountDto
    //                {
    //                    GroupId = g.Id,
    //                    GroupType = g.GroupType,
    //                    GroupName = g.GroupName,
    //                    CreatedOn = g.CreatedAt,
    //                    Status = g.IsEnabled,
    //                    CreatedByRoleName = g.CreatedBy == 1 ? AppConstants.SUPER_ADMIN_ROLE : "",
    //                    UserCount = userCount
    //                };
    //            }).ToList();

    //        // Apply searchTerm logic for all DTO columns (after mapping to DTOs)
    //        if (!string.IsNullOrWhiteSpace(searchTerm) && !isDateSearch)
    //        {
    //            userGroups = userGroups.Where(dto =>
    //                (!string.IsNullOrEmpty(dto.GroupName) && dto.GroupName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
    //                (dto.Status == true && "enabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
    //                (dto.Status == false && "disabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
    //                (dto.CreatedOn.HasValue && dto.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
    //                dto.UserCount.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
    //            ).ToList();
    //        }

    //        totalCount = userGroups.Count;

    //        // Pagination should be at last
    //        userGroups = userGroups
    //            .Skip((pageNumber - 1) * pageSize)
    //            .Take(pageSize)
    //            .ToList();

    //        return new Tuple<List<UserGroupUserCountDto>, int>(userGroups, totalCount);
    //    }
    //    catch
    //    {
    //        throw new Exception(AppConstants.ErrorCodes.CLIENT_ACCOUNT_MANAGERS_RETRIEVAL_FAILED);
    //    }
    //}

    public async Task<Tuple<List<UserGroupUserCountDto>, int>> GetFilteredUserAssociatedGroupAsync(bool IsSuperAdmin, string searchTerm, bool IsDefaultUserGroupType, int pageNumber, int pageSize)
    {
        try
        {
            var groupTypeToFilter = IsDefaultUserGroupType ? AppConstants.USER_GROUP_TYPE_DEFAULT : AppConstants.USER_GROUP_TYPE_CUSTOM;
            int totalCount = 0;
            List<UserGroupUserCountDto> userGroups = new List<UserGroupUserCountDto>();

            // Prepare base query
            var baseQuery = _dbContext.UserGroups
                .Where(ug => !ug.IsDeleted && ug.GroupType == groupTypeToFilter);

            if (IsSuperAdmin)
            {
                baseQuery = baseQuery.Where(ug => ug.CreatedBy == (int)Roles.SuperAdmin);
            }

            // Try to parse searchTerm as a date
            DateTime searchDate = default;
            bool isDateSearch = !string.IsNullOrWhiteSpace(searchTerm) &&
                DateTime.TryParseExact(searchTerm, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out searchDate);

            if (isDateSearch)
            {
                baseQuery = baseQuery.Where(ug => ug.CreatedAt.Date == searchDate.Date);
            }

            // Materialize to memory for CreatedAt/UpdatedAt and string comparison
            var groupList = await baseQuery
                .Select(ug => new
                {
                    ug.Id,
                    ug.GroupType,
                    ug.GroupName,
                    ug.CreatedAt,
                    /* include UpdatedAt so we can order by last modified time (fallback to CreatedAt) */
                    UpdatedAt = EF.Property<DateTime?>(ug, "UpdatedAt"),
                    ug.CreatedBy,
                    ug.IsEnabled
                })
                .ToListAsync();

            // Get user counts for each group
            var groupIds = groupList.Select(g => g.Id).ToList();
            var userMappingCounts = await _dbContext.UserGroupMappings
                .Where(ugm => !ugm.IsDeleted && groupIds.Contains(ugm.UserGroupId))
                .Join(
                    _dbContext.Users.Where(u => !u.IsDeleted),
                    ugm => ugm.UserId,
                    u => u.Id,
                    (ugm, u) => ugm
                )
                .GroupBy(ugm => ugm.UserGroupId)
                .Select(g => new { GroupId = g.Key, Count = g.Count() })
                .ToListAsync();

            // Order by last modified (UpdatedAt if present otherwise CreatedAt) so latest created or updated records appear at top
            userGroups = groupList
                .OrderByDescending(g => g.UpdatedAt ?? g.CreatedAt)
                .Select(g =>
                {
                    var userCount = userMappingCounts.FirstOrDefault(x => x.GroupId == g.Id)?.Count ?? 0;
                    return new UserGroupUserCountDto
                    {
                        GroupId = g.Id,
                        GroupType = g.GroupType,
                        GroupName = g.GroupName,
                        CreatedOn = g.CreatedAt,
                        Status = g.IsEnabled,
                        CreatedByRoleName = g.CreatedBy == 1 ? AppConstants.SUPER_ADMIN_ROLE : "",
                        UserCount = userCount
                    };
                }).ToList();

            // Apply searchTerm logic for all DTO columns (after mapping to DTOs)
            if (!string.IsNullOrWhiteSpace(searchTerm) && !isDateSearch)
            {
                userGroups = userGroups.Where(dto =>
                    (!string.IsNullOrEmpty(dto.GroupName) && dto.GroupName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (dto.Status == true && "enabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (dto.Status == false && "disabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (dto.CreatedOn.HasValue && dto.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    dto.UserCount.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            totalCount = userGroups.Count;

            // Pagination should be at last
            userGroups = userGroups
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new Tuple<List<UserGroupUserCountDto>, int>(userGroups, totalCount);
        }
        catch
        {
            throw new Exception(AppConstants.ErrorCodes.CLIENT_ACCOUNT_MANAGERS_RETRIEVAL_FAILED);
        }
    }
    public async Task<List<CompanyUserDto>> GetCompany5TabUserGroupUsersAsync(int groupId, int companyId)
    {
        // Only retrieve excluded user IDs if group == delegateAdmin
        IQueryable<int> excludedUserIds = null;
        //bool isDelegateAdmin = groupId == (int)UserGroupRoles.MigrationUser;

        //if (isDelegateAdmin)
        //{
        //excludedUserIds = _dbContext.UserGroupMappings
        //    .Where(x => x.UserGroupId == (int)UserGroupRoles.AccountManager || x.UserGroupId == (int)UserGroupRoles.Checker)
        //    .Select(x => x.UserId)
        //    .Distinct();
        excludedUserIds = _dbContext.ElixirUsers
           .Where(eu => (eu.UserGroupId == (int)UserGroupRoles.AccountManager || eu.UserGroupId == (int)UserGroupRoles.Checker)
                        && eu.CompanyId == companyId)
           .Select(eu => eu.UserId)
           .Distinct();
        // }

        var query = _dbContext.UserGroupMappings
            .Where(ugm => ugm.UserGroupId == groupId)
            .Join(
                _dbContext.Users.Where(u => (bool)u.IsEnabled),
                ugm => ugm.UserId,
                u => u.Id,
                (ugm, u) => new { ugm.UserId, u.FirstName, u.LastName, u.Email }
            );

        //if (isDelegateAdmin)
        //{
            query = query
                .Where(m => !excludedUserIds.Contains(m.UserId))
                .Where(m => !_dbContext.ElixirUsers.Any(cu => cu.UserId == m.UserId && cu.CompanyId == companyId));
        //}

        return await query
            .Select(m => new CompanyUserDto
            {
                UserId = m.UserId,
                UserName = m.FirstName + " " + m.LastName,
                Email = m.Email,
            })
            .ToListAsync();
    }
    public async Task<IEnumerable<CompanyUserDto>> GetAllUserGroupUsersAsync()
    {
        try
        {
            return await _dbContext.UserGroupMappings
                .Where(ugm => !ugm.IsDeleted)
                .Join(
                    _dbContext.Users.Where(u => (bool)u.IsEnabled),
                    ugm => ugm.UserId,
                    u => u.Id,
                    (ugm, u) => new CompanyUserDto
                    {
                        UserId = u.Id,
                        UserName = $"{u.FirstName} {u.LastName}",
                        Email = u.Email,
                        Status = (bool)u.IsEnabled
                    }
                )
                .ToListAsync();
        }
        catch
        {
            throw new Exception(AppConstants.ErrorCodes.CLIENT_ACCOUNT_MANAGERS_RETRIEVAL_FAILED);
        }
    }
    public async Task<List<UserListforUserMappingDto>> GetAllUsersforUserMappingAsync()
    {
        return await _dbContext.Users.Select(u => new UserListforUserMappingDto
        {
            UserID = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Status = (u.IsEnabled ?? false),
            IsEligible = _dbContext.UserGroupMappings.Where(ugm => ugm.UserId == u.Id).Join(_dbContext.UserGroups, ugm => ugm.UserGroupId, ug => ug.Id, (ugm, ug) => ug)
            .Any(ug => ug.GroupType == AppConstants.USER_GROUP_TYPE_DEFAULT || ug.GroupType.StartsWith(AppConstants.USER_GROUP_TYPE_CUSTOM))
            && !_dbContext.UserGroupMappings.Where(ugm => ugm.UserGroupId == (int)Roles.AccountManager || ugm.UserGroupId == (int)Roles.Checker).Any(ugm => ugm.UserId == u.Id)
            ? AppConstants.USER_ELIGIBILITY_USERMAPPING_YES : AppConstants.USER_ELIGIBILITY_USERMAPPING_NO
        })
    .ToListAsync();
    }
    public async Task<IEnumerable<UserMappingGroupsByGroupTypeDto>> GetUserMappingGroupNamesByGroupTypeAsync()
    {
        return await _dbContext.UserGroups
            .Where(g => !g.IsDeleted) // && (g.IsEnabled ?? false)
            .GroupBy(g => g.GroupType)
            .OrderBy(g => g.Key == AppConstants.USER_GROUP_TYPE_DEFAULT ? 0 : 1)
            .Select(grouped => new UserMappingGroupsByGroupTypeDto
            {
                GroupType = grouped.Key,
                GroupNames = grouped
                    .OrderBy(g => g.Id)
                    .Select(g => new UserGroupDto
                    {
                        GroupId = g.Id,
                        GroupName = g.GroupName,
                        GroupType = g.GroupType,
                        CreatedBy = g.CreatedBy,
                        IsSuperAdminCreatedGroup = g.CreatedBy == (int)Roles.SuperAdmin
                    })
                    .ToList()
            })
            .ToListAsync();
    }
    public async Task<UserMappedGroupNamesWithCompaniesDto> GetUserMappedGroupNamesWithCompaniesAsync(int userId)
    {
        try
        {
            // Fetch user details
            var userDetails = await _dbContext.Users
                .Where(user => user.Id == userId && !user.IsDeleted)
                .Select(user => new UserProfileDto
                {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNo = user.PhoneNumber
                })
                .FirstOrDefaultAsync();

            // Fetch user groups
            var userGroups = await _dbContext.UserGroupMappings
                .Where(mapping => mapping.UserId == userId && !mapping.IsDeleted)
                .Join(_dbContext.UserGroups,
                    mapping => mapping.UserGroupId,
                    group => group.Id,
                    (mapping, group) => new { group.GroupType, group.GroupName, group.Id })
                .ToListAsync();


            var groupIds = userGroups
                .Where(x => x.GroupName != AppConstants.ACCOUNTMANAGER_GROUPNAME && x.GroupName != AppConstants.CHECKER_GROUPNAME)
                .Select(x => x.Id)
                .ToList();

            // Prepare and execute company queries sequentially
            var accountManagerCompanies = await _dbContext.ElixirUsers
                .Where(eu => eu.UserId == userId && eu.UserGroupId == (int)UserGroupRoles.AccountManager && eu.ClientId == null)
                .Join(_dbContext.Companies, eu => eu.CompanyId, c => c.Id, (eu, c) => new
                {
                    GroupId = (int)Roles.AccountManager,
                    GroupName = AppConstants.ACCOUNTMANAGER_GROUPNAME,
                    CompanyName = c.CompanyName
                })
                .ToListAsync();

            var checkerCompanies = await _dbContext.ElixirUsers
                .Where(eu => eu.UserId == userId && eu.UserGroupId == (int)UserGroupRoles.Checker && eu.ClientId == null)
                .Join(_dbContext.Companies, eu => eu.CompanyId, c => c.Id, (eu, c) => new
                {
                    GroupId = (int)Roles.Checker,
                    GroupName = AppConstants.CHECKER_GROUPNAME,
                    CompanyName = c.CompanyName
                })
                .ToListAsync();

            var companyUserCompanies = await (from eu in _dbContext.ElixirUsers
                                              join ug in _dbContext.UserGroups on eu.UserGroupId equals ug.Id
                                              join c in _dbContext.Companies on eu.CompanyId equals c.Id
                                              where eu.UserId == userId && eu.ClientId == null && groupIds.Contains(eu.UserGroupId)
                                              select new
                                              {
                                                  GroupId = ug.Id,
                                                  GroupName = ug.GroupName,
                                                  CompanyName = c.CompanyName
                                              }).ToListAsync();

            var allCompanies = accountManagerCompanies
                .Concat(checkerCompanies)
                .Concat(companyUserCompanies)
                .ToList();

            var groupedCompanies = allCompanies
                .GroupBy(c => new { c.GroupId, c.GroupName })
                .Select(group => new UserMappedCompaniesDto
                {
                    groupId = group.Key.GroupId,
                    GroupName = group.Key.GroupName,
                    CompanyNames = group.Select(c => c.CompanyName ?? string.Empty)
                                        .Where(name => name != null)
                                        .Distinct()
                                        .ToList()!,
                    CompanyCount = group.Count()
                })
                .ToList();

            // Group names with company count by group type
            var groupTypeResults = new List<UserGroupNamesWithCount>();

            foreach (var g in userGroups.GroupBy(g => g.GroupType))
            {
                var ids = g.Select(x => x.Id).ToList();

                // Count queries sequentially
                int accountManagerCount = await _dbContext.ElixirUsers.CountAsync(eu => eu.UserId == userId && eu.ClientId == null && eu.UserGroupId == (int)UserGroupRoles.AccountManager);
                int checkerCount = await _dbContext.ElixirUsers.CountAsync(eu => eu.UserId == userId && eu.ClientId == null && eu.UserGroupId == (int)UserGroupRoles.Checker);
                int otherGroupsCount = await _dbContext.ElixirUsers.CountAsync(eu => eu.UserId == userId && eu.ClientId == null && ids.Contains(eu.UserGroupId));

                int companyCount = accountManagerCount + checkerCount + otherGroupsCount;

                groupTypeResults.Add(new UserGroupNamesWithCount
                {
                    GroupType = g.Key,
                    GroupNames = g.Select(x => x.GroupName).Distinct().ToList(),
                    companyCount = companyCount
                });
            }

            // Fetch client mapped Account Managers for this user
            var clientsMappedUsers = await (
                from eu in _dbContext.ElixirUsers
                join cl in _dbContext.Clients on eu.ClientId equals cl.Id
                where eu.UserId == userId
                    && eu.UserGroupId == (int)UserGroupRoles.AccountManager
                    && eu.ClientId != null
                select new
                {
                    GroupId = (int)UserGroupRoles.AccountManager,
                    GroupName = AppConstants.ACCOUNTMANAGER_GROUPNAME,
                    ClientName = cl.ClientName
                }
            )
            .ToListAsync();

            var groupedClients = clientsMappedUsers
                .GroupBy(x => new { x.GroupId, x.GroupName })
                .Select(group => new UserMappedClientsDto
                {
                    // Replace the following code block to get the count of distinct ClientNames
                    groupId = group.Key.GroupId,
                    GroupName = group.Key.GroupName,
                    ClientNames = group.Select(c => c.ClientName ?? string.Empty)
                                        .Where(name => name != null)
                                        .Distinct()
                                        .ToList()!,
                    ClientCount = group.Select(c => c.ClientName ?? string.Empty)
                                       .Where(name => name != null)
                                       .Distinct()
                                       .Count()
                })
                .ToList();

            return new UserMappedGroupNamesWithCompaniesDto
            {
                userGroupNamesWithCounts = groupTypeResults,
                userDetails = userDetails ?? new UserProfileDto(),
                userMappedCompanies = groupedCompanies,
                clientsMappedUsers = groupedClients
            };
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while fetching user mapped group names with companies.", ex);
        }
    }
    public async Task<List<CompanyUserDto>> GetUserMappingUsersByGroupIdAsync(int groupId)
    {
        if (groupId == (int)Roles.DelegateAdmin)
        {
            // Exclude users who are part of AccountManager or Checker groups
            var excludedUserIds = await _dbContext.UserGroupMappings
                .Where(m => m.UserGroupId == (int)Roles.AccountManager || m.UserGroupId == (int)Roles.Checker)
                .Select(m => m.UserId)
                .Distinct()
                .ToListAsync();

            return await _dbContext.UserGroupMappings
                .Where(m => m.UserGroupId == groupId && !excludedUserIds.Contains(m.UserId))
                .Join(
                    _dbContext.Users.Where(u => u.IsEnabled == true),
                    m => m.UserId,
                    u => u.Id,
                    (m, u) => new CompanyUserDto
                    {
                        UserId = u.Id,
                        UserName = u.FirstName + " " + u.LastName,
                        Email = u.Email,
                    })
                .ToListAsync();
        }
        else
        {
            return await _dbContext.UserGroupMappings
                .Where(m => m.UserGroupId == groupId)
                .Join(
                    _dbContext.Users.Where(u => u.IsEnabled == true),
                    m => m.UserId,
                    u => u.Id,
                    (m, u) => new CompanyUserDto
                    {
                        UserId = u.Id,
                        UserName = u.FirstName + " " + u.LastName,
                        Email = u.Email,
                    })
                .ToListAsync();
        }
    }

    public Tuple<List<UserListforUserMappingDto>, int> GetFilteredUserGroupMappingUsersListAsync(bool IsEligibleToBeRemoved, int groupId, int pageNumber, int pageSize, string searchTerm)
    {
        var users = _dbContext.Users.Where(u => !u.IsDeleted).AsQueryable();
        var userGroupMappings = _dbContext.UserGroupMappings.Where(ugm => !ugm.IsDeleted);
        var userGroups = _dbContext.UserGroups.Where(ug => !ug.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            users = users.Where(u =>
                (u.FirstName != null && u.FirstName.Contains(searchTerm)) ||
                (u.LastName != null && u.LastName.Contains(searchTerm)) ||
                (u.Email != null && u.Email.Contains(searchTerm)));

        var companyIdsInDb = _dbContext.Companies.Select(c => c.Id).ToList();

        var singleAccountManager = _dbContext.ElixirUsers
            .Where(eu => eu.UserGroupId == (int)UserGroupRoles.AccountManager)
            .GroupBy(eu => eu.CompanyId)
            .Where(g => g.Count() == 1 && companyIdsInDb.Contains(g.Key))
            .Select(g => new { CompanyId = g.Key, UserId = g.First().UserId })
            .ToList();

        var singleChecker = _dbContext.ElixirUsers
            .Where(eu => eu.UserGroupId == (int)UserGroupRoles.Checker)
            .GroupBy(eu => eu.CompanyId)
            .Where(g => g.Count() == 1 && companyIdsInDb.Contains(g.Key))
            .Select(g => new { CompanyId = g.Key, UserId = g.First().UserId })
            .ToList();

        List<UserListforUserMappingDto> userList;
        int totalCount;

        if (IsEligibleToBeRemoved)
        {
            var filteredUsers = users.Where(u => userGroupMappings.Any(ugm => ugm.UserId == u.Id && ugm.UserGroupId == groupId));
            totalCount = filteredUsers.Count();
            userList = filteredUsers.OrderBy(u => u.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList().Select(u =>
            {
                var companies = groupId == (int)UserGroupRoles.AccountManager
                    ? singleAccountManager.Where(x => x.UserId == u.Id).Select(x => x.CompanyId).ToList()
                    : groupId == (int)UserGroupRoles.Checker
                        ? singleChecker.Where(x => x.UserId == u.Id).Select(x => x.CompanyId).ToList()
                        : new List<int>();
                var isEligible = companies.Any() ? AppConstants.USER_ELIGIBILITY_USERMAPPING_NO : AppConstants.USER_ELIGIBILITY_USERMAPPING_YES;
                var notEligibleReason = companies.Any()
                    ? $"User is the only {(groupId == (int)UserGroupRoles.AccountManager ? "Account Manager" : "Checker")} for: {string.Join(", ", _dbContext.Companies.Where(c => companies.Contains(c.Id)).Select(c => c.CompanyName))}"
                    : AppConstants.USER_ELIGIBILITY_USERMAPPING_YES;
                return new UserListforUserMappingDto
                {
                    UserID = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Status = u.IsEnabled,
                    CreatedAt = u.CreatedAt,
                    IsEligible = isEligible,
                    NotEligibleReason = notEligibleReason
                };
            }).ToList();
        }
        else
        {
            // Updated logic for eligibility between default and custom groups
            var allUsers = users.OrderBy(u => u.Id);
            totalCount = allUsers.Count();
            userList = allUsers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList().Select(u =>
            {
                var mappedGroups = userGroupMappings.Where(ugm => ugm.UserId == u.Id)
                    .Join(userGroups, ugm => ugm.UserGroupId, ug => ug.Id, (ugm, ug) => ug).ToList();

                bool isInSameGroup = groupId > 0 && userGroupMappings.Any(ugm => ugm.UserId == u.Id && ugm.UserGroupId == groupId);
                bool isInDefaultGroup = mappedGroups.Any(ug => ug.GroupType == AppConstants.USER_GROUP_TYPE_DEFAULT);
                bool isInCustomGroup = mappedGroups.Any(ug => ug.GroupType == AppConstants.USER_GROUP_TYPE_CUSTOM);

                // Determine the group type of the current groupId
                var currentGroup = userGroups.FirstOrDefault(g => g.Id == groupId);
                string currentGroupType = currentGroup?.GroupType;

                string isEligible = AppConstants.USER_ELIGIBILITY_USERMAPPING_YES, notEligibleReason = "";

                // If user is already in the same group, not eligible
                if (isInSameGroup)
                {
                    isEligible = AppConstants.USER_ELIGIBILITY_USERMAPPING_NO;
                    notEligibleReason = "User already in group";
                }
                // If user is in any custom group, not eligible for another custom group
                else if (currentGroupType == AppConstants.USER_GROUP_TYPE_CUSTOM && isInCustomGroup)
                {
                    isEligible = AppConstants.USER_ELIGIBILITY_USERMAPPING_NO;
                    notEligibleReason = "User already in another custom group";
                }
                // If user is in a custom group, not eligible for default group
                else if (currentGroupType == AppConstants.USER_GROUP_TYPE_DEFAULT && isInCustomGroup)
                {
                    isEligible = AppConstants.USER_ELIGIBILITY_USERMAPPING_NO;
                    notEligibleReason = "User in a custom group, not eligible for default group";
                }
                // If user is in a default group, not eligible for custom group
                else if (currentGroupType == AppConstants.USER_GROUP_TYPE_CUSTOM && isInDefaultGroup)
                {
                    isEligible = AppConstants.USER_ELIGIBILITY_USERMAPPING_NO;
                    notEligibleReason = "User in a default group, not eligible for custom group";
                }

                return new UserListforUserMappingDto
                {
                    UserID = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Status = u.IsEnabled,
                    CreatedAt = u.CreatedAt,
                    IsDefaultGroup = isInDefaultGroup,
                    IsSameGroup = isInSameGroup,
                    IsEligible = isEligible,
                    NotEligibleReason = notEligibleReason
                };

            }).ToList();
        }
        return new Tuple<List<UserListforUserMappingDto>, int>(userList, totalCount);
    }
    public Tuple<List<CompanyUserDto>, int> GetFilteredUserGroupUsers(int userGroupId, int pageNumber, int pageSize, string searchTerm)
    {
        try
        {
            var query = from u in _dbContext.Users
                        join ugm in _dbContext.UserGroupMappings on u.Id equals ugm.UserId
                        where ugm.UserGroupId == userGroupId && !u.IsDeleted && !ugm.IsDeleted
                        select new CompanyUserDto
                        {
                            UserId = u.Id,
                            UserName = u.FirstName + " " + u.LastName,
                            Email = u.Email,
                        };

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(u => (u.UserName ?? "").Contains(searchTerm) || (u.Email ?? "").Contains(searchTerm));

            int totalCount = query.Count();
            var pagedList = query.OrderBy(u => u.UserId).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new Tuple<List<CompanyUserDto>, int>(pagedList, totalCount);
        }
        catch
        {
            throw new Exception(AppConstants.ErrorCodes.CLIENT_ACCOUNT_MANAGERS_RETRIEVAL_FAILED);
        }
    }


}
