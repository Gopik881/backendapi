using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Enums;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class CompaniesRepository : ICompaniesRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public CompaniesRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<bool> ExistsWithCompanyCodeAsync(string companyCode)
    {
        // Check in both CompanyHistories and Companies tables for a non-deleted record with the given company code
        var existsInHistories = await _dbContext.CompanyHistories
            .AnyAsync(c => EF.Functions.Like(c.CompanyCode, companyCode) && !c.IsDeleted);

        var existsInCompanies = await _dbContext.Companies
            .AnyAsync(c => EF.Functions.Like(c.CompanyCode, companyCode) && !c.IsDeleted);

        return existsInHistories || existsInCompanies;
    }
    public async Task<bool> ExistsWithCompanyNameAsync(string companyName)
    {
        var existsInHistories = await _dbContext.CompanyHistories
            .AnyAsync(c => EF.Functions.Like(c.CompanyName, companyName) && !c.IsDeleted);
        var existsInCompanies = await _dbContext.Companies
            .AnyAsync(c => EF.Functions.Like(c.CompanyName, companyName) && !c.IsDeleted);
        // Return true if either table contains a non-deleted record with the given company name
        return existsInHistories || existsInCompanies;
    }
    public async Task<bool> ExistsWithCompanyNameForUpdateAsync(string companyName, int companyId)
    {
        var existsInHistories = await _dbContext.CompanyHistories
            .AnyAsync(c => EF.Functions.Like(c.CompanyName, companyName) && c.CompanyId != companyId && !c.IsDeleted);
        var existsInCompanies = await _dbContext.Companies
            .AnyAsync(c => EF.Functions.Like(c.CompanyName, companyName) && c.Id != companyId && !c.IsDeleted);
        // Return true if either table contains a non-deleted record with the given company name and different ID
        return existsInHistories || existsInCompanies;
    }

    public async Task<int> FindCompanyByCodeAsync(string companyCode)
    {
        // If companyCode is "Tmi" (case-insensitive), always check directly in Companies table (predefined dataset)
        if (string.Equals(companyCode, "Tmi", StringComparison.OrdinalIgnoreCase))
        {
            var tmiCompany = await _dbContext.Companies
                .Where(c => !c.IsDeleted && EF.Functions.Like(c.CompanyCode, companyCode))
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            return tmiCompany;
        }

        // For other company codes, check only in Companies and CompanyHistories where OnboardingStatus is Approved
        // Instead of fetching all approvedCompanyIds, filter with a join for better performance

        // Search in Companies with Approved status
        var companyId = await (
            from c in _dbContext.Companies
            join cos in _dbContext.CompanyOnboardingStatuses
                on c.Id equals cos.CompanyId
            where !c.IsDeleted
                && cos.OnboardingStatus == AppConstants.ONBOARDING_STATUS_APPROVED
                && !cos.IsDeleted
                && c.CompanyCode == companyCode
            select c.Id
        ).FirstOrDefaultAsync();

        if (companyId != 0)
            return companyId;

        // Search in CompanyHistories with Approved status
        var companyIdFromHistory = await (
            from ch in _dbContext.CompanyHistories
            join cos in _dbContext.CompanyOnboardingStatuses
                on ch.CompanyId equals cos.CompanyId
            where !ch.IsDeleted
                && ch.CompanyId.HasValue
                && cos.OnboardingStatus == AppConstants.ONBOARDING_STATUS_APPROVED
                && !cos.IsDeleted
                && ch.CompanyCode == companyCode
            select ch.CompanyId.Value
        ).FirstOrDefaultAsync();

        return companyIdFromHistory;
    }

    public async Task<bool> UpdateCompanyUnderEditAsync(int companyId, int userId, bool isUnderEdit)
    {
        var company = await _dbContext.Companies.Where(c => c.Id == companyId && !c.IsDeleted).FirstOrDefaultAsync();
        if (company == null) return false;
        company.IsUnderEdit = isUnderEdit;
        //company.LastUpdatedBy = userId;
        //company.CreatedBy = userId;
        _dbContext.Companies.Update(company);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateCompanyLastUpdatedBy(int companyId, int userId)
    {
        var company = await _dbContext.Companies.Where(c => c.Id == companyId && !c.IsDeleted).FirstOrDefaultAsync();
        if (company == null) return false;
        company.LastUpdatedBy = userId;
        company.CreatedBy = userId;
        company.LastUpdatedOn = DateTime.UtcNow;
        company.UpdatedAt = DateTime.UtcNow;
        _dbContext.Companies.Update(company);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateCompanyHistoryLastUpdatedBy(int companyId, int userId)
    {
        var company = await _dbContext.CompanyHistories.Where(c => c.Id == companyId && !c.IsDeleted).FirstOrDefaultAsync();
        if (company == null) return false;
        company.LastUpdatedBy = userId;
        company.CreatedBy = userId;
        _dbContext.CompanyHistories.Update(company);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    // Pseudocode / Plan (detailed):
    // 1. We need to ensure newly created or recently updated companies appear on top when returning paged results.
    // 2. The ranking should primarily consider LastUpdatedOn. If LastUpdatedOn is null, fall back to CreatedOn to ensure newly created records appear near the top.
    // 3. Do not change existing filtering logic. Only add ordering by (LastUpdatedOn ?? CreatedOn) descending just before pagination is applied.
    // 4. Apply the ordering in both branches of the method (IsUnderEdit == false and IsUnderEdit == true).
    // 5. Preserve existing search filtering and pagination behavior. Compute totalCount after applying search filters and ordering, then take the requested page.
    // 6. Keep everything else unchanged to avoid disturbing existing functionality.
    //
    // Implementation notes:
    // - After materializing the query to a list, perform the search filter in-memory (existing behavior).
    // - Then order the filtered results by (LastUpdatedOn ?? CreatedOn) descending.
    // - Finally set totalCount and apply Skip/Take for pagination.

    public async Task<Tuple<List<CompanySummaryDto>, int>> GetPagedSuperAdminCompaniesSummaryAsync(int userId, bool IsUnderEdit, bool IsSuperUser, string searchTerm, int pageNumber, int pageSize)
    {
        int totalCount = 0;
        List<CompanySummaryDto> companies = new List<CompanySummaryDto>();
        if (!IsUnderEdit)
        {
            // Old Master Id : 12 - Super Admin
            var companiesQuery = _dbContext.Companies.Where(cm => !cm.IsDeleted && (cm.IsEnabled ?? false))
                   .Join(_dbContext.Clients, cm => cm.ClientId, cl => cl.Id, (cm, cl) => new { cm, cl })
                   .Join(_dbContext.Accounts, c => c.cm.Id, a => a.CompanyId, (c, a) => new CompanySummaryDto
                   {
                       CompanyID = c.cm.Id,
                       CompanyName = c.cm.CompanyName,
                       ClientCode = (string.IsNullOrEmpty(c.cm.ClientName) || c.cm.ClientName == AppConstants.NOTAVAILABLE) ? AppConstants.NOTAVAILABLE : c.cl.ClientCode,
                       CompanyStorageConsumedGB = c.cm.CompanyStorageConsumedGb ?? 0,
                       DisplaydCompanyStorageConsumedGB = "0/" + c.cm.CompanyStorageConsumedGb,
                       CompanyStorageTotalGB = c.cm.CompanyStorageTotalGb ?? 0,
                       DisplaydPerUserStorageMB = "0/" + c.cm.CompanyStorageTotalGb,
                       IsEnabled = c.cm.IsEnabled ?? false,
                       Users = 0, //Place Holders for now, it will be part of FRD2
                       UserGroups = 0,  //Place Holders for now, it will be part of FRD2
                       ClientId = c.cl.Id,
                       ClientName = c.cm.ClientName ?? AppConstants.NOTAVAILABLE,
                       IsUnderEdit = c.cm.IsUnderEdit ?? false,
                       PerUserStorageMB = a.PerUserStorageMb ?? 0,
                       ClientCompaniesCount = _dbContext.ClientCompaniesMappings
                             .Where(cl => cl.ClientId == c.cl.Id)
                             .Select(cl => cl.CompanyId)
                             .Distinct()
                             .Count(),
                       ClientAccountManagersCount = _dbContext.ElixirUsers
                             .Where(cl => cl.ClientId == c.cl.Id)
                             .Select(cl => cl.UserId)
                             .Distinct()
                             .Count(),
                       CreatedOn = (DateTime?)c.cm.CreatedAt ?? DateTime.UtcNow,
                       LastUpdatedOn = (from ch in _dbContext.CompanyHistories
                                        where ch.CompanyId == c.cm.Id
                                        orderby ch.Version descending
                                        select ch.LastUpdatedOn).FirstOrDefault() ?? c.cm.LastUpdatedOn,
                       LastUpdatedBy =
                                    (_dbContext.CompanyOnboardingStatuses
                                        .Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_NEW))
                                    ? _dbContext.SuperUsers
                                        .Select(su => su.FirstName + " " + su.LastName)
                                        .FirstOrDefault() ?? string.Empty
                                    : (_dbContext.CompanyOnboardingStatuses
                                        .Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_APPROVED))
                                    ? (from comp in _dbContext.Companies
                                       join u in _dbContext.Users on comp.CreatedBy equals u.Id
                                       where comp.Id == c.cm.Id && !u.IsDeleted
                                       select u.FirstName + " " + u.LastName).FirstOrDefault() ?? string.Empty
                                    : (_dbContext.CompanyOnboardingStatuses
                                        .Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_REJECTED))
                                    ? (from comp in _dbContext.CompanyOnboardingStatuses
                                       join u in _dbContext.Users on comp.UpdatedBy equals u.Id
                                       where comp.CompanyId == c.cm.Id && !u.IsDeleted
                                       select u.FirstName + " " + u.LastName).FirstOrDefault() ?? string.Empty
                                    : (from ch in _dbContext.CompanyHistories
                                       join u in _dbContext.Users on ch.CreatedBy equals u.Id
                                       where ch.CompanyId == c.cm.Id && !u.IsDeleted
                                       orderby ch.Version descending
                                       select u.FirstName + " " + u.LastName).FirstOrDefault() ?? string.Empty,
                       CreatedBy =
                         (
                             from createdById in
                                 _dbContext.CompanyOnboardingStatuses
                                     .Where(cos => cos.CompanyId == c.cm.Id)
                                     .OrderByDescending(cos => cos.CreatedAt)
                                     .Select(cos => cos.CreatedBy)
                                     .Take(1)
                             select
                                 _dbContext.Users
                                     .Where(u => u.Id == createdById && !u.IsDeleted)
                                     .Select(u => u.FirstName + " " + u.LastName)
                                     .FirstOrDefault()
                                 ?? _dbContext.SuperUsers
                                     .Where(su => su.Id == createdById)
                                     .Select(su => su.FirstName + " " + su.LastName)
                                     .FirstOrDefault()
                                 ?? string.Empty
                         ).FirstOrDefault() ?? string.Empty,

                       CompanyStatus = _dbContext.CompanyOnboardingStatuses
                           .Where(cos => cos.CompanyId == c.cm.Id)
                           .Select(cos => (bool?)cos.IsActive)
                           .FirstOrDefault(),
                   });

            var companiesList = await companiesQuery.ToListAsync();

            companies = companiesList
                .Where(c =>
                    (!string.IsNullOrEmpty(c.CompanyName) && c.CompanyName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.ClientName) && c.ClientName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStorageConsumedGB.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStorageTotalGB.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.UserGroups.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Users.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.DisplaydCompanyStorageConsumedGB) && c.DisplaydCompanyStorageConsumedGB.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||

                    (c.CompanyStatus == true && "enabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStatus == false && "disabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                )
                .ToList();

            // Order by LastUpdatedOn (fallback to CreatedOn) descending so recently created/updated records appear first
            companies = companies
                .OrderByDescending(c => c.LastUpdatedOn ?? c.CreatedOn ?? DateTime.MinValue)
                .ToList();

            totalCount = companies.Count;
            companies = companies
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
        else
        {
            // Old Master Id : 12 - Super Admin
            var companiesQuery = _dbContext.Companies.Where(cm => !cm.IsDeleted && cm.IsUnderEdit == IsUnderEdit && (cm.IsEnabled ?? false))
                .Join(_dbContext.Clients, cm => cm.ClientId, cl => cl.Id, (cm, cl) => new { cm, cl })
                .Join(_dbContext.Accounts, c => c.cm.Id, a => a.CompanyId, (c, a) => new CompanySummaryDto
                {
                    CompanyID = c.cm.Id,
                    CompanyName = c.cm.CompanyName,
                    ClientCode = (string.IsNullOrEmpty(c.cm.ClientName) || c.cm.ClientName == AppConstants.NOTAVAILABLE) ? AppConstants.NOTAVAILABLE : c.cl.ClientCode,
                    CompanyStorageConsumedGB = c.cm.CompanyStorageConsumedGb ?? 0,
                    DisplaydCompanyStorageConsumedGB = "0/" + c.cm.CompanyStorageConsumedGb,
                    CompanyStorageTotalGB = c.cm.CompanyStorageTotalGb ?? 0,
                    DisplaydPerUserStorageMB = "0/" + c.cm.CompanyStorageTotalGb,
                    IsEnabled = c.cm.IsEnabled ?? false,
                    Users = 0, //Place Holders for now, it will be part of FRD2
                    UserGroups = 0,  //Place Holders for now, it will be part of FRD2
                    ClientId = c.cl.Id,
                    ClientName = c.cm.ClientName ?? AppConstants.NOTAVAILABLE,
                    IsUnderEdit = c.cm.IsUnderEdit ?? false,
                    PerUserStorageMB = a.PerUserStorageMb ?? 0,
                    ClientCompaniesCount = _dbContext.ClientCompaniesMappings
                             .Where(cl => cl.ClientId == c.cl.Id)
                             .Select(cl => cl.CompanyId)
                             .Distinct()
                             .Count(),
                    ClientAccountManagersCount = _dbContext.ElixirUsers
                             .Where(cl => cl.ClientId == c.cl.Id)
                             .Select(cl => cl.UserId)
                             .Distinct()
                             .Count(),
                    CreatedOn = (DateTime?)c.cm.CreatedAt ?? DateTime.UtcNow,
                    LastUpdatedOn = (from ch in _dbContext.CompanyHistories
                                     where ch.CompanyId == c.cm.Id
                                     orderby ch.Version descending
                                     select ch.LastUpdatedOn).FirstOrDefault() ?? c.cm.LastUpdatedOn,
                    LastUpdatedBy =
                        // If OnBoardingStatus is "New", get CreatedBy from SuperUsers
                        (_dbContext.CompanyOnboardingStatuses.Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_NEW))
                            ? (
                                from su in _dbContext.SuperUsers
                                select su.FirstName + " " + su.LastName
                              ).FirstOrDefault() ?? string.Empty
                        // If OnBoardingStatus is "Approved", get CreatedBy from Companies table
                        : (_dbContext.CompanyOnboardingStatuses.Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_APPROVED))
                            ? (
                                from comp in _dbContext.Companies
                                join u in _dbContext.Users on comp.CreatedBy equals u.Id
                                where comp.Id == c.cm.Id && !u.IsDeleted
                                select u.FirstName + " " + u.LastName
                              ).FirstOrDefault() ?? string.Empty
                        // Else, get CreatedBy from latest CompanyHistory for the company
                        : (
                            from ch in _dbContext.CompanyHistories
                            join u in _dbContext.Users on ch.CreatedBy equals u.Id
                            where ch.CompanyId == c.cm.Id && !u.IsDeleted
                            orderby ch.Version descending
                            select u.FirstName + " " + u.LastName
                          ).FirstOrDefault() ?? string.Empty,
                    CreatedBy =
                         (
                             from createdById in
                                 _dbContext.CompanyOnboardingStatuses
                                     .Where(cos => cos.CompanyId == c.cm.Id)
                                     .OrderByDescending(cos => cos.CreatedAt)
                                     .Select(cos => cos.CreatedBy)
                                     .Take(1)
                             select
                                 _dbContext.Users
                                     .Where(u => u.Id == createdById && !u.IsDeleted)
                                     .Select(u => u.FirstName + " " + u.LastName)
                                     .FirstOrDefault()
                                 ?? _dbContext.SuperUsers
                                     .Where(su => su.Id == createdById)
                                     .Select(su => su.FirstName + " " + su.LastName)
                                     .FirstOrDefault()
                                 ?? string.Empty
                         ).FirstOrDefault() ?? string.Empty,
                    CompanyStatus = _dbContext.CompanyOnboardingStatuses
                           .Where(cos => cos.CompanyId == c.cm.Id)
                           .Select(cos => (bool?)cos.IsActive)
                           .FirstOrDefault(),
                });

            //// Apply pagination
            companies = await companiesQuery
                .ToListAsync();

            companies = companies
                .Where(c =>
                    (!string.IsNullOrEmpty(c.CompanyName) && c.CompanyName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.ClientName) && c.ClientName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStorageConsumedGB.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStorageTotalGB.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.UserGroups.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Users.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.DisplaydCompanyStorageConsumedGB) && c.DisplaydCompanyStorageConsumedGB.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStatus == true && "enabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStatus == false && "disabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                )
                .ToList();

            // Order by LastUpdatedOn (fallback to CreatedOn) descending so recently created/updated records appear first
            companies = companies
                .OrderByDescending(c => c.LastUpdatedOn ?? c.CreatedOn ?? DateTime.MinValue)
                .ToList();

            totalCount = companies.Count;
            companies = companies
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        return new Tuple<List<CompanySummaryDto>, int>(companies, totalCount);

    }
    // Pseudocode / Plan (detailed):
    // 1. Materialize the company lists as before to avoid EF translation issues.
    // 2. Apply the existing in-memory search filters (preserve all filtering logic).
    // 3. Order the filtered results by (LastUpdatedOn ?? CreatedOn) descending so that newly created or recently updated records appear first.
    //    - Use DateTime.MinValue as a final fallback for null safety.
    //    - Retain secondary ordering by ClientName to keep stable UI ordering for equal timestamps.
    // 4. Compute totalCount from the ordered filtered list.
    // 5. Apply pagination (Skip/Take) on the ordered list.
    // 6. Do this in both branches (IsUnderEdit == false and IsUnderEdit == true) without changing any other business logic.

    public async Task<Tuple<List<CompanyTMISummaryDto>, int>> GetPagedTMIUsersCompaniesSummaryAsync(int userId, bool IsUnderEdit, string searchTerm, int pageNumber, int pageSize)
    {
        // Returns true if the user is in a custom group (not a TMI user), false otherwise
        bool isCustomGroupUser = await _dbContext.UserGroups
            .AnyAsync(ug => !ug.IsDeleted
                && EF.Functions.Like(ug.GroupType, AppConstants.USER_GROUP_TYPE_CUSTOM)
                && _dbContext.UserGroupMappings.Any(ugm => ugm.UserId == userId && !ugm.IsDeleted && ugm.UserGroupId == ug.Id));

        // Collect the UserGroup Id for the User if he is in Default User Group
        var userGroupIds = await _dbContext.UserGroups
            .Where(ug => !ug.IsDeleted && EF.Functions.Like(ug.GroupType, AppConstants.USER_GROUP_TYPE_DEFAULT) && _dbContext.UserGroupMappings.Any(ugm => ugm.UserId == userId && !ugm.IsDeleted && ugm.UserGroupId == ug.Id))
            .Select(ug => ug.Id).ToListAsync();

        // Materialize ElixirUsers and UserGroups for the user and relevant companies
        var elixirUsersForUser = await _dbContext.ElixirUsers
            .Where(eu => eu.UserId == userId && !eu.IsDeleted && userGroupIds.Contains(eu.UserGroupId) && eu.ClientId == null)
            .ToListAsync();

        var userGroupIdsForUser = elixirUsersForUser.Select(eu => eu.UserGroupId).Distinct().ToList();
        var userGroupsDict = await _dbContext.UserGroups
            .Where(ug => userGroupIdsForUser.Contains(ug.Id))
            .ToDictionaryAsync(ug => ug.Id, ug => ug.GroupName);

        int totalCount = 0;
        List<CompanyTMISummaryDto> companies = new List<CompanyTMISummaryDto>();

        // Get all company IDs the user has access to
        var accessibleCompanyIds = elixirUsersForUser.Select(eu => eu.CompanyId).Distinct().ToList();

        if (!IsUnderEdit)
        {
            // Materialize companies with accounts
            var companiesWithAccountsQuery = _dbContext.Companies
                .Where(cm => accessibleCompanyIds.Contains(cm.Id) && !cm.IsDeleted && (cm.IsEnabled ?? false))
                .Join(_dbContext.Clients, cm => cm.ClientId, cl => cl.Id, (cm, cl) => new { cm, cl })
                .Join(_dbContext.Accounts, c => c.cm.Id, a => a.CompanyId, (c, a) => new { c.cm, c.cl, a })
                .ToList(); // Materialize to avoid translation issues

            var companiesWithAccountsList = companiesWithAccountsQuery.Select(c => new CompanyTMISummaryDto
            {
                IsActive = true,
                CompanyID = c.cm.Id,
                CompanyName = c.cm.CompanyName ?? AppConstants.NOTAVAILABLE,
                ClientCode = string.IsNullOrEmpty(c.cm.ClientName) || c.cm.ClientName == AppConstants.NOTAVAILABLE ? AppConstants.NOTAVAILABLE : c.cl.ClientCode,
                CompanyStorageConsumedGB = c.cm.CompanyStorageConsumedGb ?? 0,
                CompanyStorageTotalGB = c.cm.CompanyStorageTotalGb ?? 0,
                DisplaydCompanyStorageConsumedGB = "0/" + c.cm.CompanyStorageConsumedGb,
                DisplaydPerUserStorageMB = "0/" + c.cm.CompanyStorageTotalGb,
                IsEnabled = c.cm.IsEnabled ?? false,
                Users = 0,
                UserGroups = 0,
                ClientId = c.cl.Id,
                ClientName = string.IsNullOrEmpty(c.cm.ClientName) || c.cl.ClientName == AppConstants.NOTAVAILABLE ? AppConstants.NOTAVAILABLE : c.cl.ClientName,
                IsUnderEdit = c.cm.IsUnderEdit ?? false,
                PerUserStorageMB = c.a.PerUserStorageMb ?? 0,
                ActiveSince = _dbContext.CompanyOnboardingStatuses.Where(cos => cos.CompanyId == c.cm.Id && !cos.IsDeleted).OrderBy(cos => cos.CreatedAt).Select(cos => (DateTime?)cos.CreatedAt).FirstOrDefault() ?? DateTime.UtcNow,
                Module = (_dbContext.ModuleMappings.Where(mm => mm.CompanyId == c.cm.Id && !mm.IsDeleted).Select(mm => mm.ModuleId).Distinct().Count() - 1) < 0
                            ? string.Empty
                            : $"{AppConstants.MODULE_CORE_HR}+ {_dbContext.ModuleMappings.Where(mm => mm.CompanyId == c.cm.Id && !mm.IsDeleted).Select(mm => mm.ModuleId).Distinct().Count() - 1}",
                AdminName = _dbContext.CompanyAdminUsers.Where(cau => !cau.IsDeleted && cau.CompanyId == c.cm.Id).Select(cau => cau.FirstName + " " + cau.LastName).FirstOrDefault()
                    ?? _dbContext.Users.Where(u => !u.IsDeleted)
                        .Join(_dbContext.ElixirUsers.Where(eu => !eu.IsDeleted && eu.RoleId == (int)Roles.ClientAccoutManagers), u => u.Id, eu => eu.UserId, (u, eu) => new { FullName = u.FirstName + " " + u.LastName, CompanyId = eu.CompanyId })
                        .Where(user => user.CompanyId == c.cm.Id).Select(user => user.FullName).FirstOrDefault() ?? string.Empty,
                UserRights = string.Join(", ",
                    elixirUsersForUser
                        .Where(eu => eu.CompanyId == c.cm.Id)
                        .Select(eu =>
                            eu.UserGroupId == (int)UserGroupRoles.AccountManager
                                ? AppConstants.USER_RIGHTS_ACCOUNT_MANAGER
                                : (eu.UserGroupId == (int)UserGroupRoles.Checker
                                    ? AppConstants.USER_RIGHTS_CHECKER
                                    : (eu.UserGroupId == (int)UserGroupRoles.MigrationUser
                                        ? AppConstants.USER_RIGHTS_MIGRATION_USER
                                        : (userGroupsDict.ContainsKey(eu.UserGroupId) ? userGroupsDict[eu.UserGroupId] : string.Empty)
                                    )
                                )
                        )
                        .Distinct()
                ) ?? string.Empty,
                ClientCompaniesCount = _dbContext.ClientCompaniesMappings
                    .Where(cl => cl.ClientId == c.cl.Id)
                    .Select(cl => cl.CompanyId)
                    .Distinct()
                    .Count(),
                ClientAccountManagersCount = _dbContext.ElixirUsers
                    .Where(cl => cl.ClientId == c.cl.Id)
                    .Select(cl => cl.UserId)
                    .Distinct()
                    .Count(),
                UserName = (
                    (from cos in _dbContext.CompanyOnboardingStatuses
                     join u in _dbContext.Users on cos.CreatedBy equals u.Id
                     where cos.CompanyId == c.cm.Id && !u.IsDeleted
                     select u.FirstName + " " + u.LastName
                    ).FirstOrDefault()
                    ?? _dbContext.SuperUsers
                        .Select(su => su.FirstName + " " + su.LastName)
                        .FirstOrDefault()
                    ?? string.Empty
                ),
                CreatedOn = (DateTime?)c.cm.CreatedAt ?? DateTime.UtcNow,
                LastUpdatedOn = (from ch in _dbContext.CompanyHistories
                                 where ch.CompanyId == c.cm.Id
                                 orderby ch.Version descending
                                 select ch.LastUpdatedOn).FirstOrDefault() ?? c.cm.LastUpdatedOn,
                LastUpdatedBy =
                            (_dbContext.CompanyOnboardingStatuses
                        .Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_NEW))
                        ? (
                            (
                                from cos in _dbContext.CompanyOnboardingStatuses
                                where cos.CompanyId == c.cm.Id && !cos.IsDeleted && cos.OnboardingStatus == AppConstants.ONBOARDING_STATUS_NEW
                                orderby cos.CreatedAt descending
                                select cos.CreatedBy
                            ).Select(createdById =>
                                _dbContext.Users
                                    .Where(u => u.Id == createdById && !u.IsDeleted)
                                    .Select(u => u.FirstName + " " + u.LastName)
                                    .FirstOrDefault()
                                ?? _dbContext.SuperUsers
                                    .Where(su => su.Id == createdById)
                                    .Select(su => su.FirstName + " " + su.LastName)
                                    .FirstOrDefault()
                            ).FirstOrDefault()
                        ) ?? string.Empty
                        : (_dbContext.CompanyOnboardingStatuses.Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_APPROVED))
                            ? (
                                from comp in _dbContext.Companies
                                join u in _dbContext.Users on comp.CreatedBy equals u.Id
                                where comp.Id == c.cm.Id && !u.IsDeleted
                                select u.FirstName + " " + u.LastName
                              ).FirstOrDefault() ?? string.Empty
                            : (
                                from ch in _dbContext.CompanyHistories
                                join u in _dbContext.Users on ch.CreatedBy equals u.Id
                                where ch.CompanyId == c.cm.Id && !u.IsDeleted
                                orderby ch.Version descending
                                select u.FirstName + " " + u.LastName
                              ).FirstOrDefault() ?? string.Empty,
                CreatedBy =
                     (from cos in _dbContext.CompanyOnboardingStatuses
                      join u in _dbContext.Users on cos.CreatedBy equals u.Id
                      where cos.CompanyId == c.cm.Id && !u.IsDeleted
                      select u.FirstName + " " + u.LastName
                    ).FirstOrDefault()
                    ?? _dbContext.SuperUsers
                        .Select(su => su.FirstName + " " + su.LastName)
                        .FirstOrDefault()
                    ?? string.Empty,
                CompanyStatus = _dbContext.CompanyOnboardingStatuses
                    .Where(cos => cos.CompanyId == c.cm.Id)
                    .Select(cos => (bool?)cos.IsActive)
                    .FirstOrDefault(),
            }).ToList();

            // Companies with AccountManagerUserId == userId (but no Accounts)
            var companiesWithoutAccountsQuery = _dbContext.Companies
                .Where(cm => !cm.IsDeleted && (cm.IsEnabled ?? false) && cm.AccountManagerId == userId)
                .Join(_dbContext.Clients, cm => cm.ClientId, cl => cl.Id, (cm, cl) => new { cm, cl })
                .ToList();

            var companiesWithoutAccountsList = companiesWithoutAccountsQuery.Select(c => new CompanyTMISummaryDto
            {
                CompanyID = c.cm.Id,
                CompanyName = c.cm.CompanyName ?? AppConstants.NOTAVAILABLE,
                ClientCode = string.IsNullOrEmpty(c.cm.ClientName) || c.cm.ClientName == AppConstants.NOTAVAILABLE ? AppConstants.NOTAVAILABLE : c.cl.ClientCode,
                CompanyStorageConsumedGB = c.cm.CompanyStorageConsumedGb ?? 0,
                CompanyStorageTotalGB = c.cm.CompanyStorageTotalGb ?? 0,
                DisplaydCompanyStorageConsumedGB = "0/" + c.cm.CompanyStorageConsumedGb,
                DisplaydPerUserStorageMB = "0/" + c.cm.CompanyStorageTotalGb,
                IsEnabled = c.cm.IsEnabled ?? false,
                Users = 0,
                UserGroups = 0,
                ClientId = c.cl.Id,
                ClientName = string.IsNullOrEmpty(c.cl.ClientName) || c.cl.ClientName == AppConstants.NOTAVAILABLE ? AppConstants.NOTAVAILABLE : c.cl.ClientName,
                IsUnderEdit = c.cm.IsUnderEdit ?? false,
                PerUserStorageMB = 0,
                ActiveSince = _dbContext.CompanyOnboardingStatuses.Where(cos => cos.CompanyId == c.cm.Id && !cos.IsDeleted).OrderBy(cos => cos.CreatedAt).Select(cos => (DateTime?)cos.CreatedAt).FirstOrDefault() ?? DateTime.UtcNow,
                CreatedOn = (DateTime?)c.cm.CreatedAt ?? DateTime.UtcNow,
                Module = (_dbContext.ModuleMappings.Where(mm => mm.CompanyId == c.cm.Id && !mm.IsDeleted).Select(mm => mm.ModuleId).Distinct().Count() - 1) < 0
                            ? string.Empty
                            : $"{AppConstants.MODULE_CORE_HR}+ {_dbContext.ModuleMappings.Where(mm => mm.CompanyId == c.cm.Id && !mm.IsDeleted).Select(mm => mm.ModuleId).Distinct().Count() - 1}",
                AdminName = (
                    (from cai in _dbContext.ClientAdminInfos
                     where cai.ClientId == c.cl.Id
                     select cai.FirstName + " " + cai.LastName
                    ).FirstOrDefault() ?? string.Empty
                ),
                UserRights = AppConstants.ACCOUNTMANAGER_GROUPNAME,
                ClientCompaniesCount = _dbContext.ClientCompaniesMappings
                    .Where(cl => cl.ClientId == c.cl.Id)
                    .Select(cl => cl.CompanyId)
                    .Distinct()
                    .Count(),
                ClientAccountManagersCount = _dbContext.ElixirUsers
                    .Where(eu => eu.ClientId == c.cl.Id)
                    .Select(eu => eu.UserId)
                    .Distinct()
                    .Count(),
                CompanyStatus = _dbContext.Clients
                    .Where(cl => cl.Id == c.cl.Id)
                    .Select(cl => cl.IsEnabled)
                    .FirstOrDefault(),
            }).ToList();

            var finalCompaniesQuery = companiesWithAccountsList
                .Union(companiesWithoutAccountsList)
                .Where(c =>
                    (!string.IsNullOrEmpty(c.CompanyName) && c.CompanyName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.ClientName) && c.ClientName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStorageConsumedGB.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStorageTotalGB.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.DisplaydCompanyStorageConsumedGB.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.DisplaydPerUserStorageMB.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.UserGroups.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Users.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.AdminName) && c.AdminName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CreatedOn.HasValue && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.LastUpdatedOn.HasValue && c.LastUpdatedOn.Value.ToString("dd/MM/yyyy").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStatus == true && "enabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStatus == false && "disabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Module != null && c.Module.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                )
                // Order by LastUpdatedOn (fallback to CreatedOn) descending so recently created/updated records appear first
                .OrderByDescending(c => c.LastUpdatedOn ?? c.CreatedOn ?? DateTime.MinValue)
                .ThenBy(c => c.ClientName)
                .ToList();

            totalCount = finalCompaniesQuery.Count;

            companies = finalCompaniesQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
        else
        {
            // Materialize companies with accounts (under edit)
            var companiesWithAccountsQuery = _dbContext.Companies
                .Where(cm =>
                    accessibleCompanyIds.Contains(cm.Id)
                    && !cm.IsDeleted
                    && (cm.IsUnderEdit ?? false) == IsUnderEdit
                    && (cm.IsEnabled ?? false)
                )
                .Join(_dbContext.Clients, cm => cm.ClientId, cl => cl.Id, (cm, cl) => new { cm, cl })
                .Join(_dbContext.Accounts, c => c.cm.Id, a => a.CompanyId, (c, a) => new { c.cm, c.cl, a })
                .ToList();

            var companiesWithAccountsList = companiesWithAccountsQuery.Select(c => new CompanyTMISummaryDto
            {
                IsActive = true,
                CompanyID = c.cm.Id,
                CompanyName = c.cm.CompanyName ?? AppConstants.NOTAVAILABLE,
                ClientCode = string.IsNullOrEmpty(c.cm.ClientName) || c.cm.ClientName == AppConstants.NOTAVAILABLE ? AppConstants.NOTAVAILABLE : c.cl.ClientCode,
                CompanyStorageConsumedGB = c.cm.CompanyStorageConsumedGb ?? 0,
                CompanyStorageTotalGB = c.cm.CompanyStorageTotalGb ?? 0,
                DisplaydCompanyStorageConsumedGB = "0/" + c.cm.CompanyStorageConsumedGb,
                DisplaydPerUserStorageMB = "0/" + c.cm.CompanyStorageTotalGb,
                IsEnabled = c.cm.IsEnabled ?? false,
                Users = 0,
                UserGroups = 0,
                ClientId = c.cl.Id,
                ClientName = string.IsNullOrEmpty(c.cm.ClientName) || c.cm.ClientName == AppConstants.NOTAVAILABLE ? AppConstants.NOTAVAILABLE : c.cl.ClientName,
                IsUnderEdit = c.cm.IsUnderEdit ?? false,
                PerUserStorageMB = c.a.PerUserStorageMb ?? 0,
                ActiveSince = _dbContext.CompanyOnboardingStatuses.Where(cos => cos.CompanyId == c.cm.Id && !cos.IsDeleted).OrderBy(cos => cos.CreatedAt).Select(cos => (DateTime?)cos.CreatedAt).FirstOrDefault() ?? DateTime.UtcNow,
                Module = (_dbContext.ModuleMappings.Where(mm => mm.CompanyId == c.cm.Id && !mm.IsDeleted).Select(mm => mm.ModuleId).Distinct().Count() - 1) < 0
                            ? string.Empty
                            : $"{AppConstants.MODULE_CORE_HR}+ {_dbContext.ModuleMappings.Where(mm => mm.CompanyId == c.cm.Id && !mm.IsDeleted).Select(mm => mm.ModuleId).Distinct().Count() - 1}",
                AdminName = _dbContext.CompanyAdminUsers.Where(cau => !cau.IsDeleted && cau.CompanyId == c.cm.Id).Select(cau => cau.FirstName + " " + cau.LastName).FirstOrDefault()
                    ?? _dbContext.Users.Where(u => !u.IsDeleted)
                        .Join(_dbContext.ElixirUsers.Where(eu => !eu.IsDeleted && eu.RoleId == (int)Roles.ClientAccoutManagers), u => u.Id, eu => eu.UserId, (u, eu) => new { FullName = u.FirstName + " " + u.LastName, CompanyId = eu.CompanyId })
                        .Where(user => user.CompanyId == c.cm.Id).Select(user => user.FullName).FirstOrDefault() ?? string.Empty,
                UserRights = string.Join(", ",
                    elixirUsersForUser
                        .Where(eu => eu.CompanyId == c.cm.Id)
                        .Select(eu =>
                            eu.UserGroupId == (int)UserGroupRoles.AccountManager
                                ? AppConstants.USER_RIGHTS_ACCOUNT_MANAGER
                                : (eu.UserGroupId == (int)UserGroupRoles.Checker
                                    ? AppConstants.USER_RIGHTS_CHECKER
                                    : (eu.UserGroupId == (int)UserGroupRoles.MigrationUser
                                        ? AppConstants.USER_RIGHTS_MIGRATION_USER
                                        : (userGroupsDict.ContainsKey(eu.UserGroupId) ? userGroupsDict[eu.UserGroupId] : string.Empty)
                                    )
                                )
                        )
                        .Distinct()
                ) ?? string.Empty,
                ClientCompaniesCount = _dbContext.ClientCompaniesMappings
                    .Where(cl => cl.ClientId == c.cl.Id)
                    .Select(cl => cl.CompanyId)
                    .Distinct()
                    .Count(),
                ClientAccountManagersCount = _dbContext.ElixirUsers
                    .Where(cl => cl.ClientId == c.cl.Id)
                    .Select(cl => cl.UserId)
                    .Distinct()
                    .Count(),
                UserName = (
                    (from cos in _dbContext.CompanyOnboardingStatuses
                     join u in _dbContext.Users on cos.CreatedBy equals u.Id
                     where cos.CompanyId == c.cm.Id && !u.IsDeleted
                     select u.FirstName + " " + u.LastName
                    ).FirstOrDefault()
                    ?? _dbContext.SuperUsers
                        .Select(su => su.FirstName + " " + su.LastName)
                        .FirstOrDefault()
                    ?? string.Empty
                ),
                CreatedOn = (DateTime?)c.cm.CreatedAt ?? DateTime.UtcNow,
                LastUpdatedOn = (from ch in _dbContext.CompanyHistories
                                 where ch.CompanyId == c.cm.Id
                                 orderby ch.Version descending
                                 select ch.LastUpdatedOn).FirstOrDefault() ?? c.cm.LastUpdatedOn,
                LastUpdatedBy =
                    (_dbContext.CompanyOnboardingStatuses
                        .Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_NEW))
                        ? (
                            (
                                from cos in _dbContext.CompanyOnboardingStatuses
                                where cos.CompanyId == c.cm.Id && !cos.IsDeleted && cos.OnboardingStatus == AppConstants.ONBOARDING_STATUS_NEW
                                orderby cos.CreatedAt descending
                                select cos.CreatedBy
                            ).Select(createdById =>
                                _dbContext.Users
                                    .Where(u => u.Id == createdById && !u.IsDeleted)
                                    .Select(u => u.FirstName + " " + u.LastName)
                                    .FirstOrDefault()
                                ?? _dbContext.SuperUsers
                                    .Where(su => su.Id == createdById)
                                    .Select(su => su.FirstName + " " + su.LastName)
                                    .FirstOrDefault()
                            ).FirstOrDefault()
                        ) ?? string.Empty
                        : (_dbContext.CompanyOnboardingStatuses.Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_APPROVED))
                            ? (
                                from comp in _dbContext.Companies
                                join u in _dbContext.Users on comp.CreatedBy equals u.Id
                                where comp.Id == c.cm.Id && !u.IsDeleted
                                select u.FirstName + " " + u.LastName
                              ).FirstOrDefault() ?? string.Empty
                            : (
                                from ch in _dbContext.CompanyHistories
                                join u in _dbContext.Users on ch.CreatedBy equals u.Id
                                where ch.CompanyId == c.cm.Id && !u.IsDeleted
                                orderby ch.Version descending
                                select u.FirstName + " " + u.LastName
                              ).FirstOrDefault() ?? string.Empty,
                CreatedBy =
                      (from cos in _dbContext.CompanyOnboardingStatuses
                       join u in _dbContext.Users on cos.CreatedBy equals u.Id
                       where cos.CompanyId == c.cm.Id && !u.IsDeleted
                       select u.FirstName + " " + u.LastName
                    ).FirstOrDefault()
                    ?? _dbContext.SuperUsers
                        .Select(su => su.FirstName + " " + su.LastName)
                        .FirstOrDefault()
                    ?? string.Empty,
                CompanyStatus = _dbContext.CompanyOnboardingStatuses
                    .Where(cos => cos.CompanyId == c.cm.Id)
                    .Select(cos => (bool?)cos.IsActive)
                    .FirstOrDefault(),
            }).ToList();

            var finalCompaniesQuery = companiesWithAccountsList
                .Where(c =>
                    (!string.IsNullOrEmpty(c.CompanyName) && c.CompanyName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.ClientName) && c.ClientName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.CreatedBy) && c.CreatedBy.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.LastUpdatedBy) && c.LastUpdatedBy.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.UserName) && c.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.AdminName) && c.AdminName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStorageConsumedGB.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStorageTotalGB.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.DisplaydCompanyStorageConsumedGB.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.DisplaydPerUserStorageMB.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.UserGroups.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Users.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CreatedOn.HasValue && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.LastUpdatedOn.HasValue && c.LastUpdatedOn.Value.ToString("dd/MM/yyyy").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStatus == true && "enabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStatus == false && "disabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Module != null && c.Module.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                )
                // Order by LastUpdatedOn (fallback to CreatedOn) descending so recently created/updated records appear first
                .OrderByDescending(c => c.LastUpdatedOn ?? c.CreatedOn ?? DateTime.MinValue)
                .ThenBy(c => c.ClientName)
                .ToList();

            totalCount = finalCompaniesQuery.Count;

            companies = finalCompaniesQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        return new Tuple<List<CompanyTMISummaryDto>, int>(companies, totalCount);
    }
    // Pseudocode / Plan (detailed):
    // 1. Materialize the LINQ companiesQuery to a list (companiesList) as before.
    // 2. Apply the existing in-memory search filter to produce filteredCompanies.
    // 3. Order filteredCompanies by (LastUpdatedOn ?? CreatedOn) descending so recently created/updated records appear first.
    //    - Use DateTime.MinValue as a final fallback to avoid null comparison issues.
    // 4. Compute totalCount from the ordered filtered list.
    // 5. Apply pagination (Skip/Take) on the ordered list.
    // 6. Return the paged list and totalCount as Tuple<List<CompanySummaryDto>, int>.
    // 7. Do not change any other existing filtering logic or projections.

    public async Task<Tuple<List<CompanySummaryDto>, int>> GetPagedDelegateAdminCompaniesSummaryAsync(int userId, bool IsUnderEdit, bool IsSuperUser, string searchTerm, int pageNumber, int pageSize)
    {

        // Old Master Id : 12 - Delegate Admin
        // Get the Group Id for the User if he is in Custom User Group
        var userGroupId = await _dbContext.UserGroups
            .Where(ug => !ug.IsDeleted && EF.Functions.Like(ug.GroupType, AppConstants.USER_GROUP_TYPE_CUSTOM) && _dbContext.UserGroupMappings.Any(ugm => ugm.UserId == userId && !ugm.IsDeleted && ugm.UserGroupId == ug.Id))
            .Select(ug => ug.Id).FirstOrDefaultAsync();

        //Get the UserGroupMenuMappings for the User Group Id for the SubMenu Item 'Company List' since this is for the company summary, this sub menu corresponds to the company list in the UI
        var IsAllCompanies = await _dbContext.UserGroupMenuMappings
            .Where(ugmm => _dbContext.SubMenuItems.Where(smi => EF.Functions.Like(smi.SubMenuItemName, AppConstants.COMPANY_LIST_SUB_MENU_ITEM) && ugmm.UserGroupId == userGroupId)
            .Select(smi => smi.Id).Contains(ugmm.SubMenuItemId)).Select(ugmm => ugmm.IsAllCompanies).FirstOrDefaultAsync() ?? false;

        // Check if 'Company List' screen has any permission (Create, ViewOnly, Approve, Edit) for the given user group
        bool hasCompanyListScreenPermission = await _dbContext.UserGroupMenuMappings
            .Where(ugmm =>
                ugmm.UserGroupId == userGroupId &&
                _dbContext.SubMenuItems
                    .Where(smi => EF.Functions.Like(smi.SubMenuItemName, AppConstants.COMPANY_LIST_SUB_MENU_ITEM))
                    .Select(smi => smi.Id)
                    .Contains(ugmm.SubMenuItemId) &&
                (
                    (ugmm.CreateAccess ?? false) ||
                    (ugmm.ViewOnlyAccess ?? false) ||
                    (ugmm.ApproveAccess ?? false) ||
                    (ugmm.EditAccess ?? false)
                )
            )
            .AnyAsync();

        IQueryable<CompanySummaryDto> companiesQuery = null;
        if (!hasCompanyListScreenPermission)
        {
            // If the user does not have permission to view the company list, return an empty list
            return new Tuple<List<CompanySummaryDto>, int>(new List<CompanySummaryDto>(), 0);
        }
        if (IsAllCompanies)
        {
            //As per the FRD bring all the companies even if the user does not belong to that companies, if incase the FRD changes use the filter like this ****eu.UserId == userId****.
            companiesQuery = _dbContext.Companies.Where(cm => _dbContext.ElixirUsers.Where(eu => !eu.IsDeleted).Select(eu => eu.CompanyId).Distinct().Contains(cm.Id) && !cm.IsDeleted && (cm.IsEnabled ?? false))
                .Join(_dbContext.Clients, cm => cm.ClientId, cl => cl.Id, (cm, cl) => new { cm, cl }).Join(_dbContext.Accounts, c => c.cm.Id, a => a.CompanyId, (c, a) => new CompanySummaryDto
                {
                    CompanyID = c.cm.Id,
                    CompanyName = c.cm.CompanyName,
                    ClientCode = (string.IsNullOrEmpty(c.cm.ClientName) || c.cm.ClientName == AppConstants.NOTAVAILABLE) ? AppConstants.NOTAVAILABLE : c.cl.ClientCode,
                    CompanyStorageConsumedGB = c.cm.CompanyStorageConsumedGb ?? 0,
                    DisplaydCompanyStorageConsumedGB = "0/" + c.cm.CompanyStorageConsumedGb,
                    CompanyStorageTotalGB = c.cm.CompanyStorageTotalGb ?? 0,
                    DisplaydPerUserStorageMB = "0/" + c.cm.CompanyStorageTotalGb,
                    IsEnabled = c.cm.IsEnabled ?? false,
                    Users = 0, //Place Holders for now, it will be part of FRD2
                    UserGroups = 0,  //Place Holders for now, it will be part of FRD2
                    ClientId = c.cl.Id,
                    ClientName = c.cm.ClientName ?? AppConstants.NOTAVAILABLE,
                    IsUnderEdit = c.cm.IsUnderEdit ?? false,
                    PerUserStorageMB = a.PerUserStorageMb ?? 0,
                    ClientCompaniesCount = _dbContext.ClientCompaniesMappings
                             .Where(cl => cl.ClientId == c.cl.Id)
                             .Select(cl => cl.CompanyId)
                             .Distinct()
                             .Count(),
                    ClientAccountManagersCount = _dbContext.ElixirUsers
                             .Where(cl => cl.ClientId == c.cl.Id)
                             .Select(cl => cl.UserId)
                             .Distinct()
                             .Count(),
                    CreatedOn = (DateTime?)c.cm.CreatedAt ?? DateTime.UtcNow,
                    LastUpdatedOn = (from ch in _dbContext.CompanyHistories
                                     where ch.CompanyId == c.cm.Id
                                     orderby ch.Version descending
                                     select ch.LastUpdatedOn).FirstOrDefault() ?? c.cm.LastUpdatedOn,
                    LastUpdatedBy =
                        // If OnBoardingStatus is "New", get CreatedBy from SuperUsers
                        (_dbContext.CompanyOnboardingStatuses.Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_NEW))
                            ? (
                                (from cos in _dbContext.CompanyOnboardingStatuses
                                 join u in _dbContext.Users on cos.CreatedBy equals u.Id
                                 where cos.CompanyId == c.cm.Id && !u.IsDeleted
                                 select u.FirstName + " " + u.LastName
                                ).FirstOrDefault() ?? string.Empty
                            )
                        // If OnBoardingStatus is "Approved", get CreatedBy from Companies table
                        : (_dbContext.CompanyOnboardingStatuses.Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_APPROVED))
                            ? (
                                (from comp in _dbContext.Companies
                                 join u in _dbContext.Users on comp.CreatedBy equals u.Id
                                 where comp.Id == c.cm.Id && !u.IsDeleted
                                 select u.FirstName + " " + u.LastName
                                ).FirstOrDefault() ?? string.Empty
                            )
                        // Else, get CreatedBy from latest CompanyHistory for the company
                        : (
                            (from ch in _dbContext.CompanyHistories
                             join u in _dbContext.Users on ch.CreatedBy equals u.Id
                             where ch.CompanyId == c.cm.Id && !u.IsDeleted
                             orderby ch.Version descending
                             select u.FirstName + " " + u.LastName
                            ).FirstOrDefault() ?? string.Empty
                        ),
                    CreatedBy =
                        (from cos in _dbContext.CompanyOnboardingStatuses
                         where cos.CompanyId == c.cm.Id
                         orderby cos.CreatedAt descending
                         select cos.CreatedBy).FirstOrDefault() == (int)Roles.SuperAdmin
                            ? (_dbContext.SuperUsers.Select(su => su.FirstName + " " + su.LastName).FirstOrDefault() ?? string.Empty)
                            : (
                                from cos in _dbContext.CompanyOnboardingStatuses
                                join u in _dbContext.Users on cos.CreatedBy equals u.Id
                                where cos.CompanyId == c.cm.Id && !u.IsDeleted
                                select u.FirstName + " " + u.LastName
                            ).FirstOrDefault() ?? string.Empty,
                    CompanyStatus = _dbContext.CompanyOnboardingStatuses
                           .Where(cos => cos.CompanyId == c.cm.Id)
                           .Select(cos => (bool?)cos.IsActive)
                           .FirstOrDefault(),
                });
        }
        else
        {
            //Apply UserId and GroupId Filter since user can be part of only one custom group
            companiesQuery = _dbContext.Companies.Where(cm => _dbContext.ElixirUsers.Where(eu => eu.UserId == userId && eu.UserGroupId == userGroupId && !eu.IsDeleted).Select(eu => eu.CompanyId).Distinct().Contains(cm.Id) && !cm.IsDeleted && (cm.IsEnabled ?? false))
                .Join(_dbContext.Clients, cm => cm.ClientId, cl => cl.Id, (cm, cl) => new { cm, cl }).Join(_dbContext.Accounts, c => c.cm.Id, a => a.CompanyId, (c, a) => new CompanySummaryDto
                {
                    CompanyID = c.cm.Id,
                    CompanyName = c.cm.CompanyName,
                    ClientCode = (string.IsNullOrEmpty(c.cm.ClientName) || c.cm.ClientName == AppConstants.NOTAVAILABLE) ? AppConstants.NOTAVAILABLE : c.cl.ClientCode,
                    CompanyStorageConsumedGB = c.cm.CompanyStorageConsumedGb ?? 0,
                    DisplaydCompanyStorageConsumedGB = "0/" + c.cm.CompanyStorageConsumedGb,
                    CompanyStorageTotalGB = c.cm.CompanyStorageTotalGb ?? 0,
                    DisplaydPerUserStorageMB = "0/" + c.cm.CompanyStorageTotalGb,
                    IsEnabled = c.cm.IsEnabled ?? false,
                    Users = 0, //Place Holders for now, it will be part of FRD2
                    UserGroups = 0,  //Place Holders for now, it will be part of FRD2
                    ClientId = c.cl.Id,
                    ClientName = c.cm.ClientName ?? AppConstants.NOTAVAILABLE,
                    IsUnderEdit = c.cm.IsUnderEdit ?? false,
                    PerUserStorageMB = a.PerUserStorageMb ?? 0,
                    ClientCompaniesCount = _dbContext.ClientCompaniesMappings
                             .Where(cl => cl.ClientId == c.cl.Id)
                             .Select(cl => cl.CompanyId)
                             .Distinct()
                             .Count(),
                    ClientAccountManagersCount = _dbContext.ElixirUsers
                             .Where(cl => cl.ClientId == c.cl.Id)
                             .Select(cl => cl.UserId)
                             .Distinct()
                             .Count(),
                    CreatedOn = (DateTime?)c.cm.CreatedAt ?? DateTime.UtcNow,
                    LastUpdatedOn = (from ch in _dbContext.CompanyHistories
                                     where ch.CompanyId == c.cm.Id
                                     orderby ch.Version descending
                                     select ch.LastUpdatedOn).FirstOrDefault() ?? c.cm.LastUpdatedOn,
                    LastUpdatedBy =
                        // If OnBoardingStatus is "New", get CreatedBy from SuperUsers
                        (_dbContext.CompanyOnboardingStatuses.Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_NEW))
                            ? (
                                (from cos in _dbContext.CompanyOnboardingStatuses
                                 join u in _dbContext.Users on cos.CreatedBy equals u.Id
                                 where cos.CompanyId == c.cm.Id && !u.IsDeleted
                                 select u.FirstName + " " + u.LastName
                                ).FirstOrDefault() ?? string.Empty
                            )
                        // If OnBoardingStatus is "Approved", get CreatedBy from Companies table
                        : (_dbContext.CompanyOnboardingStatuses.Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_APPROVED))
                            ? (
                                (from comp in _dbContext.Companies
                                 join u in _dbContext.Users on comp.CreatedBy equals u.Id
                                 where comp.Id == c.cm.Id && !u.IsDeleted
                                 select u.FirstName + " " + u.LastName
                                ).FirstOrDefault() ?? string.Empty
                            )
                        // Else, get CreatedBy from latest CompanyHistory for the company
                        : (
                            (from ch in _dbContext.CompanyHistories
                             join u in _dbContext.Users on ch.CreatedBy equals u.Id
                             where ch.CompanyId == c.cm.Id && !u.IsDeleted
                             orderby ch.Version descending
                             select u.FirstName + " " + u.LastName
                            ).FirstOrDefault() ?? string.Empty
                        ),
                    CreatedBy =
                        (from cos in _dbContext.CompanyOnboardingStatuses
                         where cos.CompanyId == c.cm.Id
                         orderby cos.CreatedAt descending
                         select cos.CreatedBy).FirstOrDefault() == (int)Roles.SuperAdmin
                            ? (_dbContext.SuperUsers.Select(su => su.FirstName + " " + su.LastName).FirstOrDefault() ?? string.Empty)
                            : (
                                from cos in _dbContext.CompanyOnboardingStatuses
                                join u in _dbContext.Users on cos.CreatedBy equals u.Id
                                where cos.CompanyId == c.cm.Id && !u.IsDeleted
                                select u.FirstName + " " + u.LastName
                            ).FirstOrDefault() ?? string.Empty,
                    CompanyStatus = _dbContext.CompanyOnboardingStatuses
                           .Where(cos => cos.CompanyId == c.cm.Id)
                           .Select(cos => (bool?)cos.IsActive)
                           .FirstOrDefault(),
                });
        }
        // Get total count
        //var totalCount = await companiesQuery.CountAsync();

        // Materialize the query first
        var companiesList = await companiesQuery.ToListAsync();

        // Apply the search filter in-memory
        var filteredCompanies = companiesList
            .Where(c =>
                    (!string.IsNullOrEmpty(c.CompanyName) && c.CompanyName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.ClientName) && c.ClientName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStorageConsumedGB.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStorageTotalGB.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.UserGroups.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Users.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.DisplaydCompanyStorageConsumedGB) && c.DisplaydCompanyStorageConsumedGB.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStatus == true && "enabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.CompanyStatus == false && "disabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            )
            .ToList();

        // Order by LastUpdatedOn (fallback to CreatedOn) descending so recently created/updated records appear first
        var orderedCompanies = filteredCompanies
            .OrderByDescending(c => c.LastUpdatedOn ?? c.CreatedOn ?? DateTime.MinValue)
            .ToList();

        var totalCount = orderedCompanies.Count;

        var companies = orderedCompanies
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new Tuple<List<CompanySummaryDto>, int>(companies, totalCount);
    }

    public async Task<CompanyBasicInfoDto> GetCompanyBasicInfoAsync(int companyId)
    {
        return await _dbContext.Companies
            .Where(c => c.Id == companyId && !c.IsDeleted)
            .Select(c => new CompanyBasicInfoDto
            {
                CompanyId = c.Id,
                CompanyName = c.CompanyName,
                CompanyCode = c.CompanyCode,
            })
            .FirstOrDefaultAsync();
    }



    // Pseudocode / Plan (detailed):
    // 1. Ensure searchTerm is non-null to avoid null reference issues when calling Contains.
    // 2. Materialize the base query that selects distinct users for the company to an in-memory list (to avoid EF translation issues).
    // 3. Apply the existing in-memory search filtering (preserve all filtering logic).
    // 4. Order the filtered results by their last activity timestamp so that recently created or updated users appear on top.
    //    - We will use CreatedOn as the primary timestamp (we preserve existing CreatedAt usage).
    //    - Use DateTime.MinValue as a fallback for null CreatedOn to avoid null comparison issues.
    //    - Add a stable secondary ordering (UserName) to keep UI ordering deterministic.
    // 5. Compute totalCount from the ordered list.
    // 6. Apply pagination (Skip/Take) on the ordered list and return paged results and total count.
    // 7. Do not modify any of the existing DTO shapes or filtering semantics beyond adding the ordering and null-safety for searchTerm.
    public async Task<Tuple<List<CompanyUserDto>, int>> GetFilteredCompanyUsersAsync(int CompanyId, string searchTerm, int pageNumber, int pageSize)
    {
        // Normalize search term to avoid null reference exceptions
        searchTerm = searchTerm ?? string.Empty;

        // Get distinct user ids for the company and join to Users to fetch user details
        var query = _dbContext.ElixirUsers
            .Where(eu => eu.CompanyId == CompanyId && !eu.IsDeleted)
            .Select(eu => new { eu.UserId })
            .Distinct()
            .Join(
                _dbContext.Users.Where(u => !u.IsDeleted),
                eu => eu.UserId,
                u => u.Id,
                (eu, u) => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    IsEnabled = u.IsEnabled ?? false,
                    u.CreatedAt
                });

        // Materialize to memory to perform in-memory filtering and ordering
        var userList = await query.ToListAsync();

        // Apply existing in-memory search filter
        var filteredUsers = userList
            .Where(ur =>
                (!string.IsNullOrEmpty(ur.FirstName) && ur.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(ur.LastName) && ur.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(ur.Email) && ur.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                ($"{ur.FirstName} {ur.LastName}".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (ur.IsEnabled && "enabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (!ur.IsEnabled && "disabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            )
            .Select(ur => new CompanyUserDto
            {
                UserId = ur.Id,
                UserName = $"{ur.FirstName} {ur.LastName}",
                Email = ur.Email,
                Status = ur.IsEnabled,
                CreatedOn = ur.CreatedAt,
            })
            .ToList();

        // Order by CreatedOn descending so newly created/updated records appear first.
        // Use DateTime.MinValue as a fallback for null CreatedOn. ThenBy UserName for stable ordering.
        var ordered = filteredUsers
            .OrderByDescending(u => u.CreatedOn ?? DateTime.MinValue)
            .ThenBy(u => u.UserName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var totalCount = ordered.Count;

        var paged = ordered
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new Tuple<List<CompanyUserDto>, int>(paged, totalCount);
    }


    // Pseudocode / Plan (detailed):
    // 1. Materialize the existing query into a list of CompanyBasicInfoDto (same as current behavior).
    // 2. Collect the company IDs from that list.
    // 3. For those company IDs, fetch the most recent LastUpdatedOn per company from CompanyHistories (by highest Version).
    //    - If there's no history, fall back to Companies.LastUpdatedOn.
    //    - Build a dictionary companyId -> LastUpdatedOn (nullable DateTime).
    // 4. Apply the existing in-memory search filtering to the materialized companies list (preserve all current filters).
    // 5. Order the filtered list by (LastUpdatedOn ?? CreatedOn) descending so newly created/updated records appear first.
    //    - Use DateTime.MinValue as final fallback to avoid null issues.
    // 6. Compute totalCount from the ordered list.
    // 7. Apply pagination (Skip/Take) and return the paged CompanyBasicInfoDto list and totalCount.
    // 8. Do not change any other business logic or DTO shapes, only change ordering behavior.

    public async Task<Tuple<List<CompanyBasicInfoDto>, int>> GetFilteredCompanyByUsersAsync(int userId, int groupId, string groupName, string searchTerm, int pageNumber, int pageSize)
    {
        var query = _dbContext.ElixirUsers
            .Where(eu => eu.UserId == userId && !eu.IsDeleted &&
                (
                    groupId == (int)UserGroupRoles.AccountManager ? eu.UserGroupId == (int)UserGroupRoles.AccountManager :
                    groupId == (int)UserGroupRoles.Checker ? eu.UserGroupId == (int)UserGroupRoles.Checker :
                    eu.UserGroupId == groupId
                )
            )
            .Join(_dbContext.Companies.Where(c => !c.IsDeleted), u => u.CompanyId, c => c.Id,
                (u, c) => new CompanyBasicInfoDto
                {
                    CompanyId = c.Id,
                    CompanyName = c.CompanyName,
                    CompanyCode = c.CompanyCode,
                    CreatedOn = c.CreatedAt,
                }).Distinct();

        var companies = await query.ToListAsync();

        // If there are no companies, return early
        if (companies == null || companies.Count == 0)
        {
            return new Tuple<List<CompanyBasicInfoDto>, int>(new List<CompanyBasicInfoDto>(), 0);
        }

        // Collect company IDs
        var companyIds = companies.Select(c => c.CompanyId).Distinct().ToList();

        // Fetch latest LastUpdatedOn from CompanyHistories per company (by highest Version)
        // Fall back to Companies.LastUpdatedOn when history not present
        var historyLastUpdated = await _dbContext.CompanyHistories
            .Where(ch => ch.CompanyId.HasValue && companyIds.Contains(ch.CompanyId.Value))
            .GroupBy(ch => ch.CompanyId.Value)
            .Select(g => new
            {
                CompanyId = g.Key,
                LastUpdated = g.OrderByDescending(x => x.Version).Select(x => x.LastUpdatedOn).FirstOrDefault()
            })
            .ToDictionaryAsync(x => x.CompanyId, x => x.LastUpdated);

        // Fetch Companies.LastUpdatedOn for fallback (only for companyIds)
        var companiesLastUpdated = await _dbContext.Companies
            .Where(c => companyIds.Contains(c.Id))
            .Select(c => new { c.Id, c.LastUpdatedOn })
            .ToDictionaryAsync(x => x.Id, x => x.LastUpdatedOn);

        // Build final map companyId -> LastUpdatedOn? (history takes precedence)
        var lastUpdatedMap = new Dictionary<int, DateTime?>(companyIds.Count);
        foreach (var id in companyIds)
        {
            DateTime? lastUpdated = null;
            if (historyLastUpdated.TryGetValue(id, out var histVal))
            {
                lastUpdated = histVal;
            }
            else if (companiesLastUpdated.TryGetValue(id, out var compVal))
            {
                lastUpdated = compVal;
            }
            lastUpdatedMap[id] = lastUpdated;
        }

        // Apply existing in-memory search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            companies = companies
                .Where(c =>
                    (!string.IsNullOrEmpty(c.CompanyName) && c.CompanyName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                )
                .ToList();
        }

        // Order by LastUpdatedOn (fallback to CreatedOn) descending so recently created/updated records appear first
        var orderedCompanies = companies
            .OrderByDescending(c =>
            {
                var lu = lastUpdatedMap.ContainsKey(c.CompanyId) ? lastUpdatedMap[c.CompanyId] : null as DateTime?;
                return lu ?? c.CreatedOn ?? DateTime.MinValue;
            })
            .ToList();

        var totalCount = orderedCompanies.Count;

        var paged = orderedCompanies
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new Tuple<List<CompanyBasicInfoDto>, int>(paged, totalCount);
    }

    public async Task<bool> Company5TabApproveCompanyDataAsync(int userId, Company5TabCompanyDto company5TabCompanyData, int companyStorageGB, int perUserStorageMB, CancellationToken cancellationToken = default)
    {
        // Get the latest version of the CompanyHistory record
        var company = await _dbContext.Companies.Where(ch => ch.Id == company5TabCompanyData.CompanyId && !ch.IsDeleted).FirstOrDefaultAsync(cancellationToken);

        if (company == null)
        {
            return false; // No record found to update
        }

        // Get CreatedAt from Companies table for consistency
        var createdAt = await _dbContext.Companies
            .Where(cmp => cmp.Id == company5TabCompanyData.CompanyId && !cmp.IsDeleted)
            .Select(c => c.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        bool isBillingDataSame = company5TabCompanyData.SameAddressForBilling ?? false;

        // Update the existing CompanyHistory record
        company.CompanyName = company5TabCompanyData.CompanyName;
        company.CompanyCode = company5TabCompanyData.CompanyCode;
        company.CompanyStorageConsumedGb = companyStorageGB;
        company.CompanyStorageTotalGb = perUserStorageMB;
        company.Address1 = company5TabCompanyData.Address1;
        company.Address2 = company5TabCompanyData.Address2;
        company.StateId = company5TabCompanyData.StateId;
        company.CountryId = company5TabCompanyData.CountryId;
        company.ZipCode = company5TabCompanyData.ZipCode;
        company.TelephoneCodeId = company5TabCompanyData.TelephoneCodeId;
        company.PhoneNumber = company5TabCompanyData.PhoneNumber;
        company.MfaEnabled = company5TabCompanyData.MultiFactor;
        company.MfaSms = company5TabCompanyData.IsSms;
        company.MfaEmail = company5TabCompanyData.IsEmail;
        company.BillingAddressSameAsCompany = company5TabCompanyData.SameAddressForBilling;
        company.LastUpdatedOn = DateTime.UtcNow;
        company.CreatedBy = userId;
        company.CreatedAt = createdAt;
        //company.IsEnabled = company5TabCompanyData.IsActive;
        company.LastUpdatedBy = userId;
        company.UpdatedAt = DateTime.UtcNow;
        company.BillingAddress1 = isBillingDataSame ? company5TabCompanyData.Address1 : company5TabCompanyData.BillingAddress1;
        company.BillingAddress2 = isBillingDataSame ? company5TabCompanyData.Address2 : company5TabCompanyData.BillingAddress2;
        company.BillingStateId = isBillingDataSame ? company5TabCompanyData.StateId : company5TabCompanyData.BillingStateId;
        company.BillingZipCode = isBillingDataSame ? company5TabCompanyData.ZipCode : company5TabCompanyData.BillingZipCode;
        company.BillingCountryId = isBillingDataSame ? company5TabCompanyData.CountryId : company5TabCompanyData.BillingCountryId;
        company.BillingTelephoneCodeId = isBillingDataSame ? company5TabCompanyData.BillingTelePhoneCodeId : company5TabCompanyData.BillingTelePhoneCodeId;
        company.BillingPhoneNumber = isBillingDataSame ? company5TabCompanyData.PhoneNumber : company5TabCompanyData.BillingPhoneNo;

        // Note: Version is not incremented since we're updating the existing record
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    //public Task<Tuple<List<CompanySummaryDto>, int>> GetPagedCompaniesSummaryByUser(int UserId, string searchTerm, int pageNumber, int pageSize)
    //{
    //    throw new NotImplementedException();
    //}
    public async Task<bool> AddClientAccountManagersAsync(CreateClientDto createClientDto, int userId, int clientId)
    {
        var now = DateTime.UtcNow;

        var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        var clientIds = await _dbContext.Clients
            .Where(c => c.ClientName == client.ClientName)
            .Select(c => c.Id)
            .ToListAsync();

        // Delete companies where ClientId is in the given clientIds list AND CompanyName is null or empty
        var companiesToDelete = await _dbContext.Companies
            .Where(c => clientIds.Contains(c.ClientId ?? 0) && (string.IsNullOrEmpty(c.CompanyName)))
            .ToListAsync();

        if (companiesToDelete.Any())
        {
            _dbContext.Companies.RemoveRange(companiesToDelete);
            await _dbContext.SaveChangesAsync();
        }

        //if (createClientDto.clientCompanyMappingDtos.Count == 0) return true;
        // 2. Add new companies for each ClientAccountManager
        var companiesToAdd = new List<Company>();
        foreach (var ec in createClientDto.ClientAccountManagers)
        {
            string companyCode;
            do
            {
                companyCode = $"DUMMYACCMGR-{Guid.NewGuid():N}".Substring(0, 15).ToUpper();
            } while (await _dbContext.Companies.AnyAsync(c => c.CompanyCode == companyCode && !c.IsDeleted));

            companiesToAdd.Add(new Company
            {
                AccountManagerId = ec.ClientAccountManagerId,
                IsEnabled = true,
                ClientId = clientId,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = userId,
                UpdatedBy = userId,
                LastUpdatedOn = now,
                CompanyCode = companyCode
            });
        }


        if (companiesToAdd.Count == 0)
            return true;

        await _dbContext.Companies.AddRangeAsync(companiesToAdd);
        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<Company> GetCompanyByIdAsync(int companyId)
    {
        return await _dbContext.Companies.FirstOrDefaultAsync(c => c.Id == companyId);
    }

    public async Task UpdateCompanyAsync(Company company)
    {
        _dbContext.Companies.Update(company);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<int> CreateCompanyAsync(int userId, CreateCompanyDto companyDto, CancellationToken cancellationToken = default)
    {
        // For a company, the same user cannot be both account manager and checker at the same time.
        // Check all users in the list, not just the first.
        if (companyDto.elixirUsers != null)
        {
            // Validation for CreateCompanyDto: A user cannot be both Account Manager and Checker
            if (companyDto.elixirUsers != null)
            {
                var groupedByUser = companyDto.elixirUsers
                    .Where(eu => eu.UserId.HasValue)
                    .GroupBy(eu => eu.UserId.Value)
                    .ToList();

                foreach (var group in groupedByUser)
                {
                    var userGroupIds = group.Select(eu => eu.UserGroupId).ToHashSet();
                    if (userGroupIds.Contains((int)UserGroupRoles.AccountManager) && userGroupIds.Contains((int)UserGroupRoles.Checker))
                    {
                        throw new InvalidOperationException("A user cannot be both an Account Manager and Checker");
                    }
                }
            }
            //var accountManagerUserIds = companyDto.elixirUsers
            //    .Where(eu => eu.UserGroupId == (int)UserGroupRoles.AccountManager && eu.UserId.HasValue)
            //    .Select(eu => eu.UserId.Value)
            //    .ToHashSet();

            //var checkerUserIds = companyDto.elixirUsers
            //    .Where(eu => eu.UserGroupId == (int)UserGroupRoles.Checker && eu.UserId.HasValue)
            //    .Select(eu => eu.UserId.Value)
            //    .ToHashSet();

            //var duplicateUserId = accountManagerUserIds.Intersect(checkerUserIds).FirstOrDefault();

            //if (duplicateUserId != 0)
            //{
            //    throw new Exception("A user cannot be both an Account Manager and a Checker at the same time.");
            //}
        }

        // 1. Create and save the Client first to get its Id
        var client = new Client
        {
            ClientName = AppConstants.NOTAVAILABLE,
            ClientInfo = AppConstants.DUMMYCLIENTENTRY,
            CreatedAt = DateTime.UtcNow,
            IsEnabled = false,
            ClientCode = GenerateClientCode(),
        };
        await _dbContext.Clients.AddAsync(client, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken); // Save to get client.Id

        // 2. Create the Company with the correct ClientId
        var company = new Company
        {
            CompanyName = companyDto.CompanyName,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            ClientId = client.Id, // Use the saved client Id
            IsUnderEdit = false,
            IsEnabled = false,
            CompanyCode = $"DUMMYCMP-{Guid.NewGuid():N}".Substring(0, 10).ToUpper()
        };
        await _dbContext.Companies.AddAsync(company, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken); // Save to get company.Id

        // 3. Save ElixirUsers (AccountManager, Checker, Custom Users)
        var elixirUsers = companyDto.elixirUsers?
            .Where(eu => eu.UserId.HasValue)
            .Select(eu => new ElixirUser
            {
                CompanyId = company.Id,
                UserId = eu.UserId.GetValueOrDefault(), // Safe: already filtered for HasValue
                UserGroupId = eu.UserGroupId,
                // Set RoleId based on UserGroupId
                RoleId = eu.UserGroupId == (int)UserGroupRoles.AccountManager
                    ? (int)Roles.AccountManager
                    : (eu.UserGroupId == (int)UserGroupRoles.Checker
                        ? (int)Roles.Checker
                        : 0),

            })
            .ToList();

        if (elixirUsers?.Count > 0)
        {
            await _dbContext.ElixirUsers.AddRangeAsync(elixirUsers, cancellationToken);
        }

        // 4. Prepare and add onboarding status/history
        var onboardingStatus = new CompanyOnboardingStatus
        {
            ClientId = client.Id,
            CompanyId = company.Id,
            OnboardingStatus = AppConstants.ONBOARDING_STATUS_NEW,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true, // Set to true for new onboarding
        };

        var onboardingHistory = new Company5TabOnboardingHistory
        {
            CompanyId = company.Id,
            Status = AppConstants.ONBOARDING_STATUS_NEW,
            UserId = userId,
            UpdatedAt = DateTime.UtcNow,
            IsEnabled = false,
        };

        await _dbContext.CompanyOnboardingStatuses.AddAsync(onboardingStatus, cancellationToken);
        await _dbContext.Company5TabOnboardingHistories.AddAsync(onboardingHistory, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return company.Id; // Return
    }

    // Helper method to generate client code
    private string GenerateClientCode() => $"CL-{Guid.NewGuid():N}".Substring(0, 10).ToUpper();
    public async Task<bool> UpdateCompanyAsync(int companyId, int userId, CreateCompanyDto companyDto, CancellationToken cancellationToken = default)
    {
        // For a company, the same user cannot be both account manager and checker at the same time.
        // Check all users in the list, not just the first.
        if (companyDto.elixirUsers != null)
        {
            // Validation for CreateCompanyDto: A user cannot be both Account Manager and Checker
            if (companyDto.elixirUsers != null)
            {
                var groupedByUser = companyDto.elixirUsers
                    .Where(eu => eu.UserId.HasValue)
                    .GroupBy(eu => eu.UserId.Value)
                    .ToList();

                foreach (var group in groupedByUser)
                {
                    var userGroupIds = group.Select(eu => eu.UserGroupId).ToHashSet();
                    if (userGroupIds.Contains((int)UserGroupRoles.AccountManager) && userGroupIds.Contains((int)UserGroupRoles.Checker))
                    {
                        throw new InvalidOperationException("A user cannot be both an Account Manager and Checker");
                    }
                }
            }
            //var accountManagerUserIds = companyDto.elixirUsers
            //    .Where(eu => eu.UserGroupId == (int)UserGroupRoles.AccountManager && eu.UserId.HasValue)
            //    .Select(eu => eu.UserId.Value)
            //    .ToHashSet();

            //var checkerUserIds = companyDto.elixirUsers
            //    .Where(eu => eu.UserGroupId == (int)UserGroupRoles.Checker && eu.UserId.HasValue)
            //    .Select(eu => eu.UserId.Value)
            //    .ToHashSet();

            //var duplicateUserId = accountManagerUserIds.Intersect(checkerUserIds).FirstOrDefault();

            //if (duplicateUserId != 0)
            //{
            //    throw new InvalidOperationException("A user cannot be both an Account Manager and a Checker at the same time.");
            //}
        }
        // Find the existing company
        var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Id == companyId && !c.IsDeleted, cancellationToken);
        if (company == null)
            return false;

        // Update company properties
        company.CompanyName = companyDto.CompanyName;
        company.UpdatedBy = userId;
        company.UpdatedAt = DateTime.UtcNow;
        //company.IsUnderEdit = false;
        //company.IsEnabled = companyDto.Status;


        // After updating company properties and before saving changes, update the IsActive field in CompanyOnboardingStatus
        var onboardingStatus = await _dbContext.CompanyOnboardingStatuses
            .FirstOrDefaultAsync(cos => cos.CompanyId == companyId, cancellationToken);

        if (onboardingStatus != null && companyDto.CompanyStatus.HasValue)
        {
            onboardingStatus.IsActive = companyDto.CompanyStatus.Value;
            _dbContext.CompanyOnboardingStatuses.Update(onboardingStatus);
        }

        // Update ElixirUsers (remove old, add new)
        var existingElixirUsers = await _dbContext.ElixirUsers
            .Where(eu => eu.CompanyId == companyId && !eu.IsDeleted)
            .ToListAsync(cancellationToken);

        if (existingElixirUsers.Count > 0)
        {
            _dbContext.ElixirUsers.RemoveRange(existingElixirUsers);
        }

        var newElixirUsers = companyDto.elixirUsers?
            .Where(eu => eu.UserId.HasValue)
            .Select(eu => new ElixirUser
            {
                CompanyId = company.Id,
                UserId = eu.UserId.Value,
                UserGroupId = eu.UserGroupId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RoleId = eu.UserGroupId == (int)UserGroupRoles.AccountManager
                    ? (int)Roles.AccountManager
                    : (eu.UserGroupId == (int)UserGroupRoles.Checker
                        ? (int)Roles.Checker
                        : 0),
            })
            .ToList();

        if (newElixirUsers?.Count > 0)
        {
            await _dbContext.ElixirUsers.AddRangeAsync(newElixirUsers, cancellationToken);
        }

        // Optionally update related entities (Client, OnboardingStatus, OnboardingHistory) if needed
        // This example does not update those, as requirements are not specified

        _dbContext.Companies.Update(company);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    // Pseudocode / Plan:
    // 1. Query ElixirUsers joined with Users for the given companyId and AccountManager group.
    // 2. Build the full name using FirstName and LastName with null-coalescing to avoid nulls.
    // 3. Apply Distinct() at the query level to ensure unique names (avoid duplicate names).
    // 4. Materialize with ToListAsync() and keep the rest of the method unchanged.

    public async Task<IEnumerable<object>> GetCompanyPopupDetailsByCompanyIdAsync(int companyId)
    {
        var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Id == companyId);
        if (company == null) return Enumerable.Empty<object>();

        var companyCodeFromHistory = await _dbContext.CompanyHistories
            .Where(ch => ch.CompanyId == companyId)
            .Select(ch => ch.CompanyCode)
            .FirstOrDefaultAsync();

        var companyLoginCode = !company.IsEnabled.GetValueOrDefault() ? companyCodeFromHistory : company.CompanyCode;

        var clientName = await _dbContext.Clients
            .Where(cl => cl.Id == company.ClientId)
            .Select(cl => cl.ClientName)
            .FirstOrDefaultAsync() ?? AppConstants.NOTAVAILABLE;

        var accountManagers = await (
            from eu in _dbContext.ElixirUsers
            join u in _dbContext.Users on eu.UserId equals u.Id
            where eu.CompanyId == companyId
                  && eu.UserGroupId == (int)UserGroupRoles.AccountManager
                  && !u.IsDeleted
            select (u.FirstName ?? "") + " " + (u.LastName ?? "")
        ).Distinct().ToListAsync();

        var groupedUsers = accountManagers.Any()
            ? new List<object> { new { groupName = AppConstants.ACCOUNTMANAGER_GROUPNAME, userNames = accountManagers } }
            : new List<object>();

        var noOfUsers = await _dbContext.ElixirUsers
                        .Where(eu => eu.CompanyId == companyId && !eu.IsDeleted)
                        .Select(eu => eu.UserId)
                        .Distinct()
                        .CountAsync();

        object? status = _dbContext.CompanyOnboardingStatuses
            .Where(cos => cos.CompanyId == company.Id)
            .Select(cos => cos.IsActive ? (bool?)cos.IsActive : null)
            .FirstOrDefault();

        return new[]
        {
            new
            {
                CompanyName = company.CompanyName,
                ClientName = clientName,
                CompanyCode = companyLoginCode,
                Status = status,
                CreatedOn = company.CreatedAt,
                noOfUsers,
                groupedUsers
            }
        };
    }
    public async Task<SuperAdminDashBoardDetailsDto> GetSuperAdminDashBoardDetailsAsync()
    {
        try
        {
            var activeCompaniesCount = await _dbContext.Companies
                .Where(c => (c.IsEnabled ?? false) && !String.IsNullOrEmpty(c.CompanyName) && !c.IsDeleted)
                .Skip(1)
                .CountAsync();

            var onboardingCompaniesCount = await _dbContext.CompanyOnboardingStatuses
                .Join(_dbContext.Companies,
                    onboarding => onboarding.CompanyId,
                    company => company.Id,
                    (onboarding, company) => new { onboarding, company })
                .CountAsync(x => (x.company.IsEnabled ?? false) == false && !x.company.IsDeleted);


            var clientsCount = await _dbContext.Clients
                            .Where(c => c.ClientName != null
                                        && c.ClientName != AppConstants.NOTAVAILABLE
                                        && !c.IsDeleted)
                            .Select(c => c.ClientName)
                            .Distinct()
                            .Skip(1)
                            .CountAsync();

            // Replace the onboardingClientsCount query with the following:
            var onboardingClientsCount = await _dbContext.CompanyOnboardingStatuses
                .Join(_dbContext.Clients,
                    cos => cos.ClientId,
                    client => client.Id,
                    (cos, client) => new { cos, client })
                .Join(_dbContext.Companies,
                    x => x.cos.CompanyId,
                    company => company.Id,
                    (x, company) => new { x.client, company })
                .Where(x =>
                    x.client.ClientName != null &&
                    x.client.ClientName != AppConstants.NOTAVAILABLE &&
                    !x.client.IsDeleted &&
                    (x.company.IsEnabled ?? false) == false && // Only companies that are NOT enabled
                    !x.company.IsDeleted)
                .Select(x => x.client.ClientName)
                .Distinct()
                .CountAsync();

            var elixirUsersCount = await _dbContext.Users
                .Where(u => !u.IsDeleted)
                .CountAsync();

            var elixirUserGroupsCount = await _dbContext.UserGroups
                .Where(ug => !ug.IsDeleted)
                .CountAsync();

            var companyStorageInGB = await _dbContext.Companies
                .Where(c => !c.IsDeleted && c.IsEnabled == true)
                .SumAsync(c => (double?)(c.CompanyStorageConsumedGb ?? 0)) ?? 0;
            var companyStorageInTB = (decimal)(companyStorageInGB / 1024); // Convert GB to TB

            var userStorageInGB = (decimal)(await _dbContext.Companies
                .Where(c => c.IsEnabled == true && !c.IsDeleted)
                .SumAsync(c => (decimal?)(c.CompanyStorageTotalGb ?? 0)) ?? 0);
            var userStorageInTB = (decimal)(userStorageInGB / 1048576m); // Convert MB to TB

            return new SuperAdminDashBoardDetailsDto
            {
                ActiveCompaniesCount = activeCompaniesCount,
                OnboardingCompaniesCount = onboardingCompaniesCount,
                ClientsCount = clientsCount,
                OnboardingClientsCount = onboardingClientsCount,
                ElixirUsersCount = elixirUsersCount,
                ElixirUserGroupsCount = elixirUserGroupsCount,
                CompanyStorageInGB = "0/" + companyStorageInTB.ToString("F3"),
                UserStorageInGB = "0/" + userStorageInTB.ToString("F3")
            };
        }
        catch
        {
            return null;
        }
    }
    public async Task<TmiDashBoardDetailsDto> GetTMIAdminDashBoardDetailsAsync(int userId)
    {
        // Count user groups for the user
        var userGroupsCount = await _dbContext.UserGroupMappings
            .Where(ugm => ugm.UserId == userId && !ugm.IsDeleted)
            .CountAsync();

        // Get all companies the user has access to (via ElixirUsers)
        var userCompanyIds = await _dbContext.ElixirUsers
            .Where(eu => eu.UserId == userId && !eu.IsDeleted)
            .Select(eu => eu.CompanyId)
            .Distinct()
            .ToListAsync();

        // Count active companies (not deleted, not under edit)
        var activeCompaniesCount = await _dbContext.Companies
            .Where(c => userCompanyIds.Contains(c.Id) && !c.IsDeleted && !String.IsNullOrEmpty(c.CompanyName) && (c.IsEnabled ?? false))
            //.Skip(1) // Skip the first company as per original logic
            .CountAsync();

        // Count onboarding companies (not deleted, under edit or onboarding status)
        var onboardingCompaniesCount = await _dbContext.CompanyOnboardingStatuses
            .Join(
                _dbContext.Companies.OrderBy(c => c.Id).Skip(1), // Skip the first company
                cos => cos.CompanyId,
                c => c.Id,
                (cos, c) => new { cos, c }
            )
            .Where(x => userCompanyIds.Contains(x.cos.CompanyId) && !x.cos.IsDeleted && !(x.c.IsEnabled ?? false))
            .CountAsync();

        return new TmiDashBoardDetailsDto
        {
            ActiveCompaniesCount = activeCompaniesCount,
            OnboardingCompaniesCount = onboardingCompaniesCount,
            UserGroupsCount = userGroupsCount,
        };
    }

    //public async Task<List<UserGroupDto>> GetCompany5TabCustomUserGroups(int companyId)
    //{
    //    // Exclude users in GroupId 1 or 3
    //    var excludedUserIds = await _dbContext.UserGroupMappings
    //        .Where(x => x.UserGroupId == (int)UserGroupRoles.AccountManager || x.UserGroupId == (int)UserGroupRoles.Checker)
    //        .Select(x => x.UserId)
    //        .Distinct()
    //        .ToListAsync();

    //    //// Step 1: Custom groups (not 1, 3, 5), IsAllCompanies = true (ViewOption = Custom), users not already mapped to company
    //    //var requiredSubMenuItemIds = new List<int> { AppConstants.SUBMENUITEM_ID_COMPANY_CREATION, AppConstants.SUBMENUITEM_ID_CLIENT_CREATION }; // If needed, else remove

    //    var customGroups = await (
    //        from ug in _dbContext.UserGroups
    //        join ugm in _dbContext.UserGroupMappings on ug.Id equals ugm.UserGroupId
    //        where ug.Id != (int)UserGroupRoles.AccountManager
    //            && ug.Id != (int)UserGroupRoles.Checker
    //            && _dbContext.UserGroupMenuMappings.Any(m =>
    //                m.UserGroupId == ug.Id &&
    //                (m.SubMenuItemId == AppConstants.SUBMENUITEM_ID_COMPANY_ONBOARDING_LIST ||
    //                 m.SubMenuItemId == AppConstants.SUBMENUITEM_ID_COMPANY_LIST) &&
    //                m.IsAllCompanies == false)
    //            && !_dbContext.ElixirUsers.Any(eu => eu.UserId == ugm.UserId && eu.CompanyId == companyId)
    //        group new { ug, ugm } by new { ug.Id, ug.GroupName } into g
    //        where g.Any()
    //        select new UserGroupDto
    //        {
    //            GroupId = g.Key.Id,
    //            GroupName = g.Key.GroupName
    //        }
    //    ).ToListAsync();

    //    // Step 2: Migration group (GroupId = 5), only if user not in GroupId 1 or 3, and not mapped to company
    //    var migrationGroup = await (
    //        from ugm in _dbContext.UserGroupMappings
    //        join ug in _dbContext.UserGroups on ugm.UserGroupId equals ug.Id
    //        where ug.Id == (int)UserGroupRoles.MigrationUser
    //            && !excludedUserIds.Contains(ugm.UserId)
    //            && !_dbContext.ElixirUsers.Any(eu => eu.UserId == ugm.UserId && eu.CompanyId == companyId)
    //        group new { ug, ugm } by new { ug.Id, ug.GroupName } into g
    //        where g.Any()
    //        select new UserGroupDto
    //        {
    //            GroupId = g.Key.Id,
    //            GroupName = g.Key.GroupName
    //        }
    //    ).ToListAsync();

    //    // Step 3: Combine both, ensure at least one mapped user is NOT in company users
    //    var finalResult = customGroups
    //        .UnionBy(migrationGroup, g => g.GroupId)
    //        .Where(group =>
    //        {
    //            var mappedUsers = _dbContext.UserGroupMappings
    //                .Where(um => um.UserGroupId == group.GroupId)
    //                .Select(um => um.UserId)
    //                .ToList();

    //            return mappedUsers.Any(uid => !_dbContext.ElixirUsers.Any(eu => eu.UserId == uid && eu.CompanyId == companyId));
    //        })
    //        .ToList();

    //    return finalResult;
    //}

    public async Task<List<UserGroupDto>> GetCompany5TabCustomUserGroups(int companyId, string? ScreenName = "")
    {
        //// Step 1: Exclude users in AccountManager or Checker groups
        //var excludedUserIds = await _dbContext.UserGroupMappings
        //    .Where(x => x.UserGroupId == (int)UserGroupRoles.AccountManager || x.UserGroupId == (int)UserGroupRoles.Checker)
        //    .Select(x => x.UserId)
        //    .Distinct()
        //    .ToListAsync();
        var excludedUserIds = await (
          from ugm in _dbContext.UserGroupMappings
          join eu in _dbContext.ElixirUsers.Where(eu => eu.CompanyId == companyId && !eu.IsDeleted) on ugm.UserId equals eu.UserId
          where ugm.UserGroupId == (int)UserGroupRoles.AccountManager || ugm.UserGroupId == (int)UserGroupRoles.Checker
          select ugm.UserId
      ).Distinct().ToListAsync();

        // Step 2: Users already mapped to the company
        var companyMappedUserIds = await _dbContext.ElixirUsers
            .Where(eu => eu.CompanyId == companyId && !eu.IsDeleted)
            .Select(eu => eu.UserId)
            .Distinct()
            .ToListAsync();

        // Parse screen flags
        bool isCompanyList = false;
        bool isCompanyOnboardingList = false;

        var screen = ScreenName?.Trim();
        if (!string.IsNullOrEmpty(screen))
        {
            if (string.Equals(screen, "Onboardinglist", StringComparison.OrdinalIgnoreCase))
                isCompanyOnboardingList = true;
            else if (string.Equals(screen, "CompanyList", StringComparison.OrdinalIgnoreCase))
                isCompanyList = true;
        }

        // Build allowed SubMenuItemIds based on flags
        var allowedSubMenuIds = new List<int>();
        if (isCompanyList)
            allowedSubMenuIds.Add(AppConstants.SUBMENUITEM_ID_COMPANY_LIST);
        if (isCompanyOnboardingList)
            allowedSubMenuIds.Add(AppConstants.SUBMENUITEM_ID_COMPANY_ONBOARDING_LIST);        

        // Step 3: Custom groups (join mappings and apply allowedSubMenuIds or fallback)
        var customGroupsQuery = from ugm in _dbContext.UserGroupMappings
                                join ug in _dbContext.UserGroups on ugm.UserGroupId equals ug.Id
                                join cls in _dbContext.UserGroupMenuMappings on ugm.UserGroupId equals cls.UserGroupId
                                where ug.GroupType == AppConstants.USER_GROUP_TYPE_CUSTOM
                                      && (allowedSubMenuIds.Count > 0
                                            ? allowedSubMenuIds.Contains(cls.SubMenuItemId)
                                            : (cls.SubMenuItemId == AppConstants.SUBMENUITEM_ID_COMPANY_ONBOARDING_LIST ||
                                               cls.SubMenuItemId == AppConstants.SUBMENUITEM_ID_COMPANY_LIST))
                                      && !(cls.IsAllCompanies ?? false)
                                      && (cls.ViewOnlyAccess ?? false)
                                      && !companyMappedUserIds.Contains(ugm.UserId)
                                      && !excludedUserIds.Contains(ugm.UserId)
                                group ug by new { ug.Id, ug.GroupName } into g
                                select new UserGroupDto
                                {
                                    GroupId = g.Key.Id,
                                    // Assuming UserGroupDto contains GroupName in actual DTO; include it here.
                                    GroupName = g.Key.GroupName
                                };

        var customGroupsList = await customGroupsQuery.Distinct().ToListAsync();

        // Step 4: Migration group (exclude excludedUserIds and users already mapped to the company)
        var migrationGroupList = await (
                from ugm in _dbContext.UserGroupMappings
                join ug in _dbContext.UserGroups on ugm.UserGroupId equals ug.Id
                join u in _dbContext.Users on ugm.UserId equals u.Id
                where ugm.UserGroupId == (int)UserGroupRoles.MigrationUser
                      && !u.IsDeleted
                      && !excludedUserIds.Contains(ugm.UserId)
                      && !companyMappedUserIds.Contains(ugm.UserId)
                group ug by new { ug.Id, ug.GroupName } into g
                select new UserGroupDto
                {
                    GroupId = g.Key.Id,
                    GroupName = g.Key.GroupName
                }
            ).Distinct().ToListAsync();

        // Step 5: Combine both lists (unique by GroupId) and return
        var finalResult = customGroupsList
            .UnionBy(migrationGroupList, g => g.GroupId)
            .ToList();

        return finalResult;
    }
    public async Task CloneElixirTenantDatabaseAsync(string sourceDb, string targetDb, string elasticPool)
    {
        try
        {
            var sql = "EXEC [dbo].[CloneElixirTenantDatabase] @SourceDB, @TargetDB, @ElasticPool";
            var parameters = new[]
            {
                new Microsoft.Data.SqlClient.SqlParameter("@SourceDB", sourceDb),
                new Microsoft.Data.SqlClient.SqlParameter("@TargetDB", targetDb),
                new Microsoft.Data.SqlClient.SqlParameter("@ElasticPool", elasticPool)
            };

            // Set command timeout to 10 minutes (600 seconds)
            var previousTimeout = _dbContext.Database.GetCommandTimeout();
            _dbContext.Database.SetCommandTimeout(600);
            try
            {
                await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);
            }
            finally
            {
                _dbContext.Database.SetCommandTimeout(previousTimeout);
            }
        }
        catch
        {

        }
    }

}
