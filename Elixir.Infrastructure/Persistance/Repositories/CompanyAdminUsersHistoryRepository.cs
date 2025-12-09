using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.Company5TabHistoryMapEntityToDictionary;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class CompanyAdminUsersHistoryRepository : ICompanyAdminUsersHistoryRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public CompanyAdminUsersHistoryRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Company5TabCreateCompanyAdminDataAsync(int companyId, int userId, Company5TabCompanyAdminDto company5TabCompanyAdmin, CancellationToken cancellationToken = default)
    {
        int lastVersion = _dbContext.CompanyAdminUsersHistories
            .Where(ch => ch.CompanyId == companyId && !ch.IsDeleted)
            .OrderByDescending(ch => ch.Version)
            .Select(ch => ch.Version)
            .FirstOrDefault();


        _dbContext.CompanyAdminUsersHistories.Add(new CompanyAdminUsersHistory
        {
            CompanyId = companyId,
            FirstName = company5TabCompanyAdmin.CompanyAdminFirstName,
            LastName = company5TabCompanyAdmin.CompanyAdminLastName,
            Email = company5TabCompanyAdmin.CompanyAdminEmailId,
            TelephoneCodeId = company5TabCompanyAdmin.TelephoneCodeId,
            PhoneNumber = company5TabCompanyAdmin.CompanyAdminPhoneNo,
            Designation = company5TabCompanyAdmin.CompanyAdminDesignation,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            IsEnabled = true,
            Version = lastVersion + 1,
            UpdatedAt = DateTime.UtcNow,
        });

        return await _dbContext.SaveChangesAsync(cancellationToken)>0;
    }
    public async Task<Company5TabCompanyAdminDto?> GetCompany5TabLatestCompanyAdminHistoryAsync(int companyId, bool isPrevious, CancellationToken cancellationToken = default)
    {
        var versionQuery = _dbContext.CompanyAdminUsersHistories
            .Where(e => e.CompanyId == companyId && !e.IsDeleted);

        int latestVersion = isPrevious
            ? versionQuery.OrderByDescending(e => e.Version).Skip(1).Select(e => e.Version).FirstOrDefault()
            : (versionQuery.Max(e => (int?)e.Version) ?? 0);

        var adminHistory = await _dbContext.CompanyAdminUsersHistories
            .Where(ah => ah.CompanyId == companyId && !ah.IsDeleted && ah.Version == latestVersion)
            .OrderByDescending(ah => ah.Version)
            .FirstOrDefaultAsync(cancellationToken);

        if (adminHistory == null) return null;

        string? phoneShortCutCode = await _dbContext.TelephoneCodeMasters
            .Where(s => s.Id == adminHistory.TelephoneCodeId)
            .Select(s => s.TelephoneCode)
            .FirstOrDefaultAsync(cancellationToken);

        return new Company5TabCompanyAdminDto
        {
            CompanyAdminFirstName = adminHistory.FirstName,
            CompanyAdminLastName = adminHistory.LastName,
            CompanyAdminEmailId = adminHistory.Email,
            CompanyAdminPhoneCode = phoneShortCutCode ?? string.Empty,
            TelephoneCodeId = adminHistory.TelephoneCodeId,
            CompanyAdminPhoneNo = adminHistory.PhoneNumber,
            CompanyAdminDesignation = adminHistory.Designation
        };
    }
    public async Task<bool> WithdrawCompany5TabCompanyAdminHistoryAsync(int companyId, int userId, CancellationToken cancellationToken = default)
    {
        // Get the latest version for the given company and user
        int? latestAdminInfoVersion = await _dbContext.CompanyAdminUsersHistories
            .Where(e => e.CompanyId == companyId)
            .MaxAsync(e => (int?)e.Version, cancellationToken);
        if (latestAdminInfoVersion == null) return true;
        // Find all records to remove
        var recordsToRemove = _dbContext.CompanyAdminUsersHistories
            .Where(e => e.CompanyId == companyId && e.Version == latestAdminInfoVersion);
        if(recordsToRemove.Count() == 0) return true; // Nothing to remove
        _dbContext.CompanyAdminUsersHistories.RemoveRange(recordsToRemove);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
    public async Task<Company5TabHistoryDto> GetCompany5TabCompanyAdminHistoryByVersionAsync(
        int userId,
        int companyId,
        int versionNumber)
    {
        // Fetch the two most recent versions: requested and previous
        var histories = await _dbContext.CompanyAdminUsersHistories
            .Where(a => a.CompanyId == companyId && (a.Version == versionNumber || a.Version == versionNumber - 1))
            .OrderByDescending(a => a.Version)
            .ToListAsync();

        Company5TabHistoryMapEntitytoDictionary company5TabHistoryMapEntitytoDictionary = new Company5TabHistoryMapEntitytoDictionary();

        var company5TabAdminHistory = new Company5TabHistoryDto();

        foreach (var history in histories)
        {
            var mapped = company5TabHistoryMapEntitytoDictionary.MapEntityToDictionary(history)
                .ToDictionary(
                    keyValue => keyValue.Key,
                    keyValue => (object)new Dictionary<string, string> { { keyValue.Key, keyValue.Value } }
                );
            company5TabAdminHistory.Company5TabHistory[history.Version.ToString()] = mapped;
        }

        return company5TabAdminHistory;
    }

}
