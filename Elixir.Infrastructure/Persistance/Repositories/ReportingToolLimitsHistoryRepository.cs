using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Elixir.Application.Interfaces.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.Company5TabHistoryMapEntityToDictionary;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class ReportingToolLimitsHistoryRepository : IReportingToolLimitsHistoryRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public ReportingToolLimitsHistoryRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> Company5TabCreateReportingToolLimitsDataAsync(int companyId, int userId, Company5TabReportingToolLimitsDto Company5TabReportingTool, CancellationToken cancellationToken = default)
    {
        int lastVersion = _dbContext.ReportingToolLimitsHistories
           .Where(rh => rh.CompanyId == companyId && !rh.IsDeleted)
           .OrderByDescending(rh => rh.Version)
           .Select(rh => rh.Version)
           .FirstOrDefault();

        _dbContext.ReportingToolLimitsHistories.Add(new ReportingToolLimitsHistory
        {
            CompanyId = companyId,
            NoOfReportingAdmins = Company5TabReportingTool.NoOfReportingAdmins,
            NoOfCustomReportCreators = Company5TabReportingTool.NoOfCustomReportCreators,
            SavedReportQueriesInLibrary = Company5TabReportingTool.NoOfSavedReportQueriesCompany,
            DashboardsInLibrary = Company5TabReportingTool.NoOfDashboardsCompany,
            SavedReportQueriesPerUser = Company5TabReportingTool.NoOfSavedReportQueriesUsers,
            DashboardsInPersonalLibrary = Company5TabReportingTool.NoOfDashboardsUsers,
            LetterGenerationAdmins = Company5TabReportingTool.NoOfLetterGenerationAdmin,
            TemplatesSaved = Company5TabReportingTool.NoOfTemplatesSaved,
            Version = lastVersion + 1,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedAt = DateTime.UtcNow
        });

        return await _dbContext.SaveChangesAsync(cancellationToken)>0;
    }
    public async Task<Company5TabReportingToolLimitsDto?> GetCompany5TabLatestReportingToolLimitsHistoryAsync(int companyId, bool isPrevious, CancellationToken cancellationToken = default)
    {
        var versionQuery = _dbContext.ReportingToolLimitsHistories
            .Where(e => e.CompanyId == companyId && !e.IsDeleted);

        int latestVersion = isPrevious
            ? versionQuery.OrderByDescending(e => e.Version).Skip(1).Select(e => e.Version).FirstOrDefault()
            : (versionQuery.Max(e => (int?)e.Version) ?? 0);

        var reportingToolLimitsHistory = await _dbContext.ReportingToolLimitsHistories
            .Where(rh => rh.CompanyId == companyId && !rh.IsDeleted && rh.Version == latestVersion)
            .OrderByDescending(rh => rh.Version)
            .FirstOrDefaultAsync(cancellationToken);

        if (reportingToolLimitsHistory == null) return null;

        return new Company5TabReportingToolLimitsDto
        {
            NoOfReportingAdmins = reportingToolLimitsHistory.NoOfReportingAdmins,
            NoOfCustomReportCreators = reportingToolLimitsHistory.NoOfCustomReportCreators,
            NoOfSavedReportQueriesCompany = reportingToolLimitsHistory.SavedReportQueriesInLibrary,
            NoOfDashboardsCompany = reportingToolLimitsHistory.DashboardsInLibrary,
            NoOfSavedReportQueriesUsers = reportingToolLimitsHistory.SavedReportQueriesPerUser,
            NoOfDashboardsUsers = reportingToolLimitsHistory.DashboardsInPersonalLibrary,
            NoOfLetterGenerationAdmin = reportingToolLimitsHistory.LetterGenerationAdmins,
            NoOfTemplatesSaved = reportingToolLimitsHistory.TemplatesSaved
        };
    }
    public async Task<bool> WithdrawCompany5TabReportingToolLimitsHistoryAsync(int companyId, int userId, CancellationToken cancellationToken = default)
    {
        // Get the latest version for the given company and user
        int? latestVersion = await _dbContext.ReportingToolLimitsHistories
            .Where(e => e.CompanyId == companyId)
            .MaxAsync(e => (int?)e.Version, cancellationToken);
        if (latestVersion == null) return true;
        // Find all records to remove
        var recordsToRemove = _dbContext.ReportingToolLimitsHistories
            .Where(e => e.CompanyId == companyId && e.Version == latestVersion);
        if(recordsToRemove.Count() == 0) return true;
        _dbContext.ReportingToolLimitsHistories.RemoveRange(recordsToRemove);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
    public async Task<Company5TabHistoryDto> GetCompany5TabReportingToolLimitsHistoryByVersionAsync(
        int userId,
        int companyId,
        int versionNumber)
    {
        // Fetch the two most recent versions: requested and previous
        var histories = await _dbContext.ReportingToolLimitsHistories
            .Where(rh => rh.CompanyId == companyId && (rh.Version == versionNumber || rh.Version == versionNumber - 1))
            .OrderByDescending(rh => rh.Version)
            .ToListAsync();

        Company5TabHistoryMapEntitytoDictionary company5TabHistoryMapEntitytoDictionary = new Company5TabHistoryMapEntitytoDictionary();

        var company5TabReportingToolLimitsHistory = new Company5TabHistoryDto();

        foreach (var history in histories)
        {
            var mapped = company5TabHistoryMapEntitytoDictionary.MapEntityToDictionary(history)
                .ToDictionary(
                    keyValue => keyValue.Key,
                    keyValue => (object)new Dictionary<string, string> { { keyValue.Key, keyValue.Value } }
                );
            company5TabReportingToolLimitsHistory.Company5TabHistory[history.Version.ToString()] = mapped;
        }

        return company5TabReportingToolLimitsHistory;
    }
    


}
