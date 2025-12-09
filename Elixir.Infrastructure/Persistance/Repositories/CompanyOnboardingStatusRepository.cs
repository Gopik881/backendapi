using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Enums;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class CompanyOnboardingStatusRepository : ICompanyOnboardingStatusRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public CompanyOnboardingStatusRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    
    public async Task<Tuple<List<CompanyOnBoardingSummaryDto>, int>> GetPagedDelegateAdminCompaniesOnBoardingSummaryAsync(int userId, string searchTerm, int pageNumber, int pageSize)
    {
        // Old Master Id : 13 - Delegate Admin
        // Get the Group Id for the User if he is in Custom User Group
        var userGroupId = await _dbContext.UserGroups
            .Where(ug => !ug.IsDeleted && EF.Functions.Like(ug.GroupType, AppConstants.USER_GROUP_TYPE_CUSTOM) && _dbContext.UserGroupMappings.Any(ugm => ugm.UserId == userId && !ugm.IsDeleted && ugm.UserGroupId == ug.Id))
            .Select(ug => ug.Id).FirstOrDefaultAsync();

        //Get the UserGroupMenuMappings for the User Group Id for the SubMenu Item 'Company List' since this is for the company summary, this sub menu corresponds to the company list in the UI
        var IsAllCompanies = await _dbContext.UserGroupMenuMappings
            .Where(ugmm => _dbContext.SubMenuItems.Where(smi => EF.Functions.Like(smi.SubMenuItemName, AppConstants.COMPANY_ONBOARDING_ITEM_NAME) && ugmm.UserGroupId == userGroupId)
            .Select(smi => smi.Id).Contains(ugmm.SubMenuItemId)).Select(ugmm => ugmm.IsAllCompanies).FirstOrDefaultAsync() ?? false;

        // Check if 'Company List' screen has any permission (Create, ViewOnly, Approve, Edit) for the given user group
        bool hasCompanyListScreenPermission = await _dbContext.UserGroupMenuMappings
            .Where(ugmm =>
                ugmm.UserGroupId == userGroupId &&
                _dbContext.SubMenuItems
                    .Where(smi => EF.Functions.Like(smi.SubMenuItemName, AppConstants.COMPANY_ONBOARDING_ITEM_NAME))
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

        if (!hasCompanyListScreenPermission)
        {
            // If the user does not have permission to view the company onboarding list, return an empty list
            return new Tuple<List<CompanyOnBoardingSummaryDto>, int>(new List<CompanyOnBoardingSummaryDto>(), 0);
        }
        IQueryable<CompanyOnBoardingSummaryDto> companiesQuery = null;
        if (IsAllCompanies)
        {
            //As per the FRD bring all the companies even if the user does not belong to that companies, if incase the FRD changes use the filter like this ****eu.UserId == userId****.
            companiesQuery = _dbContext.Companies.Where(cm => _dbContext.ElixirUsers.Where(eu => !eu.IsDeleted).Select(eu => eu.CompanyId).Distinct().Contains(cm.Id) && (!cm.IsEnabled ?? false) && !cm.IsDeleted)
                .Join(_dbContext.Clients, cm => cm.ClientId, cl => cl.Id, (cm, cl) => new { cm, cl })
                .Select(c => new CompanyOnBoardingSummaryDto
                {
                    CompanyID = c.cm.Id,
                    CompanyName = c.cm.CompanyName,
                    ClientId = c.cl.Id,
                    ClientName = c.cm.ClientName ?? AppConstants.NOTAVAILABLE,
                    ClientCode = (string.IsNullOrEmpty(c.cm.ClientName) || c.cm.ClientName == AppConstants.NOTAVAILABLE) ? AppConstants.NOTAVAILABLE : c.cl.ClientCode,
                    UsersCount = _dbContext.ElixirUsers.Where(eu => eu.CompanyId == c.cm.Id && !eu.IsDeleted).Select(eu => eu.UserId).Distinct().Count(),
                    OnBoardingStatus = _dbContext.CompanyOnboardingStatuses
                        .Where(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted)
                        .Select(cos => cos.OnboardingStatus)
                        .FirstOrDefault(),
                    //CreatedOn = _dbContext.CompanyOnboardingStatuses
                    //    .Where(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted)
                    //    .OrderBy(ah => ah.CreatedAt)
                    //    .Select(cos => (DateTime?)cos.CreatedAt)
                    //    .FirstOrDefault() ?? DateTime.UtcNow,
                    LastUpdatedAt = _dbContext.CompanyOnboardingStatuses
                        .Where(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted)
                        .Select(cos => (DateTime?)cos.UpdatedAt)
                        .FirstOrDefault() ?? DateTime.UtcNow,
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
                                ?? string.Empty
                            ).FirstOrDefault() ?? string.Empty
                        )
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
                    UserId =
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
                                    .Select(u => u.Id)
                                    .FirstOrDefault()
                                != 0
                                    ? _dbContext.Users
                                        .Where(u => u.Id == createdById && !u.IsDeleted)
                                        .Select(u => u.Id)
                                        .FirstOrDefault()
                                    : (_dbContext.SuperUsers
                                        .Where(su => su.Id == createdById)
                                        .Select(su => su.Id)
                                        .FirstOrDefault())
                            ).FirstOrDefault()
                        )
                        : (_dbContext.CompanyOnboardingStatuses
                            .Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_APPROVED))
                        ? (from comp in _dbContext.Companies
                           join u in _dbContext.Users on comp.CreatedBy equals u.Id
                           where comp.Id == c.cm.Id && !u.IsDeleted
                           select u.Id).FirstOrDefault()
                        : (_dbContext.CompanyOnboardingStatuses
                            .Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_REJECTED))
                        ? (from comp in _dbContext.CompanyOnboardingStatuses
                           join u in _dbContext.Users on comp.UpdatedBy equals u.Id
                           where comp.CompanyId == c.cm.Id && !u.IsDeleted
                           select u.Id).FirstOrDefault()
                        : (from ch in _dbContext.CompanyHistories
                           join u in _dbContext.Users on ch.CreatedBy equals u.Id
                           where ch.CompanyId == c.cm.Id && !u.IsDeleted
                           orderby ch.Version descending
                           select u.Id).FirstOrDefault(),
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
        }
        else
        {
            //Apply UserId and GroupId Filter since user can be part of only one custom group
            companiesQuery = _dbContext.Companies.Where(cm => _dbContext.ElixirUsers.Where(eu => eu.UserId == userId && eu.UserGroupId == userGroupId && !eu.IsDeleted).Select(eu => eu.CompanyId).Distinct().Contains(cm.Id) && (!cm.IsEnabled ?? false) && !cm.IsDeleted)
                .Join(_dbContext.Clients, cm => cm.ClientId, cl => cl.Id, (cm, cl) => new { cm, cl })
                .Select(c => new CompanyOnBoardingSummaryDto
                {
                    CompanyID = c.cm.Id,
                    CompanyName = c.cm.CompanyName,
                    ClientId = c.cl.Id,
                    ClientName = c.cm.ClientName ?? AppConstants.NOTAVAILABLE,
                    ClientCode = (string.IsNullOrEmpty(c.cm.ClientName) || c.cm.ClientName == AppConstants.NOTAVAILABLE) ? AppConstants.NOTAVAILABLE : c.cl.ClientCode,
                    UsersCount = _dbContext.ElixirUsers.Where(eu => eu.CompanyId == c.cm.Id && !eu.IsDeleted).Select(eu => eu.UserId).Distinct().Count(),
                    UserName = (_dbContext.CompanyOnboardingStatuses.Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.IsActive && ah.OnboardingStatus.Contains(AppConstants.ONBOARDING_STATUS_APPROVED)))
                   ? _dbContext.Users.Where(u => u.Id == c.cm.CreatedBy && !u.IsDeleted).Select(u => new { UserName = u.FirstName + " " + u.LastName }).FirstOrDefault().UserName
                   : _dbContext.CompanyOnboardingStatuses.Where(cos => cos.CompanyId == c.cm.Id && !cos.IsDeleted).Join(_dbContext.Users, cos => cos.CreatedBy, user => user.Id, (cos, user) => new
                   {
                       UserName = user.FirstName + " " + user.LastName
                   }).FirstOrDefault().UserName ?? string.Empty,
                    OnBoardingStatus = _dbContext.CompanyOnboardingStatuses.Where(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted).Select(cos => cos.OnboardingStatus).FirstOrDefault(),
                    //CreatedOn = _dbContext.CompanyOnboardingStatuses.Where(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted).Select(cos => (DateTime?)cos.CreatedAt).FirstOrDefault() ?? DateTime.UtcNow,
                    LastUpdatedAt = _dbContext.CompanyOnboardingStatuses.Where(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted).Select(cos => (DateTime?)cos.UpdatedAt).FirstOrDefault() ?? DateTime.UtcNow,
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
                                ?? string.Empty
                            ).FirstOrDefault() ?? string.Empty
                        )
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
                    UserId =
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
                                    .Select(u => u.Id)
                                    .FirstOrDefault()
                                != 0
                                    ? _dbContext.Users
                                        .Where(u => u.Id == createdById && !u.IsDeleted)
                                        .Select(u => u.Id)
                                        .FirstOrDefault()
                                    : (_dbContext.SuperUsers
                                        .Where(su => su.Id == createdById)
                                        .Select(su => su.Id)
                                        .FirstOrDefault())
                            ).FirstOrDefault()
                        )
                        : (_dbContext.CompanyOnboardingStatuses
                            .Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_APPROVED))
                        ? (from comp in _dbContext.Companies
                           join u in _dbContext.Users on comp.CreatedBy equals u.Id
                           where comp.Id == c.cm.Id && !u.IsDeleted
                           select u.Id).FirstOrDefault()
                        : (_dbContext.CompanyOnboardingStatuses
                            .Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_REJECTED))
                        ? (from comp in _dbContext.CompanyOnboardingStatuses
                           join u in _dbContext.Users on comp.UpdatedBy equals u.Id
                           where comp.CompanyId == c.cm.Id && !u.IsDeleted
                           select u.Id).FirstOrDefault()
                        : (from ch in _dbContext.CompanyHistories
                           join u in _dbContext.Users on ch.CreatedBy equals u.Id
                           where ch.CompanyId == c.cm.Id && !u.IsDeleted
                           orderby ch.Version descending
                           select u.Id).FirstOrDefault(),
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
        }
        // Get total count
        //var totalCount = await companiesQuery.CountAsync();
        //// Apply pagination
        // Pseudocode plan:
        // 1. Materialize companiesQuery to a list before applying the search filter, since the filter uses complex expressions not translatable to SQL.
        // 2. Apply the search filter in-memory using LINQ-to-Objects after ToListAsync().
        // 3. Apply pagination after filtering.

        var companiesList = await companiesQuery.ToListAsync();

        var filteredCompanies = companiesList.Where(c =>
            (!string.IsNullOrEmpty(c.CompanyName) && c.CompanyName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(c.ClientName) && c.ClientName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            //(!string.IsNullOrEmpty(c.UserName) && c.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(c.OnBoardingStatus) && c.OnBoardingStatus.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||

            (c.UsersCount.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            //(!string.IsNullOrEmpty(c.CreatedBy) && c.CreatedBy.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(c.LastUpdatedBy) && c.LastUpdatedBy.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||

            (c.CreatedOn.ToString("dd/MM/yyyy").Contains(searchTerm)) ||
            (c.LastUpdatedAt != null && c.LastUpdatedAt.Value.ToString("dd/MM/yyyy").Contains(searchTerm))
        ).ToList();

        // Order by the most relevant last-updated timestamp (descending).
        // Use LastUpdatedOn (company history) first, then LastUpdatedAt (onboarding status), then CreatedOn as fallback.
        var orderedCompanies = filteredCompanies
            .OrderByDescending(c => c.LastUpdatedOn ?? c.LastUpdatedAt ?? c.CreatedOn)
            .ThenByDescending(c => c.LastUpdatedAt ?? c.CreatedOn)
            .ToList();

        var totalCount = orderedCompanies.Count;
        var pagedCompanies = orderedCompanies
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new Tuple<List<CompanyOnBoardingSummaryDto>, int>(pagedCompanies, totalCount);
    }


    // Pseudocode plan (detailed):
    // 1. Keep existing projection (companiesQuery) and materialization to companiesList as-is to avoid changing behavior.
    // 2. Apply the existing in-memory search filter on companiesList to produce filteredCompanies.
    // 3. To ensure "latest record on top" without disturbing other functionality, sort the filtered list by
    //    the most relevant last-updated timestamp, using a safe fallback chain:
    //      - LastUpdatedOn (from CompanyHistories projection) if present
    //      - LastUpdatedAt (from CompanyOnboardingStatuses projection) if present
    //      - CreatedOn (fallback)
    //    Use descending order so the most recently updated records come first.
    // 4. After ordering, perform pagination (Skip/Take) and return the paged subset and total count.
    // 5. Keep all other search criteria and returned values intact so existing behavior is preserved.
    //
    // Implementation notes:
    // - Use null-coalescing for DateTime? values to pick the correct ordering key.
    // - Do ordering in-memory after materialization and filtering so we don't affect SQL translation.
    // - Do not change any other logic or fields to avoid breaking behavior.

    public async Task<Tuple<List<CompanyOnBoardingSummaryDto>, int>> GetPagedSuperAdminCompaniesOnBoardingSummaryAsync(int userId, bool IsSuperUser, string searchTerm, int pageNumber, int pageSize)
    {
        // Old Master Id : 12 - Super Admin
        var companiesQuery = _dbContext.Companies.Where(cm => !cm.IsDeleted && (!cm.IsEnabled ?? false))
            .Join(_dbContext.Clients, cm => cm.ClientId, cl => cl.Id, (cm, cl) => new { cm, cl })
            .Select(c => new CompanyOnBoardingSummaryDto
            {
                CompanyID = c.cm.Id,
                CompanyName = c.cm.CompanyName ?? string.Empty, // CS8601 fix
                ClientId = c.cl.Id,
                ClientName = c.cm.ClientName ?? AppConstants.NOTAVAILABLE, // CS8601 fix
                ClientCode = (string.IsNullOrEmpty(c.cm.ClientName) || c.cm.ClientName == AppConstants.NOTAVAILABLE) ? AppConstants.NOTAVAILABLE : c.cl.ClientCode,
                UsersCount = _dbContext.ElixirUsers.Where(eu => eu.CompanyId == c.cm.Id && !eu.IsDeleted).Select(eu => eu.UserId).Distinct().Count(),
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
                                ?? string.Empty
                            ).FirstOrDefault() ?? string.Empty
                        )
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

                UserId =
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
                                    .Select(u => u.Id)
                                    .FirstOrDefault()
                                != 0
                                    ? _dbContext.Users
                                        .Where(u => u.Id == createdById && !u.IsDeleted)
                                        .Select(u => u.Id)
                                        .FirstOrDefault()
                                    : (_dbContext.SuperUsers
                                        .Where(su => su.Id == createdById)
                                        .Select(su => su.Id)
                                        .FirstOrDefault())
                            ).FirstOrDefault()
                        )
                        : (_dbContext.CompanyOnboardingStatuses
                            .Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_APPROVED))
                        ? (from comp in _dbContext.Companies
                           join u in _dbContext.Users on comp.CreatedBy equals u.Id
                           where comp.Id == c.cm.Id && !u.IsDeleted
                           select u.Id).FirstOrDefault()
                        : (_dbContext.CompanyOnboardingStatuses
                            .Any(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_REJECTED))
                        ? (from comp in _dbContext.CompanyOnboardingStatuses
                           join u in _dbContext.Users on comp.UpdatedBy equals u.Id
                           where comp.CompanyId == c.cm.Id && !u.IsDeleted
                           select u.Id).FirstOrDefault()
                        : (from ch in _dbContext.CompanyHistories
                           join u in _dbContext.Users on ch.CreatedBy equals u.Id
                           where ch.CompanyId == c.cm.Id && !u.IsDeleted
                           orderby ch.Version descending
                           select u.Id).FirstOrDefault(),

                OnBoardingStatus = _dbContext.CompanyOnboardingStatuses
                    .Where(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted)
                    .Select(cos => cos.OnboardingStatus)
                    .FirstOrDefault() ?? string.Empty, // CS8601 fix

                CreatedOn = _dbContext.CompanyOnboardingStatuses
                                                           .Where(ah => ah.CompanyId == c.cm.Id && !ah.IsDeleted)
                                                           .OrderBy(ah => ah.CreatedAt)
                                                           .Select(cos => (DateTime?)cos.CreatedAt)
                                                           .FirstOrDefault() ?? DateTime.UtcNow, // Use UtcNow as a safe default
                LastUpdatedOn = (from ch in _dbContext.CompanyHistories
                                 where ch.CompanyId == c.cm.Id
                                 orderby ch.Version descending
                                 select ch.LastUpdatedOn).FirstOrDefault() ?? c.cm.LastUpdatedOn,

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

        var filteredCompanies = companiesList.Where(c =>
            (!string.IsNullOrEmpty(c.CompanyName) && c.CompanyName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(c.ClientName) && c.ClientName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            //(!string.IsNullOrEmpty(c.UserName) && c.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(c.OnBoardingStatus) && c.OnBoardingStatus.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||

            (c.UsersCount.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            //(!string.IsNullOrEmpty(c.CreatedBy) && c.CreatedBy.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(c.LastUpdatedBy) && c.LastUpdatedBy.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||

            // Add CreatedOn and LastUpdatedAt date search (format: dd/MM/yyyy)
            (c.CreatedOn.ToString("dd/MM/yyyy").Contains(searchTerm)) ||
            (c.LastUpdatedAt != null && c.LastUpdatedAt.Value.ToString("dd/MM/yyyy").Contains(searchTerm))
        ).ToList();

        // Order by the most relevant last-updated timestamp (descending).
        // Use LastUpdatedOn (company history) first, then LastUpdatedAt (onboarding status), then CreatedOn as fallback.
        var orderedCompanies = filteredCompanies
            .OrderByDescending(c => c.LastUpdatedOn ?? c.LastUpdatedAt ?? c.CreatedOn)
            .ThenByDescending(c => c.LastUpdatedAt ?? c.CreatedOn)
            .ToList();

        var totalCount = orderedCompanies.Count;

        var pagedCompanies = orderedCompanies
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new Tuple<List<CompanyOnBoardingSummaryDto>, int>(pagedCompanies, totalCount);
    }

    /* 
     PSEUDOCODE / PLAN (detailed):
     1. Keep all existing materialization and projection logic as-is to avoid changing behavior.
     2. After building resultList of CompanyTMIOnBoardingSummaryDto, perform in-memory filtering as currently implemented.
     3. To ensure that records created or updated appear on top, order the filtered results in-memory by:
        - Primary key: LastUpdatedOn (from CompanyHistories or company.LastUpdatedOn) descending.
        - Secondary key: LastUpdatedAt (if available) descending.
        - Tertiary key: CreatedOn descending as a final fallback.
     4. Use null-coalescing to pick the most relevant timestamp for each record.
     5. After ordering, calculate totalCount from the ordered list and then apply pagination (Skip/Take).
     6. Return the paged subset and totalCount without changing any other DTO fields or logic.
     NOTE: All ordering and filtering are performed in-memory after ToListAsync() to avoid EF translation issues
           and to preserve existing complex lookup semantics.
    */
    public async Task<Tuple<List<CompanyTMIOnBoardingSummaryDto>, int>> GetPagedTMIUsersCompaniesOnBoardingSummaryAsync(int userId, string searchTerm, int pageNumber, int pageSize, bool isDashboard = false)
    {

        // Returns true if the user is in a custom group (not a TMI user), false otherwise
        bool isCustomGroupUser = await _dbContext.UserGroups
            .AnyAsync(ug => !ug.IsDeleted
                && EF.Functions.Like(ug.GroupType, AppConstants.USER_GROUP_TYPE_CUSTOM)
                && _dbContext.UserGroupMappings.Any(ugm => ugm.UserId == userId && !ugm.IsDeleted && ugm.UserGroupId == ug.Id));

        //Old Master Id 16 - for TMI Users
        //Collect the UserGroup Id for the User if he is in Default User Group
        var userGroupIds = await _dbContext.UserGroups
            .Where(ug => !ug.IsDeleted && EF.Functions.Like(ug.GroupType, AppConstants.USER_GROUP_TYPE_DEFAULT) && _dbContext.UserGroupMappings.Any(ugm => ugm.UserId == userId && !ugm.IsDeleted && ugm.UserGroupId == ug.Id))
            .Select(ug => ug.Id).ToListAsync();

        // Materialize ElixirUsers and UserGroups for the user and relevant companies
        var elixirUsersForUser = await _dbContext.ElixirUsers
            .Where(eu => eu.UserId == userId && !eu.IsDeleted && userGroupIds.Contains(eu.UserGroupId))
            .ToListAsync();

        var userGroupIdsForUser = elixirUsersForUser.Select(eu => eu.UserGroupId).Distinct().ToList();
        var userGroupsDict = await _dbContext.UserGroups
            .Where(ug => userGroupIdsForUser.Contains(ug.Id))
            .ToDictionaryAsync(ug => ug.Id, ug => ug.GroupName);

        // Get the list of company IDs for the user
        var userCompanyIds = elixirUsersForUser.Select(eu => eu.CompanyId).Distinct().ToList();

        // Materialize companies and clients to avoid translation issues
        var companies = await _dbContext.Companies
            .Where(cm => userCompanyIds.Contains(cm.Id) && !cm.IsDeleted && (!cm.IsEnabled ?? false))
            .ToListAsync();

        var clientIds = companies.Select(c => c.ClientId).Distinct().ToList();
        var clients = await _dbContext.Clients
            .Where(cl => clientIds.Contains(cl.Id))
            .ToListAsync();

        // Materialize related data for projection
        var companyOnboardingStatuses = await _dbContext.CompanyOnboardingStatuses
            .Where(cos => userCompanyIds.Contains(cos.CompanyId) && !cos.IsDeleted)
            .ToListAsync();

        var companyHistories = await _dbContext.CompanyHistories
            .Where(ch => userCompanyIds.Contains(ch.CompanyId ?? 0))
            .ToListAsync();

        var users = await _dbContext.Users
            .Where(u => !u.IsDeleted)
            .ToListAsync();

        var superUser = await _dbContext.SuperUsers.FirstOrDefaultAsync();

        var clientCompaniesMappings = await _dbContext.ClientCompaniesMappings
            .Where(cl => clientIds.Contains(cl.ClientId))
            .ToListAsync();

        var elixirUsers = await _dbContext.ElixirUsers
            .Where(eu => clientIds.Contains(eu.ClientId ?? 0))
            .ToListAsync();

        var resultList = new List<CompanyTMIOnBoardingSummaryDto>();

        foreach (var company in companies)
        {
            var client = clients.FirstOrDefault(cl => cl.Id == company.ClientId);
            var onboardingStatus = companyOnboardingStatuses.FirstOrDefault(cos => cos.CompanyId == company.Id);
            var histories = companyHistories.Where(ch => ch.CompanyId == company.Id).OrderByDescending(ch => ch.Version).ToList();

            string lastUpdatedBy = string.Empty;
            int? userIdValue = 0;

            // Apply the same logic as LastUpdatedBy in the LINQ projection
            if (!isCustomGroupUser && onboardingStatus != null && onboardingStatus.OnboardingStatus == AppConstants.ONBOARDING_STATUS_NEW)
            {
                //lastUpdatedBy = superUser != null ? $"{superUser.FirstName} {superUser.LastName}" : string.Empty;
                //userIdValue = superUser?.Id;
                var cos = companyOnboardingStatuses
                   .Where(c => c.CompanyId == company.Id)
                   .OrderByDescending(c => c.CreatedAt)
                   .FirstOrDefault();
                if (cos != null)
                {
                    var user = users.FirstOrDefault(u => u.Id == cos.CreatedBy);
                    lastUpdatedBy = user != null ? $"{user.FirstName} {user.LastName}" : string.Empty;
                    userIdValue = user?.Id;
                    if (user == null)
                    {
                        lastUpdatedBy = superUser != null ? $"{superUser.FirstName} {superUser.LastName}" : string.Empty;
                        userIdValue = (int)Roles.SuperAdmin;
                    }
                }
            }
            else if (onboardingStatus != null && onboardingStatus.OnboardingStatus == AppConstants.ONBOARDING_STATUS_APPROVED)
            {
                var companyCreator = users.FirstOrDefault(u => u.Id == company.CreatedBy && !u.IsDeleted);
                lastUpdatedBy = companyCreator != null ? $"{companyCreator.FirstName} {companyCreator.LastName}" : string.Empty;
                userIdValue = companyCreator?.Id;
            }
            else if (onboardingStatus != null && onboardingStatus.OnboardingStatus == AppConstants.ONBOARDING_STATUS_REJECTED)
            {

                // With this code to join onboardingstatuses with users table:
                var onboardingStatusRecord = companyOnboardingStatuses.FirstOrDefault(cos => cos.CompanyId == company.Id && !cos.IsDeleted);
                User? updatedByUser = null;
                if (onboardingStatusRecord != null && onboardingStatusRecord.UpdatedBy.HasValue)
                {
                    updatedByUser = users.FirstOrDefault(u => u.Id == onboardingStatusRecord.UpdatedBy.Value && !u.IsDeleted);
                }
                lastUpdatedBy = updatedByUser != null ? $"{updatedByUser.FirstName} {updatedByUser.LastName}" : string.Empty;
                userIdValue = updatedByUser?.Id;
            }
            else
            {
                var lastHistory = histories.FirstOrDefault();
                if (lastHistory != null)
                {
                    var user = users.FirstOrDefault(u => u.Id == lastHistory.CreatedBy && !u.IsDeleted);
                    lastUpdatedBy = user != null ? $"{user.FirstName} {user.LastName}" : string.Empty;
                    userIdValue = user?.Id;
                }
            }

            string usersGroups = string.Join(", ",
                elixirUsersForUser
                    .Where(eu => eu.CompanyId == company.Id)
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
            ) ?? string.Empty;

            string createdBy = string.Empty;
            if (isCustomGroupUser)
            {
                var cos = companyOnboardingStatuses
                    .Where(c => c.CompanyId == company.Id)
                    .OrderByDescending(c => c.CreatedAt)
                    .FirstOrDefault();
                if (cos != null)
                {
                    var user = users.FirstOrDefault(u => u.Id == cos.CreatedBy);
                    createdBy = user != null ? $"{user.FirstName} {user.LastName}" : string.Empty;
                    userIdValue = user?.Id;
                }
            }
            else
            {
                var cos = companyOnboardingStatuses
                   .Where(c => c.CompanyId == company.Id)
                   .OrderByDescending(c => c.CreatedAt)
                   .FirstOrDefault();
                if (cos != null)
                {
                    var user = users.FirstOrDefault(u => u.Id == cos.CreatedBy);
                    createdBy = user != null ? $"{user.FirstName} {user.LastName}" : string.Empty;
                    //userIdValue = user?.Id;
                    if (user == null)
                    {
                        createdBy = superUser != null ? $"{superUser.FirstName} {superUser.LastName}" : string.Empty;
                        if (userIdValue == 0)
                            userIdValue = superUser.Id;
                    }
                }
            }

            resultList.Add(new CompanyTMIOnBoardingSummaryDto
            {
                IsActive = false,
                CompanyID = company.Id,
                CompanyName = company.CompanyName ?? AppConstants.NOTAVAILABLE,
                ClientId = client?.Id ?? 0,
                ClientName = (string.IsNullOrEmpty(company?.ClientName) || company.ClientName == AppConstants.NOTAVAILABLE)
                    ? AppConstants.NOTAVAILABLE
                    : client?.ClientName ?? AppConstants.NOTAVAILABLE,
                ClientCode = (string.IsNullOrEmpty(company?.ClientName) || company.ClientName == AppConstants.NOTAVAILABLE)
                    ? AppConstants.NOTAVAILABLE
                    : client?.ClientCode ?? AppConstants.NOTAVAILABLE,
                LastUpdatedBy = lastUpdatedBy,
                UserId = userIdValue ?? 0,
                UsersGroups = usersGroups,
                OnBoardingStatus = onboardingStatus?.OnboardingStatus ?? string.Empty,
                CreatedOn = onboardingStatus?.CreatedAt ?? DateTime.UtcNow,
                LastUpdatedOn = onboardingStatus?.UpdatedAt ?? company.LastUpdatedOn,
                ClientCompaniesCount = clientCompaniesMappings.Where(cl => cl.ClientId == client?.Id).Select(cl => cl.CompanyId).Distinct().Count(),
                ClientAccountManagersCount = elixirUsers.Where(eu => eu.ClientId == client?.Id).Select(eu => eu.UserId).Distinct().Count(),
                CreatedBy = createdBy,
                CompanyStatus = onboardingStatus?.IsActive
            });
        }
        // Filtering
        var filteredCompanies = resultList.Where(c =>
            (!string.IsNullOrEmpty(c.CompanyName) && c.CompanyName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(c.ClientName) && c.ClientName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            //(!string.IsNullOrEmpty(c.UserName) && c.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(c.OnBoardingStatus) && c.OnBoardingStatus.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            //(!string.IsNullOrEmpty(c.CreatedBy) && c.CreatedBy.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(c.LastUpdatedBy) && c.LastUpdatedBy.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(c.UsersGroups) && c.UsersGroups.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(searchTerm) && isDashboard
                    ? (c.LastUpdatedOn != null && c.LastUpdatedOn.Value.ToString("dd/MM/yyyy").Contains(searchTerm))
                    : (c.CreatedOn.ToString("dd/MM/yyyy").Contains(searchTerm) ||
                      (c.LastUpdatedOn != null && c.LastUpdatedOn.Value.ToString("dd/MM/yyyy").Contains(searchTerm))))

            ).ToList();

        // Order by last-updated timestamp so that recently created/updated records appear on top.
        var orderedCompanies = filteredCompanies
            .OrderByDescending(c => c.LastUpdatedOn ?? c.LastUpdatedAt ?? c.CreatedOn)
            .ThenByDescending(c => c.LastUpdatedAt ?? c.CreatedOn)
            .ToList();

        var totalCount = orderedCompanies.Count;
        var pagedCompanies = orderedCompanies
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new Tuple<List<CompanyTMIOnBoardingSummaryDto>, int>(pagedCompanies, totalCount);
    }


    public async Task<bool> IsCompanyOnboardingStatusDataExistsAsync(int companyId)
    {
        return await _dbContext.CompanyOnboardingStatuses
            .AnyAsync(ah => ah.CompanyId == companyId && !ah.IsDeleted && ah.OnboardingStatus == AppConstants.ONBOARDING_STATUS_PENDING);
    }

    public async Task<bool?> GetCompanyActiveStatus(int companyId)
    {
        return await _dbContext.Companies
            .Where(ah => ah.Id == companyId && !ah.IsDeleted)
            .Select(ah => ah.IsEnabled)
            .FirstOrDefaultAsync();
    }

    public async Task<string> GetCompanyOnBoardingStatus(int companyId)
    {
        var onboardingStatus = await _dbContext.CompanyOnboardingStatuses
            .Where(ah => ah.CompanyId == companyId && !ah.IsDeleted)
            .Select(ah => ah.OnboardingStatus)
            .FirstOrDefaultAsync();
        return onboardingStatus ?? AppConstants.ONBOARDING_STATUS_NEW; // Default to New if not found
    }

    // Pseudocode plan:
    // 1. Do NOT use a 'using' statement or manually call Dispose() on the DbContext in this repository method.
    // 2. The DbContext should be injected and managed by the DI container (e.g., as Scoped).
    // 3. If you use a 'using' statement like 'using(var context = new ElixirHRDbContext())', the context will be disposed before the async operation completes, causing an ObjectDisposedException.
    // 4. Always let the DI container manage the DbContext's lifetime.

    public async Task<bool> UpdateOnboardingStatusAsync(int companyId, int userId, string newStatus, bool isWithDraw = false)
    {
        
        // 1. Check company existence
        var companyExists = await _dbContext.Accounts
            .AnyAsync(c => c.CompanyId == companyId && !c.IsDeleted);

        // 2. If company exists, set newStatus to APPROVED
        if (companyExists && isWithDraw)
        {
            newStatus = AppConstants.ONBOARDING_STATUS_APPROVED;
        }

        // 3. Retrieve existing onboarding status record
        var onboardingStatus = await _dbContext.CompanyOnboardingStatuses
            .FirstOrDefaultAsync(ah => ah.CompanyId == companyId && !ah.IsDeleted);

        if (onboardingStatus != null)
        {
            // 4a. Update fields
            onboardingStatus.OnboardingStatus = newStatus;
            onboardingStatus.UpdatedAt = DateTime.UtcNow;
            onboardingStatus.UpdatedBy = userId;

            try
            {
                // 4c. Persist changes
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch
            {
                // Optionally log the exception outside of this method
                return false;
            }
        }

        // 5. No onboarding status record found
        return false;
    }
    public async Task<int> GetCompanyIdByClientIdAsync(int clientId)
    {
        return await _dbContext.CompanyOnboardingStatuses
            .Where(m => m.ClientId == clientId)
            .Select(comp => (int)comp.CompanyId)
            .FirstOrDefaultAsync();
    }


}
