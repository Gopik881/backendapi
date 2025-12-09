using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Enums;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class ElixirUsersRepository : IElixirUsersRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public ElixirUsersRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /* PSEUDOCODE / PLAN (detailed):
       1. Keep existing joins and filters intact to avoid changing current behavior.
       2. Apply search filtering the same way (SQL-translatable parts in the query).
       3. Compute totalCount from the filtered query as before.
       4. Modify ordering so that the latest records appear first:
          - Determine the "latest" timestamp per record as the greater of Role.UpdatedAt and Role.CreatedAt.
          - Order descending by that computed timestamp so newest records are on top.
          - Use a nullable DateTime expression that's translatable to SQL: (ur.r.UpdatedAt > ur.r.CreatedAt ? ur.r.UpdatedAt : (DateTime?)ur.r.CreatedAt)
       5. Keep pagination (Skip/Take) semantics intact.
       6. Keep existing in-memory date-filtering logic for date-format searchTerm (unchanged).
       7. Map the projected anonymous objects to CompanyUserDto the same way as before.
       8. Return the same Tuple<List<CompanyUserDto>, int> so external callers are unaffected.
    */
    public async Task<Tuple<List<CompanyUserDto>, int>> GetFilteredAccountManagersAsync(int CompanyId, string searchTerm, int pageNumber, int pageSize)
    {
        // Only fetch Account Managers (assuming RoleId == (int)Roles.AccountManager)
        var accountManagerRoleId = (int)Roles.AccountManager;

        var query = _dbContext.Users.Where(u => !u.IsDeleted)
            .Join(
                _dbContext.ElixirUsers.Where(eu => eu.CompanyId == CompanyId && !eu.IsDeleted && eu.RoleId == accountManagerRoleId),
                u => u.Id,
                eu => eu.UserId,
                (u, eu) => new { u, eu }
            )
            .Join(
                _dbContext.Roles.Where(r => !r.IsDeleted && r.Id == accountManagerRoleId),
                ueu => ueu.eu.RoleId,
                r => r.Id,
                (ueu, r) => new { u = ueu.u, eu = ueu.eu, r }
            );

        // Apply search filter (only on fields that can be translated to SQL)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchTermLower = searchTerm.ToLower();

            query = query.Where(ur =>
                ur.u.FirstName.ToLower().Contains(searchTermLower) ||
                ur.u.LastName.ToLower().Contains(searchTermLower) ||
                ur.u.Email.ToLower().Contains(searchTermLower) ||
                ur.r.RoleName.ToLower().Contains(searchTermLower) ||
                ((ur.u.IsEnabled ?? false) && "enabled".Contains(searchTermLower)) ||
                (!(ur.u.IsEnabled ?? true) && "disabled".Contains(searchTermLower))
            );
        }

        var totalCount = await query.CountAsync();

        // Fetch data and do date filtering in-memory if needed
        var users = await query
            // ORDER BY latest records first: use the greater of Role.UpdatedAt and Role.CreatedAt
            .OrderByDescending(ur => (ur.r.UpdatedAt.HasValue && ur.r.UpdatedAt.Value > ur.r.CreatedAt)
                                        ? (DateTime?)ur.r.UpdatedAt
                                        : (DateTime?)ur.r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(ur => new
            {
                ur.u.Id,
                ur.u.FirstName,
                ur.u.LastName,
                ur.u.Email,
                ur.r.RoleName,
                ur.u.IsEnabled,
                ur.r.CreatedAt,
                ur.r.UpdatedAt
            })
            .ToListAsync();

        // If searchTerm is a date, filter in-memory
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            bool isDateSearch = DateTime.TryParseExact(searchTerm, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var searchDate);
            if (isDateSearch)
            {
                var searchDateStr = searchDate.ToString("dd/MM/yyyy");
                users = users.Where(ur =>
                    (ur.UpdatedAt.HasValue && ur.UpdatedAt.Value.ToString("dd/MM/yyyy").Contains(searchDateStr)) ||
                    ur.CreatedAt.ToString("dd/MM/yyyy").Contains(searchDateStr)
                ).ToList();
                totalCount = users.Count;
                users = users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        var result = users.Select(ur => new CompanyUserDto
        {
            UserId = ur.Id,
            UserName = $"{ur.FirstName} {ur.LastName}",
            Email = ur.Email,
            RoleName = ur.RoleName,
            Status = (ur.IsEnabled ?? false),
            CreatedOn = ur.CreatedAt,
        }).ToList();

        return new Tuple<List<CompanyUserDto>, int>(result, totalCount);
    }


    public async Task<bool> Company5TabApproveElixirUserDataAsync(int companyId, List<Company5TabElixirUserDto> company5TabElixirUser, int userId, CancellationToken cancellationToken = default)
    {
        // Prepare new ElixirUser entities
        var newContacts = company5TabElixirUser.Any()
            ? company5TabElixirUser.Select(ec => new ElixirUser
            {
                CompanyId = companyId,
                UserGroupId = ec.GroupId,
                UserId = ec.UserId,
                RoleId = (int)Roles.DelegateAdmin,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
            }).ToList()
            : new List<ElixirUser> { new() { CompanyId = companyId, CreatedBy = userId, CreatedAt = DateTime.UtcNow, RoleId = (int)Roles.DelegateAdmin } };

        // Filter out duplicates based on unique key (ClientId, RoleId, CompanyId, UserGroupId, UserId)
        // Assuming ClientId is not used here (null), so check for CompanyId, RoleId, UserGroupId, UserId
        var userGroupIds = newContacts.Select(x => x.UserGroupId).ToList();
        var userIds = newContacts.Select(x => x.UserId).ToList();

        var existing = await _dbContext.ElixirUsers
            .Where(eu =>
                eu.CompanyId == companyId &&
                eu.RoleId == (int)Roles.DelegateAdmin &&
                userGroupIds.Contains(eu.UserGroupId) &&
                userIds.Contains(eu.UserId) &&
                !eu.IsDeleted)
            .Select(eu => new { eu.UserGroupId, eu.UserId })
            .ToListAsync(cancellationToken);

        var filteredContacts = newContacts
            .Where(nc => !existing.Any(e => e.UserGroupId == nc.UserGroupId && e.UserId == nc.UserId))
            .ToList();

        if (filteredContacts.Count == 0)
            return true;

        _dbContext.ElixirUsers.AddRange(filteredContacts);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> ReplaceClientAccountManagersAsync(int userId, int clientId, List<ClientAccountManagersDto> clientAccountManagers, CancellationToken cancellationToken = default)
    {
        // Remove existing ElixirUsers (ClientAccountManagers) for this client
        var existingAccountManagers = await _dbContext.ElixirUsers
            .Where(eu => eu.ClientId == clientId)
            .ToListAsync(cancellationToken);

        if (existingAccountManagers.Count > 0)
        {
            _dbContext.ElixirUsers.RemoveRange(existingAccountManagers);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        if (clientAccountManagers == null || clientAccountManagers.Count == 0)
            return true;

        var utcNow = DateTime.UtcNow;

        // Get valid user group ids
        var validUserGroupIds = await _dbContext.UserGroups.Select(g => g.Id).ToListAsync(cancellationToken);

        // Since ClientAccountManagersDto does not have UserGroupId, use a default or fallback value
        // Here, we assume AccountManager group (2) as default. Adjust as needed.
        const int defaultUserGroupId = (int)UserGroupRoles.AccountManager;

        var newAccountManagers = clientAccountManagers
            .Select(am => new ElixirUser
            {
                ClientId = clientId,
                UserId = am.UserId,
                CreatedAt = utcNow,
                UpdatedAt = utcNow,
                CreatedBy = userId,
                UpdatedBy = userId,
                CompanyId = clientId,
                UserGroupId = validUserGroupIds.Contains(defaultUserGroupId) ? defaultUserGroupId : validUserGroupIds.FirstOrDefault()
            }).ToList();

        if (newAccountManagers.Count == 0)
            return false; // No valid account managers to insert

        await _dbContext.ElixirUsers.AddRangeAsync(newAccountManagers, cancellationToken);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
    public async Task<List<ClientAccountManagersDto>> GetClientAccountManagersByClientIdAsync(int clientId)
    {
        // Step 1: Get the client name for the given clientId
        var clientName = await _dbContext.Clients
            .Where(c => c.Id == clientId)
            .Select(c => c.ClientName)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(clientName))
        {
            // Handle not found case as needed
            return new List<ClientAccountManagersDto>();
        }

        // Step 2: Get all clientIds with the same clientName
        var clientIds = await _dbContext.Clients
            .Where(c => c.ClientName == clientName)
            .Select(c => c.Id)
            .ToListAsync();

        // Step 3: Fetch all ElixirUsers for those clientIds
        var elixirUsers = await _dbContext.ElixirUsers
            .Where(eu => clientIds.Contains(eu.ClientId ?? 0) && !eu.IsDeleted)
            .ToListAsync();

        // Get the AccountManager group name (assume Id = 2)
        var accountManagerGroup = await _dbContext.UserGroups
            .FirstOrDefaultAsync(g => g.Id == (int)UserGroupRoles.AccountManager);

        var groupName = accountManagerGroup?.GroupName;

        // Get all userIds
        var userIds = elixirUsers.Select(eu => eu.UserId).ToList();

        // Get user details
        var users = await _dbContext.Users
            .Where(u => userIds.Contains(u.Id) && !u.IsDeleted)
            .ToListAsync();

        // Map to DTOs with distinct users
        var result = elixirUsers
            .Join(users, eu => eu.UserId, u => u.Id, (eu, user) => new { eu, user })
            .GroupBy(x => x.eu.UserId)
            .Select(g =>
            {
                var x = g.First();
                return new ClientAccountManagersDto
                {
                    ClientAccountManagerId = x.eu.UserId,
                    ClientId = x.eu.ClientId,
                    GroupId = (int)UserGroupRoles.AccountManager,
                    UserId = x.eu.UserId,
                    GroupName = groupName,
                    Users = x.user != null ? new List<string> { $"{x.user.FirstName} {x.user.LastName}" } : new List<string>()
                };
            })
            .ToList();

        return result;
    }

    public async Task<ElixirUserListDto> GetElixirUserListsByCompanyIdAsync(int companyId)
    {
        // Get company, onboarding status, and group count
        var companyData = await (from company in _dbContext.Companies
                                 join onboardingStatus in _dbContext.CompanyOnboardingStatuses
                                     on company.Id equals onboardingStatus.CompanyId into onboardingStatusJoin
                                 from onboardingStatus in onboardingStatusJoin.DefaultIfEmpty()
                                 where company.Id == companyId
                                 select new
                                 {
                                     company.Id,
                                     company.CompanyName,
                                     company.IsUnderEdit,
                                     company.IsEnabled,
                                     OnboardingStatus = onboardingStatus.OnboardingStatus,
                                     CompanyStatus = onboardingStatus.IsActive,
                                     GroupsCount = _dbContext.ElixirUsers
                                         .Where(eu => eu.CompanyId == company.Id && !eu.IsDeleted)
                                         .Select(eu => eu.UserGroupId)
                                         .Distinct()
                                         .Count()
                                 }).FirstOrDefaultAsync();

        // Account Managers: RoleId 2, Default User Group
        var accountManagerUsersList = await (from eu in _dbContext.ElixirUsers
                                             join u in _dbContext.Users on eu.UserId equals u.Id
                                             join ug in _dbContext.UserGroups on eu.UserGroupId equals ug.Id
                                             where eu.CompanyId == companyId
                                                   && !eu.IsDeleted
                                                   && eu.UserGroupId == (int)UserGroupRoles.AccountManager
                                                   && ug.GroupType == AppConstants.USER_GROUP_TYPE_DEFAULT
                                                   && !u.IsDeleted
                                             select new DefaultUserGroupUserDto
                                             {
                                                 RoleId = (int)Roles.AccountManager, // Assuming RoleId is needed
                                                 UserGroupId = (int)UserGroupRoles.AccountManager,
                                                 UserId = u.Id,
                                                 UserName = u.FirstName + " " + u.LastName,
                                                 Email = u.Email
                                             }).Distinct().ToListAsync();

        // Checkers: RoleId 3, Default User Group
        var checkerUsersList = await (from eu in _dbContext.ElixirUsers
                                      join u in _dbContext.Users on eu.UserId equals u.Id
                                      join ug in _dbContext.UserGroups on eu.UserGroupId equals ug.Id
                                      where eu.CompanyId == companyId
                                            && !eu.IsDeleted
                                            && eu.UserGroupId == (int)UserGroupRoles.Checker
                                            && ug.GroupType == AppConstants.USER_GROUP_TYPE_DEFAULT
                                            && !u.IsDeleted
                                      select new DefaultUserGroupUserDto
                                      {
                                          RoleId = (int)Roles.Checker, // Assuming RoleId is needed
                                          UserGroupId = (int)UserGroupRoles.Checker,
                                          UserId = u.Id,
                                          UserName = u.FirstName + " " + u.LastName,
                                          Email = u.Email
                                      }).Distinct().ToListAsync();

        // Custom User Groups: Any Role, Custom User Group
        var customUserGroupList = await (from eu in _dbContext.ElixirUsers
                                         join u in _dbContext.Users on eu.UserId equals u.Id
                                         join ug in _dbContext.UserGroups on eu.UserGroupId equals ug.Id
                                         where eu.CompanyId == companyId
                                               && !eu.IsDeleted
                                               && (ug.GroupType == AppConstants.USER_GROUP_TYPE_CUSTOM || ug.GroupName == AppConstants.USER_GROUP_NAME_MIGRATIONUSER)
                                               && !u.IsDeleted
                                         select new CustomUserGroupUserDto
                                         {
                                             UserGroupId = ug.Id,
                                             UserGroupName = ug.GroupName,
                                             UserId = u.Id,
                                             UserName = u.FirstName + " " + u.LastName,
                                             Email = u.Email
                                         }).Distinct().ToListAsync();

        return new ElixirUserListDto
        {
            CompanyId = companyData?.Id,
            CompanyName = companyData?.CompanyName,
            AccountManagerUsersList = accountManagerUsersList,
            CheckerUsersList = checkerUsersList,
            CustomUserGroupList = customUserGroupList,
            Status = companyData?.IsEnabled,
            OnboardingStatus = companyData?.OnboardingStatus,
            CompanyStatus = companyData?.CompanyStatus,
            GroupsCount = companyData?.GroupsCount,
            isUnderEdit = companyData?.IsUnderEdit
        };
    }
    /* PSEUDOCODE / PLAN:
    1. Read ScreenName parameter and Trim() it.
    2. If ScreenName is null or empty -> leave isCompanyList, isCompanyOnboardingList, isCompanyCreation as false.
    3. Else compare ScreenName case-insensitively:
       - "Onboardinglist"  -> isCompanyOnboardingList = true
       - "CompanyList"     -> isCompanyList = true
       - "CompanyCreation" -> isCompanyCreation = true
    4. Proceed with existing logic to build lists, using the flags to populate allowedSubMenuIds.
    */

    public async Task<ElixirUserListDto> GetUserListsFromUserGroupMappingAsync(string? ScreenName = "")
    {
        bool isCompanyList = false;
        bool isCompanyOnboardingList = false;
        bool isCompanyCreation = false;

        var screen = ScreenName?.Trim();
        if (!string.IsNullOrEmpty(screen))
        {
            if (string.Equals(screen, "Onboardinglist", StringComparison.OrdinalIgnoreCase))
                isCompanyOnboardingList = true;
            else if (string.Equals(screen, "CompanyList", StringComparison.OrdinalIgnoreCase))
                isCompanyList = true;
            else if (string.Equals(screen, "CompanyCreation", StringComparison.OrdinalIgnoreCase))
                isCompanyCreation = true;
        }

        var defaultType = AppConstants.USER_GROUP_TYPE_DEFAULT;
        var customType = AppConstants.USER_GROUP_TYPE_CUSTOM;

        var accountManagerUsersList = await _dbContext.UserGroupMappings
            .Join(_dbContext.Users, ugm => ugm.UserId, u => u.Id, (ugm, u) => new { ugm, u })
            .Join(_dbContext.UserGroups, x => x.ugm.UserGroupId, ug => ug.Id, (x, ug) => new { x.ugm, x.u, ug })
            .Where(x => x.ug.GroupType == defaultType && x.ug.Id == (int)UserGroupRoles.AccountManager && !x.u.IsDeleted && (x.u.IsEnabled ?? false))
            .Select(x => new DefaultUserGroupUserDto { RoleId = (int)Roles.AccountManager, UserGroupId = (int)UserGroupRoles.AccountManager, UserId = x.u.Id, UserName = x.u.FirstName + " " + x.u.LastName, Email = x.u.Email })
            .Distinct()
            .ToListAsync();

        var checkerUsersList = await _dbContext.UserGroupMappings
            .Join(_dbContext.Users, ugm => ugm.UserId, u => u.Id, (ugm, u) => new { ugm, u })
            .Join(_dbContext.UserGroups, x => x.ugm.UserGroupId, ug => ug.Id, (x, ug) => new { x.ugm, x.u, ug })
            .Where(x => x.ug.GroupType == defaultType && x.ug.Id == (int)UserGroupRoles.Checker && !x.u.IsDeleted && (x.u.IsEnabled ?? false))
            .Select(x => new DefaultUserGroupUserDto { RoleId = (int)Roles.Checker, UserGroupId = (int)UserGroupRoles.Checker, UserId = x.u.Id, UserName = x.u.FirstName + " " + x.u.LastName, Email = x.u.Email })
            .Distinct()
            .ToListAsync();

        // Build allowed SubMenuItemIds based on flags
        var allowedSubMenuIds = new List<int>();
        if (isCompanyList)
            allowedSubMenuIds.Add(AppConstants.SUBMENUITEM_ID_COMPANY_LIST);
        if (isCompanyOnboardingList)
            allowedSubMenuIds.Add(AppConstants.SUBMENUITEM_ID_COMPANY_ONBOARDING_LIST);
        if (isCompanyCreation)
            allowedSubMenuIds.Add(AppConstants.SUBMENUITEM_ID_COMPANY_ONBOARDING_LIST);
        // Remove duplicates (optional)
        allowedSubMenuIds = allowedSubMenuIds.Distinct().ToList();

        var customGroupsQuery = from ugm in _dbContext.UserGroupMappings
                                join ug in _dbContext.UserGroups on ugm.UserGroupId equals ug.Id
                                join cls in _dbContext.UserGroupMenuMappings on ugm.UserGroupId equals cls.UserGroupId
                                where ug.GroupType == customType
                                      // If flags supplied -> require cls.SubMenuItemId in allowed list
                                      // If no flags supplied -> fallback to previous behavior (12 or 13)
                                      && (allowedSubMenuIds.Count > 0
                                            ? allowedSubMenuIds.Contains(cls.SubMenuItemId)
                                            : (cls.SubMenuItemId == AppConstants.SUBMENUITEM_ID_COMPANY_ONBOARDING_LIST ||
                                              cls.SubMenuItemId == AppConstants.SUBMENUITEM_ID_COMPANY_LIST))
                                      && !(cls.IsAllCompanies ?? false)
                                      && (cls.ViewOnlyAccess ?? false)
                                select new CustomUserGroupUserDto
                                {
                                    UserGroupId = ug.Id,
                                    UserGroupName = ug.GroupName,
                                    Email = string.Empty
                                };

        var customGroupsList = await customGroupsQuery.Distinct().ToListAsync();
        var migrationGroupList = await (
                from ugm in _dbContext.UserGroupMappings
                join ug in _dbContext.UserGroups on ugm.UserGroupId equals ug.Id
                join u in _dbContext.Users on ugm.UserId equals u.Id
                where ugm.UserGroupId == (int)UserGroupRoles.MigrationUser
                    && !u.IsDeleted
                select new CustomUserGroupUserDto
                {
                    UserGroupId = ug.Id,
                    UserGroupName = ug.GroupName,
                    UserId = u.Id,
                    UserName = u.FirstName + " " + u.LastName,
                    Email = u.Email
                }).GroupBy(dto => dto.UserGroupName)
                .Select(g => g.First())
                .ToListAsync();

        return new ElixirUserListDto
        {
            AccountManagerUsersList = accountManagerUsersList,
            CheckerUsersList = checkerUsersList,
            CustomUserGroupList = customGroupsList.Union(migrationGroupList).ToList()
        };
    }
    


}
