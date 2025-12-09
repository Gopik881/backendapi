using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Features.UserRightsMetatadata.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class ReportAccessRepository : IReportAccessRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public ReportAccessRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AddReportAccessAsync(int groupId, ReportingAccessDto reportAccess)
    {
        // Get all valid report IDs from the database
        var validReportIds = await _dbContext.Reports
            .Select(r => r.Id)
            .ToHashSetAsync();

        // Get all valid user IDs from the Users table
        var validUserIds = await _dbContext.Set<User>().Select(u => u.Id).ToHashSetAsync();

        // Use a valid user ID or handle if none exist
        int? userId = validUserIds.FirstOrDefault();
        if (userId == 0)
            return false; // No valid user, cannot proceed

        var reports = GetSelectedSubReports(reportAccess.Reports);
        // Filter out reports that do not exist in the database
        var reportAccessEntities = reports
            .Where(report => validReportIds.Contains(report.Id)) // <-- Only allow valid ReportId
            .Select(report => new ReportAccess
            {
                UserId = userId.Value,
                UserGroupId = groupId,
                ReportId = report.Id,
                CanDownload = reportAccess.CanDownloadReports,
                IsSelected = report.IsSelected
            })
            .ToList();

        if (reportAccessEntities.Count == 0)
            return true;

        await _dbContext.ReportAccesses.AddRangeAsync(reportAccessEntities);
        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<bool> UpdateReportAccessAsync(int groupId, ReportingAccessDto reportAccess)
    {
        // Fetch all valid user IDs from the Users table
        var validUserIds = await _dbContext.Set<User>().Select(u => u.Id).ToHashSetAsync();

        // Fetch all valid report IDs from the Reports table
        var validReportIds = await _dbContext.Reports.Select(r => r.Id).ToHashSetAsync();

        var existingReportAccess = await _dbContext.ReportAccesses
            .Where(ra => ra.UserGroupId == groupId)
            .ToListAsync();

        foreach (var subRep in reportAccess.Reports)
        {
            // Only proceed if the report exists
            if (!validReportIds.Contains(subRep.Id))
                continue;

            var existing = existingReportAccess.FirstOrDefault(ra => ra.ReportId == subRep.Id);
            if (existing != null)
            {
                existing.CanDownload = reportAccess.CanDownloadReports;
                existing.IsSelected = subRep.IsSelected;
            }
            else
            {
                // Set a valid UserId or null if not required
                int? userId = null;
                // If you have a logic to determine the user, set userId accordingly.
                // Otherwise, set to null or a valid default user id.
                // Example: userId = _dbContext.Users.FirstOrDefault()?.Id;

                // If UserId is required and must not be null, pick a valid one
                userId = validUserIds.FirstOrDefault();

                if (userId == 0)
                    continue; // No valid user, skip to avoid FK violation

                _dbContext.ReportAccesses.Add(new ReportAccess
                {
                    UserGroupId = groupId,
                    UserId = userId.Value,
                    ReportId = subRep.Id,
                    CanDownload = reportAccess.CanDownloadReports,
                    IsSelected = subRep.IsSelected
                });
            }
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<List<ReportingAccessDto>> GetReportAccessData(int userGroupId)
    {
        // Step 1: Fetch all categories and reports (not deleted)
        var categories = await _dbContext.Categories
            .Where(c => !c.IsDeleted)
            .ToListAsync();

        var reports = await _dbContext.Reports
            .Where(r => !r.IsDeleted)
            .ToListAsync();

        // Step 2: Fetch report access for the user group
        var reportAccesses = await _dbContext.ReportAccesses
            .Where(ra => ra.UserGroupId == userGroupId && !ra.IsDeleted)
            .ToListAsync();

        // Step 3: Build Categories (SelectionItemDto)
        var categoryDtos = categories.Select(c => new SelectionItemDto
        {
            Id = c.Id,
            Name = c.CategoryName,
            CategoryId = null,
            IsSelected = reports.Any(r => r.CategoryId == c.Id && reportAccesses.Any(ra => ra.ReportId == r.Id && ra.IsSelected == true)),
            SubReports = new List<SelectionItemDto>()
        }).ToList();

        // Step 4: Build Reports (SelectionItemDto) with SubReports (SelectionItemDto)
        var reportDtos = categories.Select(cat =>
            {
                var reportsInCategory = reports.Where(r => r.CategoryId == cat.Id).ToList();

                // Build SubReports list
                var subReports = reportsInCategory.Select(r => new SelectionItemDto
                {
                    Id = r.Id,
                    Name = r.ReportName,
                    CategoryId = r.CategoryId,
                    IsSelected = reportAccesses.FirstOrDefault(ra => ra.ReportId == r.Id)?.IsSelected ?? false,
                    SubReports = new List<SelectionItemDto>()
                }).ToList();

                // Category IsSelected is true if any subReport IsSelected is true
                bool isCategorySelected = subReports.Any(sr => sr.IsSelected);

                return new SelectionItemDto
                {
                    Id = cat.Id,
                    Name = cat.CategoryName,
                    CategoryId = cat.Id,
                    IsSelected = isCategorySelected,
                    SubReports = subReports
                };
            }).ToList();

        // Step 5: CanDownloadReports
        bool canDownload = reportAccesses.Any(ra => ra.CanDownload == true);

        // Step 6: Build final ReportingAccessDto
        var result = new List<ReportingAccessDto>
        {
            new ReportingAccessDto
            {
                SubCategories = categoryDtos,
                Reports = reportDtos,
                CanDownloadReports = canDownload
            }
        };

        return result;
    }

    public async Task<List<ReportAccessDto>> GetReportAccessDataV2(int userGroupId)
    {
        // Step 1: Fetch all categories and reports (not deleted)
        var categories = await _dbContext.Categories
            .Where(c => !c.IsDeleted)
            .ToListAsync();

        var reports = await _dbContext.Reports
            .Where(r => !r.IsDeleted)
            .ToListAsync();

        // Step 2: Fetch report access for the user group
        var reportAccesses = await _dbContext.ReportAccesses
            .Where(ra => ra.UserGroupId == userGroupId && !ra.IsDeleted)
            .ToListAsync();

        // Step 3: Build Categories (ReportCategoryDto)
        var categoryDtos = categories.Select(c => new ReportCategoryDto
        {
            Id = c.Id,
            Name = c.CategoryName,
            CategoryId = c.Id,
            IsSelected = reportAccesses.Any(ra => ra.ReportId == c.Id && ra.IsSelected == true),
            SubReports = new List<object>()
        }).ToList();

        // Step 4: Build Reports (ReportDto) with SubReports (SubReportDto)
        var reportDtos = categories.Select(cat =>
        {
            // All reports for this category
            var reportsInCategory = reports.Where(r => r.CategoryId == cat.Id).ToList();

            return new ReportDto
            {
                Id = cat.Id,
                Name = cat.CategoryName,
                CategoryId = cat.Id,
                IsSelected = false,
                SubReports = reportsInCategory.Select(r => new SubReportDto
                {
                    Id = r.Id,
                    Name = r.ReportName,
                    CategoryId = r.CategoryId,
                    IsSelected = reportAccesses.FirstOrDefault(ra => ra.ReportId == r.Id)?.IsSelected ?? false,
                    SubReports = new List<object>()
                }).ToList()
            };
        }).ToList();

        // Step 5: CanDownloadReports
        bool canDownload = reportAccesses.Any(ra => ra.CanDownload == true);

        // Step 6: Build final ReportAccessDto
        var result = new List<ReportAccessDto>
        {
            new ReportAccessDto
            {
                Categories = categoryDtos,
                Reports = reportDtos,
                CanDownloadReports = canDownload
            }
        };

        return result;
    }
    public async Task<List<SelectionItemDto>> GetReportsForRoleAsync(int userGroupId)
    {
        return await (from report in _dbContext.Reports
                      join access in _dbContext.ReportAccesses
                          on report.Id equals access.ReportId
                      where access.UserGroupId == userGroupId
                      select new SelectionItemDto
                      {
                          Id = report.Id,
                          Name = report.ReportName,
                          //Id = report.CategoryId,
                          IsSelected = access.IsSelected ?? false,
                          //canDownloadReport = access.CanDownload
                      })
                      .Distinct()
                      .ToListAsync();
    }

    public async Task<List<SelectionItemDto>> GetCategoriesForReportsAsync(int groupId)
    {
        var categories = await _dbContext.Categories
            .Select(c => new SelectionItemDto
            {
                Id = c.Id,
                Name = c.CategoryName,
                IsSelected = _dbContext.ReportAccesses
                    .Any(ra => ra.UserGroupId == groupId && _dbContext.Reports.Any(r => r.Id == ra.ReportId && r.CategoryId == c.Id) && ra.IsSelected == true),
            })
            .ToListAsync();

        return categories;
    }

    public async Task<bool> DeleteReportAccessByUserGroupIdAsync(int groupId)
    {
        var reportAccesses = _dbContext.ReportAccesses.Where(ra => ra.UserGroupId == groupId).ToList();
        if (reportAccesses.Count == 0)
            return true; // No records to delete, treat as success

        _dbContext.ReportAccesses.RemoveRange(reportAccesses);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    // Modified Section 3
    public async Task<List<UserGroup>> CheckForDuplicateReportAccessesAsync(ReportingAccessDto reportingAccessDto, int? userGroupId = 0)
    {
        // Step 1: Get all valid report IDs from the database
        var validReportIds = await _dbContext.Reports
            .Select(r => r.Id)
            .ToHashSetAsync();

        // Step 2: Build provided report access set (only valid reports), include CanDownloadReports
        var provided = GetSelectedSubReports(reportingAccessDto.Reports)
            .Where(r => validReportIds.Contains(r.Id)) // Uncommented for consistency
            .Select(r => $"{r.Id}-{r.IsSelected}-{reportingAccessDto.CanDownloadReports}")
            .OrderBy(x => x)
            .ToHashSet();

        // Step 3: Query all user group report accesses except the current group (if provided)
        var groupAccessQuery = _dbContext.ReportAccesses
            .Where(ra => !ra.IsDeleted);
        if (userGroupId > 0)
        {
            groupAccessQuery = groupAccessQuery.Where(ra => ra.UserGroupId != userGroupId);
        }
        var groupAccesses = await groupAccessQuery.ToListAsync();

        // Step 4: Group accesses by UserGroupId and build comparable sets, include CanDownload
        var groupPermissions = groupAccesses
            .GroupBy(ra => ra.UserGroupId)
            .Select(g => new
            {
                GroupId = g.Key,
                ReportAccesses = g
                    .Where(ra => validReportIds.Contains(ra.ReportId)) // Uncommented for consistency
                    .Select(ra => $"{ra.ReportId}-{ra.IsSelected}-{ra.CanDownload}")
                    .OrderBy(x => x)
                    .ToHashSet()
            })
            .ToList();

        // Step 5: Find all groups with matching report access set
        var matches = groupPermissions.Where(g => g.ReportAccesses.SetEquals(provided)).ToList();
        var matchingGroupIds = matches.Select(m => m.GroupId).ToList();

        return await _dbContext.UserGroups
            .Where(u => matchingGroupIds.Contains(u.Id))
            .ToListAsync();
    }
    // Pseudocode:
    // 1. Given a List<SelectionItemDto> Reports, extract all SubReports where IsSelected == true.
    // 2. Return a flat list of those selected SubReports.

    public List<SelectionItemDto> GetSelectedSubReports(List<SelectionItemDto> reports)
    {
        var selectedSubReports = new List<SelectionItemDto>();

        foreach (var report in reports)
        {
            if (report.SubReports != null)
            {
                selectedSubReports.AddRange(report.SubReports);
            }
        }

        return selectedSubReports;
    }
}
