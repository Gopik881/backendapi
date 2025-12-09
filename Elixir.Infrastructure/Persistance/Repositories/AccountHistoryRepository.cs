using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.Company5TabHistoryMapEntityToDictionary;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class AccountHistoryRepository : IAccountHistoryRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public AccountHistoryRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Pseudocode plan:
    // 1. Before inserting AccountHistory, check if the companyId exists in the Companies table.
    // 2. If not, return false or throw a meaningful exception.
    // 3. Only proceed to add AccountHistory if the company exists.

    public async Task<bool> Company5TabCreateAccountDataAsync(int companyId, int userId, Company5TabAccountDto company5TabAccount, CancellationToken cancellationToken = default)
    {
        // Check if the company exists to avoid FK constraint violation
        bool companyExists = await _dbContext.Companies.AnyAsync(c => c.Id == companyId, cancellationToken);
        if (!companyExists)
        {
            // Optionally, you can throw an exception or return false
            return false;
        }

        int lastVersion = _dbContext.AccountHistories
            .Where(afh => afh.CompanyId == companyId && !afh.IsDeleted)
            .OrderByDescending(afh => afh.Version)
            .Select(afh => afh.Version)
            .FirstOrDefault();

        _dbContext.AccountHistories.Add(new AccountHistory
        {
            CompanyId = companyId,
            PerUserStorageMb = Convert.ToInt32(company5TabAccount.PerUserStorageMB),
            UserGroupLimit = Convert.ToInt32(company5TabAccount.UserGroupLimit),
            TempUserLimit = Convert.ToInt32(company5TabAccount.TempUserLimit),
            ContractName = company5TabAccount.ContractName,
            ContractId = company5TabAccount.ContractId,
            StartDate = company5TabAccount.StartDate,
            EndDate = company5TabAccount.EndDate,
            IsOpenEnded = company5TabAccount.OpenEnded,
            RenewalReminderDate = company5TabAccount.RenewalReminderDate,
            ContractNoticePeriod = Convert.ToInt32(company5TabAccount.ContractNoticePeriod),
            LicenseProcurement = company5TabAccount.LicenseProcurement,
            Pan = company5TabAccount.Pan,
            Tan = company5TabAccount.Tan,
            Gstin = company5TabAccount.Gstn,
            Version = lastVersion + 1,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId
        });

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> IsPanExistsAsync(string pan, int companyId)
    {
        return await _dbContext.AccountHistories
            .AnyAsync(ah => ah.Pan == pan && ah.CompanyId != companyId && !ah.IsDeleted);
    }
    public async Task<bool> IsTanExistsAsync(string tan, int companyId)
    {
        return await _dbContext.AccountHistories
            .AnyAsync(ah => ah.Tan == tan && ah.CompanyId != companyId && !ah.IsDeleted);
    }

    public async Task<bool> IsGstInExistsAsync(string gstIn, int companyId)
    {
        return await _dbContext.AccountHistories
            .AnyAsync(ah => ah.Gstin == gstIn && ah.CompanyId != companyId && !ah.IsDeleted);
    }

    public async Task<bool> IsContractIdExistsAsync(string contractId, int companyId)
    {
        return await _dbContext.AccountHistories
            .AnyAsync(ah => ah.ContractId == contractId && ah.CompanyId != companyId && !ah.IsDeleted);
    }
    public async Task<Company5TabAccountDto?> GetCompany5TabLatestAccountHistoryAsync(int companyId, bool isPrevious, CancellationToken cancellationToken = default)
    {
        var versionQuery = _dbContext.AccountHistories
            .Where(e => e.CompanyId == companyId && !e.IsDeleted);

        int latestVersion = isPrevious
            ? versionQuery.OrderByDescending(e => e.Version).Skip(1).Select(e => e.Version).FirstOrDefault()
            : (versionQuery.Max(e => (int?)e.Version) ?? 0);

        var accountHistory = await _dbContext.AccountHistories
            .Where(ah => ah.CompanyId == companyId && !ah.IsDeleted && ah.Version == latestVersion)
            .OrderByDescending(ah => ah.Version)
            .FirstOrDefaultAsync(cancellationToken);
        var companyStorageGB = await _dbContext.CompanyHistories.Where(cus => cus.CompanyId == companyId && cus.Version == latestVersion)
            .Select(cus => cus.CompanyStorageConsumedGb).FirstOrDefaultAsync(cancellationToken);

        if (accountHistory == null) return null;

        return new Company5TabAccountDto
        {
            CompanyStorageGB = companyStorageGB.HasValue ? (int?)Convert.ToInt32(companyStorageGB.Value) : 1,
            PerUserStorageMB = accountHistory.PerUserStorageMb.HasValue ? (int?)Convert.ToInt32(accountHistory.PerUserStorageMb.Value) : 1,
            UserGroupLimit = accountHistory.UserGroupLimit,
            TempUserLimit = accountHistory.TempUserLimit,
            ContractName = accountHistory.ContractName,
            ContractId = accountHistory.ContractId,
            StartDate = accountHistory.StartDate,
            EndDate = accountHistory.EndDate,
            OpenEnded = accountHistory.IsOpenEnded,
            RenewalReminderDate = accountHistory.RenewalReminderDate,
            ContractNoticePeriod = accountHistory.ContractNoticePeriod,
            LicenseProcurement = accountHistory.LicenseProcurement,
            Pan = accountHistory.Pan,
            Tan = accountHistory.Tan,
            Gstn = accountHistory.Gstin
        };
    }

    public async Task<bool> WithdrawCompany5TabAccountHistoryAsync(int companyId, int userId, CancellationToken cancellationToken = default)
    {
        // Get the latest version for the given company and user
        int? latestAccountInfoVersion = await _dbContext.AccountHistories
            .Where(e => e.CompanyId == companyId /*&& e.CreatedBy == userId*/)
            .MaxAsync(e => (int?)e.Version, cancellationToken);
        if (latestAccountInfoVersion == null) return true;
        // Find all records to remove
        var recordsToRemove = _dbContext.AccountHistories
            .Where(e => e.CompanyId == companyId /*&& e.CreatedBy == userId*/ && e.Version == latestAccountInfoVersion);
        if(!recordsToRemove.Any()) return true;
        _dbContext.AccountHistories.RemoveRange(recordsToRemove);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
    public async Task<Company5TabHistoryDto> GetCompany5TabAccountHistoryByVersionAsync(
        int userId,
        int companyId,
        int versionNumber)
    {
        // Fetch the two most recent versions: requested and previous
        var histories = await _dbContext.AccountHistories
            .Where(a => a.CompanyId == companyId && (a.Version == versionNumber || a.Version == versionNumber - 1))
            .OrderByDescending(a => a.Version)
            .ToListAsync();

        Company5TabHistoryMapEntitytoDictionary company5TabHistoryMapEntitytoDictionary = new Company5TabHistoryMapEntitytoDictionary();

        var company5TabAccountHistory = new Company5TabHistoryDto();

        foreach (var history in histories)
        {
            var mapped = company5TabHistoryMapEntitytoDictionary.MapEntityToDictionary(history)
                .ToDictionary(
                    keyValue => keyValue.Key,
                    keyValue => (object)new Dictionary<string, string> { { keyValue.Key, keyValue.Value } }
                );
            company5TabAccountHistory.Company5TabHistory[history.Version.ToString()] = mapped;
        }

        return company5TabAccountHistory;
    }

    

   


}
