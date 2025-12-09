using Elixir.Application.Common.Constants;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class ReportingAdminRepository : IReportingAdminRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public ReportingAdminRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> AddReportingAdminsAsync(int groupId, List<UserGroupReportingAdmin> reportingAdmins)
    {
        var adminEntities = reportingAdmins.Select(admin => new ReportingAdmin
        {
            UserGroupId = groupId,
            ReportingAdminId = admin.ReportingAdminId,
            IsSelected = admin.IsSelected,
            IsDeleted = false
        }).ToList();

        await _dbContext.ReportingAdmins.AddRangeAsync(adminEntities);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateReportingAdminsAsync(int groupId, List<UserGroupReportingAdmin> reportingAdmins)
    {
        var existingAdmins = _dbContext.ReportingAdmins.Where(ra => ra.UserGroupId == groupId);
        _dbContext.ReportingAdmins.RemoveRange(existingAdmins);

        var reportingAdminEntities = reportingAdmins.Select(admin => new ReportingAdmin
        {
            UserGroupId = groupId,
            ReportingAdminId = admin.ReportingAdminId,
            IsSelected = admin.IsSelected,
        }).ToList();

        _dbContext.ReportingAdmins.AddRange(reportingAdminEntities);
        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<List<UserGroupReportingAdmin>> GetReportingAdminsForRoleAsync(int groupId)
    {
        return await _dbContext.ReportingAdmins
            .Where(ra => ra.UserGroupId == groupId)
            .Select(ra => new UserGroupReportingAdmin
            {
                ReportingAdminId = ra.ReportingAdminId,
                IsSelected = ra.IsSelected ?? false,
                ReportingAdminName = ra.ReportingAdminId == 1 ? AppConstants.ELIXIR_REPORTING_ADMIN : ra.ReportingAdminId == 2 ? AppConstants.ELIXIR_COMPANY_CLIENT_REPORTING_ADMIN : string.Empty,
            })
            .ToListAsync();
    }

    public async Task<bool> DeleteReportingAdminsByUserGroupIdAsync(int groupId)
    {
        var reportingAdmins = _dbContext.ReportingAdmins.Where(ra => ra.UserGroupId == groupId).ToList();
        if (reportingAdmins.Count == 0)return true;
        _dbContext.ReportingAdmins.RemoveRange(reportingAdmins);
        return await _dbContext.SaveChangesAsync() > 0;
    }


    // Modified Section 4
    public async Task<List<UserGroup>> CheckForDuplicateReportingAdminsAsync(List<UserGroupReportingAdmin> reportingAdmins, int? userGroupId = 0)
    {
        // Prepare the set of reporting admins to check for duplicates
        var provided = reportingAdmins
            .Select(a => $"{a.ReportingAdminId}-{a.IsSelected}")
            .OrderBy(x => x)
            .ToHashSet();

        // Query all ReportingAdmins grouped by UserGroupId, excluding the current group if provided
        var reportingAdminsQuery = _dbContext.ReportingAdmins.AsQueryable();
        if (userGroupId > 0)
        {
            reportingAdminsQuery = reportingAdminsQuery.Where(ra => ra.UserGroupId != userGroupId);
        }
        var groupAdmins = await reportingAdminsQuery.ToListAsync();

        var groupPermissions = groupAdmins
            .GroupBy(ra => ra.UserGroupId)
            .Select(g => new
            {
                GroupId = g.Key,
                ReportingAdmins = g
                    .Select(ra => $"{ra.ReportingAdminId}-{ra.IsSelected}")
                    .OrderBy(x => x)
                    .ToHashSet()
            })
            .ToList();

        var matches = groupPermissions.Where(g => g.ReportingAdmins.SetEquals(provided)).ToList();
        var matchingGroupIds = matches.Select(m => m.GroupId).ToList();

        return await _dbContext.UserGroups
            .Where(u => matchingGroupIds.Contains(u.Id))
            .ToListAsync();
    }
}
