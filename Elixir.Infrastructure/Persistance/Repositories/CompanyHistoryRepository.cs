using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Enums;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.Company5TabHistoryMapEntityToDictionary;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class CompanyHistoryRepository : ICompanyHistoryRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public CompanyHistoryRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    // Fix: Use CompanyId instead of Id for version calculation, so version increments per company
    public async Task<bool> Company5TabCreateCompanyDataAsync(int userId, Company5TabCompanyDto company5TabCompanyData, int companyStoregeGB, int peruserStorageMB, CancellationToken cancellationToken = default)
    {
        // Use CompanyId for versioning, not Id (which is PK)
        int lastVersion = _dbContext.CompanyHistories
            .Where(ch => ch.CompanyId == company5TabCompanyData.CompanyId && !ch.IsDeleted)
            .OrderByDescending(ch => ch.Version)
            .Select(ch => ch.Version)
            .FirstOrDefault();

        var createdAt = _dbContext.Companies
            .Where(cmp => cmp.Id == company5TabCompanyData.CompanyId && !cmp.IsDeleted)
            .Select(c => c.CreatedAt)
            .FirstOrDefault();

        bool IsBillingDataSame = company5TabCompanyData.SameAddressForBilling ?? false;

        _dbContext.CompanyHistories.Add(new CompanyHistory
        {
            CompanyStorageConsumedGb = companyStoregeGB,
            CompanyStorageTotalGb = peruserStorageMB,
            CompanyId = company5TabCompanyData.CompanyId,
            CompanyName = company5TabCompanyData.CompanyName,
            CompanyCode = company5TabCompanyData.CompanyCode,
            Address1 = company5TabCompanyData.Address1,
            Address2 = company5TabCompanyData.Address2,
            StateId = company5TabCompanyData.StateId,
            CountryId = company5TabCompanyData.CountryId,
            ZipCode = company5TabCompanyData.ZipCode,
            TelephoneCodeId = company5TabCompanyData.TelephoneCodeId,
            PhoneNumber = company5TabCompanyData.PhoneNumber,
            MfaEnabled = company5TabCompanyData.MultiFactor,
            MfaSms = company5TabCompanyData.IsSms,
            MfaEmail = company5TabCompanyData.IsEmail,
            BillingAddressSameAsCompany = company5TabCompanyData.SameAddressForBilling,
            LastUpdatedOn = DateTime.UtcNow,
            CreatedBy = userId,
            LastUpdatedBy = userId,
            CreatedAt = createdAt,
            Version = lastVersion + 1,
            UpdatedAt = DateTime.UtcNow,
            BillingAddress1 = IsBillingDataSame ? company5TabCompanyData.Address1 : company5TabCompanyData.BillingAddress1,
            BillingAddress2 = IsBillingDataSame ? company5TabCompanyData.Address2 : company5TabCompanyData.BillingAddress2,
            BillingStateId = IsBillingDataSame ? company5TabCompanyData.StateId : company5TabCompanyData.BillingStateId,
            BillingZipCode = IsBillingDataSame ? company5TabCompanyData.ZipCode : company5TabCompanyData.BillingZipCode,
            BillingCountryId = IsBillingDataSame ? company5TabCompanyData.CountryId : company5TabCompanyData.BillingCountryId,
            BillingTelephoneCodeId = IsBillingDataSame ? company5TabCompanyData.BillingTelePhoneCodeId : company5TabCompanyData.BillingTelePhoneCodeId,
            BillingPhoneNumber = IsBillingDataSame ? company5TabCompanyData.PhoneNumber : company5TabCompanyData.BillingPhoneNo,
        });
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<int> GetCompanyHistoryUserIdByCompanyId(int companyId)
    {
        var userIdFromCompanyHistory = _dbContext.CompanyHistories
                                                 .Where(e => e.CompanyId == companyId)
                                                 .OrderByDescending(e => e.Version)
                                                 .Select(e => e.CreatedBy)
                                                 .FirstOrDefault();
        return userIdFromCompanyHistory ?? 0; // Default to 0 if not found
    }
    public async Task<Company5TabCompanyDto?> GetCompany5TabLatestCompanyHistoryAsync(int companyId, int userId, bool isPrevious, CancellationToken cancellationToken = default)
    {

        var versionQuery = _dbContext.CompanyHistories
            .Where(e => e.CompanyId == companyId && !e.IsDeleted);

        int latestVersion = isPrevious
            ? versionQuery.OrderByDescending(e => e.Version).Skip(1).Select(e => e.Version).FirstOrDefault()
            : (versionQuery.Max(e => (int?)e.Version) ?? 0);


        var companyHistory = await _dbContext.CompanyHistories
            .Where(ch => ch.CompanyId == companyId && !ch.IsDeleted && ch.Version == latestVersion)
            .OrderByDescending(ch => ch.Version)
            .FirstOrDefaultAsync(cancellationToken);

        if (companyHistory == null)
            return null;
        var phoneShortCutCode = await _dbContext.TelephoneCodeMasters
            .Where(s => s.Id == companyHistory.TelephoneCodeId)
            .Select(s => s.TelephoneCode)
            .FirstOrDefaultAsync(cancellationToken);

        var stateName = await _dbContext.StateMasters
            .Where(s => s.Id == companyHistory.StateId)
            .Select(s => s.StateName)
            .FirstOrDefaultAsync(cancellationToken);

        var countryName = await _dbContext.CountryMasters
            .Where(c => c.Id == companyHistory.CountryId)
            .Select(c => c.CountryName)
            .FirstOrDefaultAsync(cancellationToken);

        var billingStateName = await _dbContext.StateMasters
            .Where(s => s.Id == companyHistory.BillingStateId)
            .Select(s => s.StateName)
            .FirstOrDefaultAsync(cancellationToken);
        
        var billingStateId = await _dbContext.StateMasters
           .Where(s => s.Id == companyHistory.BillingStateId)
           .Select(s => s.Id)
           .FirstOrDefaultAsync(cancellationToken);

        var billingCountryName = await _dbContext.CountryMasters
            .Where(c => c.Id == companyHistory.BillingCountryId)
            .Select(c => c.CountryName)
            .FirstOrDefaultAsync(cancellationToken);

        var billingCountryId = await _dbContext.CountryMasters
            .Where(c => c.Id == companyHistory.BillingCountryId)
            .Select(c => c.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var billingShortCutCode = await _dbContext.TelephoneCodeMasters
            .Where(s => s.Id == companyHistory.BillingTelephoneCodeId)
            .Select(s => s.TelephoneCode)
            .FirstOrDefaultAsync(cancellationToken);

        // Get the logged-in userId from the context (assuming you have a way to get it, e.g., from a service or thread context)
        // For this example, let's assume you pass it as a parameter. If not, replace 'loggedInUserId' with your actual logic.
        int? loggedInUserId = userId; // Replace with actual logged-in userId if available

        return new Company5TabCompanyDto
        {
            CompanyId = companyHistory.CompanyId ?? 0,
            CompanyStorageGB = (int?)companyHistory.CompanyStorageTotalGb,
            PerUserStorageMB = (int?)companyHistory.CompanyStorageConsumedGb,
            CompanyName = companyHistory.CompanyName,
            CompanyCode = companyHistory.CompanyCode,
            Address1 = companyHistory.Address1,
            Address2 = companyHistory.Address2,
            StateId = (int)companyHistory.StateId,
            StateName = stateName,
            CountryId = (int)companyHistory.CountryId,
            CountryName = countryName,
            ZipCode = companyHistory.ZipCode,
            PhoneShortCutCode = phoneShortCutCode,
            TelephoneCodeId = companyHistory.TelephoneCodeId,
            PhoneNumber = companyHistory.PhoneNumber,
            MultiFactor = companyHistory.MfaEnabled,
            IsSms = companyHistory.MfaSms,
            IsEmail = companyHistory.MfaEmail,
            SameAddressForBilling = companyHistory.BillingAddressSameAsCompany,
            IsActive = companyHistory.IsEnabled,
            BillingAddress1 = companyHistory.BillingAddress1,
            BillingAddress2 = companyHistory.BillingAddress2,
            BillingStateId = billingStateId,//companyHistory.StateId,
            BillingStateName = billingStateName,
            BillingZipCode = companyHistory.BillingZipCode,
            BillingCountryId = billingCountryId,//(int)companyHistory.CountryId,
            BillingCountryName = billingCountryName,
            BillingTelePhoneCodeId = companyHistory.BillingTelephoneCodeId,
            BillingPhoneShortCutCode = billingShortCutCode,
            BillingPhoneNo = companyHistory.BillingPhoneNumber,
            CreatedBy = companyHistory.CreatedBy,
            IsOriginalAccountManager = (loggedInUserId.HasValue && companyHistory.CreatedBy.HasValue && loggedInUserId.Value == companyHistory.CreatedBy.Value)
        };
    }
    public async Task<bool> WithdrawCompany5TabCompanyHistoryAsync(int companyId, int userId, CancellationToken cancellationToken = default)
    {
        // Get the latest version for the given company and user
        int? latestCompanyHistoryVersion = await _dbContext.CompanyHistories
            .Where(e => e.CompanyId == companyId /*&& e.CreatedBy == userId*/)
            .MaxAsync(e => (int?)e.Version, cancellationToken);

        if (latestCompanyHistoryVersion == null) return true;

        // Find all records to remove
        var recordsToRemove = _dbContext.CompanyHistories
            .Where(e => e.CompanyId == companyId /*&& e.CreatedBy == userId*/ && e.Version == latestCompanyHistoryVersion);
        if (recordsToRemove.Count() == 0) return true;

        _dbContext.CompanyHistories.RemoveRange(recordsToRemove);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
    //public async Task<Company5TabHistoryDto> GetCompany5TabCompanyHistoryByVersionAsync(int userId, int companyId, int versionNumber)
    //{
    //    // Find the closest lower or equal version for the "old" value
    //    var oldVersion = await _dbContext.CompanyHistories
    //        .Where(ch => ch.CompanyId == companyId && ch.Version <= versionNumber - 1)
    //        .OrderByDescending(ch => ch.Version)
    //        .FirstOrDefaultAsync();

    //    // Find the closest lower or equal version for the "new" value
    //    var newVersion = await _dbContext.CompanyHistories
    //        .Where(ch => ch.CompanyId == companyId && ch.Version <= versionNumber)
    //        .OrderByDescending(ch => ch.Version)
    //        .FirstOrDefaultAsync();

    //    Company5TabHistoryMapEntitytoDictionary company5TabHistoryMapEntitytoDictionary = new Company5TabHistoryMapEntitytoDictionary();
    //    var company5TabCompanyHistory = new Company5TabHistoryDto();

    //    var oldMapped = oldVersion != null
    //        ? company5TabHistoryMapEntitytoDictionary.MapEntityToDictionary(oldVersion)
    //        : new Dictionary<string, string>();

    //    var newMapped = newVersion != null
    //        ? company5TabHistoryMapEntitytoDictionary.MapEntityToDictionary(newVersion)
    //        : new Dictionary<string, string>();

    //    // Only include keys where the value has changed between old and new
    //    var changedKeys = newMapped.Keys
    //        .Where(key => !oldMapped.ContainsKey(key) || oldMapped[key] != newMapped[key])
    //        .ToList();

    //    var filteredOld = oldMapped
    //        .Where(kv => changedKeys.Contains(kv.Key))
    //        .ToDictionary(
    //            kv => kv.Key,
    //            kv => (object)new Dictionary<string, string> { { kv.Key, kv.Value } }
    //        );

    //    var filteredNew = newMapped
    //        .Where(kv => changedKeys.Contains(kv.Key))
    //        .ToDictionary(
    //            kv => kv.Key,
    //            kv => (object)new Dictionary<string, string> { { kv.Key, kv.Value } }
    //        );

    //    company5TabCompanyHistory.Company5TabHistory["OldValue"] = filteredOld;
    //    company5TabCompanyHistory.Company5TabHistory["NewValue"] = filteredNew;

    //    return company5TabCompanyHistory;
    //}
    //public async Task<Company5TabHistoryDto> GetCompany5TabCompanyHistoryByVersionAsync(int companyId, int versionNumber, CancellationToken cancellationToken = default)
    //{
    //    // Find the closest lower or equal version for the "old" value
    //    var oldVersion = await _dbContext.CompanyHistories
    //        .Where(ch => ch.CompanyId == companyId && ch.Version <= versionNumber - 1 && !ch.IsDeleted)
    //        .OrderByDescending(ch => ch.Version)
    //        .FirstOrDefaultAsync(cancellationToken);

    //    // Find the closest lower or equal version for the "new" value
    //    var newVersion = await _dbContext.CompanyHistories
    //        .Where(ch => ch.CompanyId == companyId && ch.Version <= versionNumber && !ch.IsDeleted)
    //        .OrderByDescending(ch => ch.Version)
    //        .FirstOrDefaultAsync(cancellationToken);

    //    Company5TabHistoryMapEntitytoDictionary company5TabHistoryMapEntitytoDictionary = new Company5TabHistoryMapEntitytoDictionary();
    //    var company5TabCompanyHistory = new Company5TabHistoryDto();

    //    var oldMapped = oldVersion != null
    //        ? company5TabHistoryMapEntitytoDictionary.MapEntityToDictionary(oldVersion)
    //        : new Dictionary<string, string>();

    //    var newMapped = newVersion != null
    //        ? company5TabHistoryMapEntitytoDictionary.MapEntityToDictionary(newVersion)
    //        : new Dictionary<string, string>();

    //    // Only include keys where the value has changed between old and new
    //    var changedKeys = newMapped.Keys
    //        .Where(key => !oldMapped.ContainsKey(key) || oldMapped[key] != newMapped[key])
    //        .ToList();

    //    var filteredOld = oldMapped
    //        .Where(kv => changedKeys.Contains(kv.Key))
    //        .ToDictionary(
    //            kv => kv.Key,
    //            kv => (object)new Dictionary<string, string> { { kv.Key, kv.Value } }
    //        );

    //    var filteredNew = newMapped
    //        .Where(kv => changedKeys.Contains(kv.Key))
    //        .ToDictionary(
    //            kv => kv.Key,
    //            kv => (object)new Dictionary<string, string> { { kv.Key, kv.Value } }
    //        );

    //    company5TabCompanyHistory.Company5TabHistory["OldValue"] = filteredOld;
    //    company5TabCompanyHistory.Company5TabHistory["NewValue"] = filteredNew;
    //    company5TabCompanyHistory.Company5TabHistory["OldVersion"] = oldVersion?.Version;
    //    company5TabCompanyHistory.Company5TabHistory["NewVersion"] = newVersion?.Version;

    //    return company5TabCompanyHistory;
    //}
    // Fix for CS0266: Explicitly cast nullable int to int where necessary
    // Fix for CS1662: Ensure lambda expressions return the correct type
    // Fix for CS8629: Handle nullable value types to avoid potential null issues

    //public async Task<Company5TabHistoryDto> GetCompany5TabCompanyHistoryByVersionAsync(int userId, int companyId, int versionNumber)
    //{
    //    var result = new Dictionary<string, object>();

    //    // Helper to get the two versions based on the versionNumber logic
    //    async Task<int[]> GetTargetVersionsAsync<T>(IQueryable<T> query, Func<T, int?> getVersion)
    //    {
    //        var versions = await query.ToListAsync();
    //        var orderedVersions = versions
    //            .Select(getVersion)
    //            .Where(v => v.HasValue)
    //            .Select(v => v.Value)
    //            .Distinct()
    //            .OrderByDescending(v => v)
    //            .ToList();

    //        // If versionNumber is 1, take last and last-1; if 2, take last-1 and last-2, etc.
    //        if (orderedVersions.Count < 2 || versionNumber < 1 || versionNumber > orderedVersions.Count)
    //            return Array.Empty<int>();

    //        int idx = versionNumber - 1;
    //        if (idx + 1 >= orderedVersions.Count)
    //            return Array.Empty<int>();

    //        return new[] { orderedVersions[idx + 1], orderedVersions[idx] }.OrderBy(v => v).ToArray();
    //    }

    //    bool ShouldExclude(string propertyName)
    //    {
    //        var patterns = new[] { "version", "id", "created", "updated", "lastupdated", "groupid", "userid" };
    //        return patterns.Any(p => propertyName.ToLower().Contains(p));
    //    }

    //    Dictionary<string, string> MapEntityToFilteredDictionary<T>(T entity)
    //    {
    //        var dict = new Dictionary<string, string>();
    //        foreach (var prop in typeof(T).GetProperties())
    //        {
    //            if (!ShouldExclude(prop.Name))
    //            {
    //                dict[prop.Name] = prop.GetValue(entity)?.ToString() ?? "";
    //            }
    //        }
    //        return dict;
    //    }

    //    async Task<Dictionary<string, object>> GetHistoryDiffAsync<T>(IQueryable<T> query, Func<T, int?> getVersion)
    //    {
    //        int[] versions = await GetTargetVersionsAsync(query, getVersion);
    //        if (versions.Length != 2)
    //            return new Dictionary<string, object> { { "Differences", new Dictionary<string, object>() } };

    //        var entities = (await query.ToListAsync())
    //            .Where(e => versions.Contains(getVersion(e) ?? -1))
    //            .ToList();
    //        var oldEntity = entities.FirstOrDefault(e => getVersion(e) == versions[0]);
    //        var newEntity = entities.FirstOrDefault(e => getVersion(e) == versions[1]);

    //        var oldDict = oldEntity != null ? MapEntityToFilteredDictionary(oldEntity) : new Dictionary<string, string>();
    //        var newDict = newEntity != null ? MapEntityToFilteredDictionary(newEntity) : new Dictionary<string, string>();

    //        var differences = new Dictionary<string, object>();
    //        foreach (var key in newDict.Keys.Union(oldDict.Keys))
    //        {
    //            oldDict.TryGetValue(key, out var oldVal);
    //            newDict.TryGetValue(key, out var newVal);
    //            if (oldVal != newVal)
    //            {
    //                differences[key] = new
    //                {
    //                    oldValue = oldVal,
    //                    newValue = newVal
    //                };
    //            }
    //        }

    //        return new Dictionary<string, object> { { "Differences", differences } };
    //    }

    //    result["accountInfo"] = await GetHistoryDiffAsync(
    //        _dbContext.AccountHistories.Where(a => a.CompanyId == companyId),
    //        a => (a.GetType().GetProperty("Version")?.GetValue(a) as int?)
    //    );
    //    result["elixirUser"] = await GetHistoryDiffAsync(
    //        _dbContext.CompanyAdminUsersHistories.Where(e => e.CompanyId == companyId),
    //        e => (e.GetType().GetProperty("Version")?.GetValue(e) as int?)
    //    );
    //    result["companyInfo"] = await GetHistoryDiffAsync(
    //        _dbContext.CompanyHistories.Where(c => c.CompanyId == companyId),
    //        c => (c.GetType().GetProperty("Version")?.GetValue(c) as int?)
    //    );
    //    result["companyUser"] = await GetHistoryDiffAsync(
    //        _dbContext.ElixirUsersHistories.Where(u => u.CompanyId == companyId),
    //        u => (u.GetType().GetProperty("Version")?.GetValue(u) as int?)
    //    );
    //    result["moduleMapping"] = await GetHistoryDiffAsync(
    //        _dbContext.ModuleMappingHistories.Where(m => m.CompanyId == companyId),
    //        m => (m.GetType().GetProperty("Version")?.GetValue(m) as int?)
    //    );

    //    return new Company5TabHistoryDto
    //    {
    //        Company5TabHistory = result
    //    };
    //}

    //// Example MapToKeyValuePairs implementation (adjust as needed)
    //private Dictionary<string, Dictionary<string, Dictionary<string, string>>> MapToKeyValuePairs<T>(List<T> entities)
    //{
    //    var result = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
    //    foreach (var entity in entities)
    //    {
    //        var versionProp = entity.GetType().GetProperty("Version");
    //        string version = versionProp?.GetValue(entity)?.ToString() ?? "Unknown";
    //        var dict = new Dictionary<string, string>();
    //        foreach (var prop in typeof(T).GetProperties())
    //        {
    //            dict[prop.Name] = prop.GetValue(entity)?.ToString() ?? "";
    //        }
    //        result[version] = new Dictionary<string, Dictionary<string, string>>
    //        {
    //            { "Data", dict }
    //        };
    //    }
    //    return result;
    //}



    public async Task<object> GetCompany5TabCompanyHistoryByVersionJsonAsync(int userId, int companyId, int versionNumber)
    {
        //versionNumber = versionNumber + 1;
        var IncludedProperties = AppConstants.IncludedProperties;

        static Dictionary<string, object> Compare<T>(T? current, T? previous, string[] includedProps, bool allowNewOnly = false) where T : class
        {
            var differences = new Dictionary<string, object>();
            if (current == null && previous == null) return differences;
            foreach (var prop in typeof(T).GetProperties().Where(p => includedProps.Contains(p.Name)))
            {
                var currentVal = current != null ? prop.GetValue(current) : null;
                var prevVal = previous != null ? prop.GetValue(previous) : null;
                if ((currentVal == null && prevVal == null) || object.Equals(currentVal, prevVal)) continue;
                // Only consider if both old and new values are present, unless allowNewOnly is true
                if (allowNewOnly)
                {
                    if (currentVal != null || prevVal != null)
                        differences[prop.Name] = new { oldValue = prevVal, newValue = currentVal };
                }
                else
                {
                    if (currentVal != null && prevVal != null)
                        differences[prop.Name] = new { oldValue = prevVal, newValue = currentVal };
                }
            }
            return differences;
        }

        // companyInfo
        //var companyCurrent = await _dbContext.CompanyHistories
        //    .Where(c => c.CompanyId == companyId && !c.IsDeleted && c.Version == versionNumber)
        //    .FirstOrDefaultAsync();
        //if (companyCurrent == null)
        //{
        // Get the latest version for the company

        //companyInfo
        var latestVersion = await _dbContext.CompanyHistories
          .Where(c => c.CompanyId == companyId && !c.IsDeleted)
          .OrderByDescending(c => c.Version)
          .Select(c => c.Version)
          .FirstOrDefaultAsync();

        int companyCurrentVersion = versionNumber;
        CompanyHistory? companyCurrent = null;

        if (latestVersion != 0)
        {
            companyCurrent = await _dbContext.CompanyHistories
                .Where(c => c.CompanyId == companyId && !c.IsDeleted && c.Version == versionNumber)
                .Select(ch => new CompanyHistory
                {
                    CompanyName = ch.CompanyName,
                    CompanyCode = ch.CompanyCode,
                    Address1 = ch.Address1,
                    Address2 = ch.Address2,
                    StateId = ch.StateId,
                    CountryId = ch.CountryId,
                    ZipCode = ch.ZipCode,
                    TelephoneCodeId = ch.TelephoneCodeId,
                    PhoneNumber = ch.PhoneNumber,
                    MfaEnabled = ch.MfaEnabled,
                    MfaSms = ch.MfaSms,
                    MfaEmail = ch.MfaEmail,
                    BillingAddressSameAsCompany = ch.BillingAddressSameAsCompany,
                    IsEnabled = ch.IsEnabled,
                    BillingAddress1 = ch.BillingAddress1,
                    BillingAddress2 = ch.BillingAddress2,
                    BillingStateId = ch.BillingStateId,
                    BillingZipCode = ch.BillingZipCode,
                    BillingCountryId = ch.BillingCountryId,
                    BillingTelephoneCodeId = ch.BillingTelephoneCodeId,
                    BillingPhoneNumber = ch.BillingPhoneNumber,
                    Version = ch.Version
                })
                .FirstOrDefaultAsync();
            companyCurrentVersion = companyCurrent?.Version ?? versionNumber;
        }

        // Fix: Move .OrderByDescending(c => c.Version) before .Select(...) to ensure EF Core can translate the query
        var companyPrevious = await _dbContext.CompanyHistories
            .Where(c => c.CompanyId == companyId && !c.IsDeleted && c.Version < companyCurrentVersion)
            .OrderByDescending(c => c.Version)
            .Select(ch => new CompanyHistory
            {
                CompanyName = ch.CompanyName,
                CompanyCode = ch.CompanyCode,
                Address1 = ch.Address1,
                Address2 = ch.Address2,
                StateId = ch.StateId,
                CountryId = ch.CountryId,
                ZipCode = ch.ZipCode,
                TelephoneCodeId = ch.TelephoneCodeId,
                PhoneNumber = ch.PhoneNumber,
                MfaEnabled = ch.MfaEnabled,
                MfaSms = ch.MfaSms,
                MfaEmail = ch.MfaEmail,
                BillingAddressSameAsCompany = ch.BillingAddressSameAsCompany,
                IsEnabled = ch.IsEnabled,
                BillingAddress1 = ch.BillingAddress1,
                BillingAddress2 = ch.BillingAddress2,
                BillingStateId = ch.BillingStateId,
                BillingZipCode = ch.BillingZipCode,
                BillingCountryId = ch.BillingCountryId,
                BillingTelephoneCodeId = ch.BillingTelephoneCodeId,
                BillingPhoneNumber = ch.BillingPhoneNumber
            })
            .FirstOrDefaultAsync();

        var companyInfo = Compare(companyCurrent, companyPrevious, IncludedProperties["companyInfo"], false);
        var keysToRemove = new List<string>();
        var keysToAdd = new Dictionary<string, object>();
        foreach (var key in companyInfo.Keys.ToList()) // use ToList to avoid modifying the collection during iteration
        {
            var value = companyInfo[key];
            if (value is { } && value.GetType().GetProperty("oldValue") != null)
            {
                var oldVal = value.GetType().GetProperty("oldValue")?.GetValue(value);
                var newVal = value.GetType().GetProperty("newValue")?.GetValue(value);

                // Handle StateId
                if (key == "StateId" || key == "BillingStateId")
                {
                    var oldStateName = oldVal != null
                        ? _dbContext.StateMasters.FirstOrDefault(s => s.Id == (int)oldVal)?.StateName
                        : null;
                    var newStateName = newVal != null
                        ? _dbContext.StateMasters.FirstOrDefault(s => s.Id == (int)newVal)?.StateName
                        : null;

                    companyInfo[key] = new { oldValue = oldStateName, newValue = newStateName };
                    var newKey = key == "StateId" ? "State" : "BillingState";
                    keysToRemove.Add(key);
                    keysToAdd[newKey] = new { oldValue = oldStateName, newValue = newStateName };
                }

                // Handle CountryId
                if (key == "CountryId" || key == "BillingCountryId")
                {
                    var oldCountryName = oldVal != null
                        ? _dbContext.CountryMasters.FirstOrDefault(c => c.Id == (int)oldVal)?.CountryName
                        : null;
                    var newCountryName = newVal != null
                        ? _dbContext.CountryMasters.FirstOrDefault(c => c.Id == (int)newVal)?.CountryName
                        : null;

                    companyInfo[key] = new { oldValue = oldCountryName, newValue = newCountryName };
                    var newKey = key == "CountryId" ? "Country" : "BillingCountry";
                    keysToRemove.Add(key);
                    keysToAdd[newKey] = new { oldValue = oldCountryName, newValue = newCountryName };
                }
                // Handle TelephoneCodeId
                if (key == "TelephoneCodeId" || key == "BillingTelephoneCodeId")
                {
                    var oldCodeName = oldVal != null
                        ? _dbContext.TelephoneCodeMasters.FirstOrDefault(c => c.Id == (int)oldVal)?.TelephoneCode
                        : null;
                    var newCodeName = newVal != null
                        ? _dbContext.TelephoneCodeMasters.FirstOrDefault(c => c.Id == (int)newVal)?.TelephoneCode
                        : null;

                    companyInfo[key] = new { oldValue = oldCodeName, newValue = newCodeName };
                    var newKey = key == "TelephoneCodeId" ? "TelephoneCode" : "BillingTelephoneCode";
                    keysToRemove.Add(key);
                    keysToAdd[newKey] = new { oldValue = oldCodeName, newValue = newCodeName };
                }
            }
        }
        // Final dictionary modifications after loop
        foreach (var key in keysToRemove)
        {
            companyInfo.Remove(key);
        }

        foreach (var kvp in keysToAdd)
        {
            companyInfo[kvp.Key] = kvp.Value;
        }
        // accountInfo
        //var accountCurrent = await _dbContext.AccountHistories
        //    .Where(a => a.CompanyId == companyId && !a.IsDeleted && a.Version == companyCurrentVersion)
        //    .FirstOrDefaultAsync();
        //if (accountCurrent == null)
        //{
        // Get latest account history version for the company
        var latestAccountVersion = await _dbContext.AccountHistories
            .Where(a => a.CompanyId == companyId && !a.IsDeleted)
            .OrderByDescending(a => a.Version)
            .Select(a => a.Version)
            .FirstOrDefaultAsync();

        HistoryTabAccountInfoDto? accountCurrent = null;
        int accountCurrentVersion = companyCurrentVersion;

        if (latestAccountVersion != 0)
        {
            accountCurrent = await _dbContext.AccountHistories
                .Where(a => a.CompanyId == companyId && !a.IsDeleted && a.Version == versionNumber)
                .Join(
                    _dbContext.CompanyHistories.Where(ch => ch.CompanyId == companyId && !ch.IsDeleted && ch.Version == versionNumber),
                    a => a.CompanyId,
                    ch => ch.CompanyId,
                    (a, ch) => new HistoryTabAccountInfoDto
                    {
                        CompanyStorageConsumedGb = ch.CompanyStorageConsumedGb,
                        CompanyStorageTotalGb = ch.CompanyStorageTotalGb,
                        PerUserStorageMb = a.PerUserStorageMb,
                        UserGroupLimit = a.UserGroupLimit,
                        TempUserLimit = a.TempUserLimit,
                        ContractName = a.ContractName,
                        ContractId = a.ContractId,
                        StartDate = a.StartDate,
                        EndDate = a.EndDate,
                        IsOpenEnded = a.IsOpenEnded,
                        RenewalReminderDate = a.RenewalReminderDate,
                        ContractNoticePeriod = a.ContractNoticePeriod,
                        LicenseProcurement = a.LicenseProcurement,
                        Pan = a.Pan,
                        Tan = a.Tan,
                        Gstin = a.Gstin,
                        Version = a.Version
                    }
                )
                .FirstOrDefaultAsync();
            accountCurrentVersion = accountCurrent?.Version ?? companyCurrentVersion;
        }

        // Step 1: Get the previous version from AccountHistories
        var previousAccountVersion = await _dbContext.AccountHistories
            .Where(a => a.CompanyId == companyId && !a.IsDeleted && a.Version < accountCurrentVersion)
            .MaxAsync(a => (int?)a.Version); // Nullable to avoid exception if none found

        if (previousAccountVersion == null)
            return null;

        // Step 2: Get matching AccountHistory and CompanyHistory
        var accountPrevious = await (
            from a in _dbContext.AccountHistories
            from ch in _dbContext.CompanyHistories
            where a.CompanyId == companyId
                  && a.Version == previousAccountVersion
                  && ch.CompanyId == companyId
                  && ch.Version == previousAccountVersion
                  && !a.IsDeleted
                  && !ch.IsDeleted
            select new HistoryTabAccountInfoDto
            {
                CompanyStorageConsumedGb = ch.CompanyStorageConsumedGb,
                CompanyStorageTotalGb = ch.CompanyStorageTotalGb,
                PerUserStorageMb = a.PerUserStorageMb,
                UserGroupLimit = a.UserGroupLimit,
                TempUserLimit = a.TempUserLimit,
                ContractName = a.ContractName,
                ContractId = a.ContractId,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                IsOpenEnded = a.IsOpenEnded,
                RenewalReminderDate = a.RenewalReminderDate,
                ContractNoticePeriod = a.ContractNoticePeriod,
                LicenseProcurement = a.LicenseProcurement,
                Pan = a.Pan,
                Tan = a.Tan,
                Gstin = a.Gstin,
                Version = a.Version
            }
        ).FirstOrDefaultAsync();

        var accountInfo = Compare(accountCurrent, accountPrevious, IncludedProperties["accountInfo"], false);

        // ReportingToolHistories under accountInfo (merge into accountInfo dictionary)
        var reportingCurrent = await _dbContext.ReportingToolLimitsHistories
            .Where(r => r.CompanyId == companyId && !r.IsDeleted && r.Version == versionNumber)
            .FirstOrDefaultAsync();
        if (reportingCurrent == null)
        {
            var latestReportingVersion = await _dbContext.ReportingToolLimitsHistories
                .Where(r => r.CompanyId == companyId && !r.IsDeleted)
                .OrderByDescending(r => r.Version)
                .Select(r => r.Version)
                .FirstOrDefaultAsync();
            if (latestReportingVersion != 0)
                reportingCurrent = await _dbContext.ReportingToolLimitsHistories
                    .Where(r => r.CompanyId == companyId && !r.IsDeleted && r.Version == latestReportingVersion)
                    .FirstOrDefaultAsync();
        }
        int reportingCurrentVersion = reportingCurrent?.Version ?? accountCurrentVersion;
        var reportingPrevious = await _dbContext.ReportingToolLimitsHistories
            .Where(r => r.CompanyId == companyId && !r.IsDeleted && r.Version < reportingCurrentVersion)
            .OrderByDescending(r => r.Version)
            .FirstOrDefaultAsync();
        var reportingToolInfo = Compare(reportingCurrent, reportingPrevious, IncludedProperties["reportingToolInfo"], false);

        // Merge reportingToolInfo into accountInfo
        foreach (var kv in reportingToolInfo)
        {
            // If key already exists in accountInfo, append "_ReportingTool" to avoid collision
            var key = kv.Key;
            if (accountInfo.ContainsKey(key))
                key = key + "_ReportingTool";
            accountInfo[key] = kv.Value;
        }

        // ElixirUser: EmailId and PhoneNumber diff (skip duplicates in old and new)
        var elixirCurrentList = await _dbContext.ElixirUsersHistories
            .Where(e => e.CompanyId == companyId && e.RoleId != (int)Roles.SuperAdmin && !e.IsDeleted && e.Version == versionNumber)
            .ToListAsync();
        //if (elixirCurrentList.Count == 0)
        //{
        //    var latestElixirVersion = await _dbContext.ElixirUsersHistories
        //        .Where(e => e.CompanyId == companyId && e.RoleId != (int)Roles.SuperAdmin && !e.IsDeleted)
        //        .OrderByDescending(e => e.Version)
        //        .Select(e => e.Version)
        //        .FirstOrDefaultAsync();
        //    if (latestElixirVersion != 0)
        //    {
        //        elixirCurrentList = await _dbContext.ElixirUsersHistories
        //            .Where(e => e.CompanyId == companyId && e.RoleId != (int)Roles.SuperAdmin && !e.IsDeleted && e.Version == latestElixirVersion)
        //            .ToListAsync();
        //    }
        //}
        int elixirCurrentVersion = elixirCurrentList.FirstOrDefault()?.Version ?? companyCurrentVersion;
        var prevElixirVersion = await _dbContext.ElixirUsersHistories
            .Where(e => e.CompanyId == companyId  && !e.IsDeleted && e.Version < versionNumber)
            .OrderByDescending(e => e.Version)
            .Select(e => e.Version)
            .FirstOrDefaultAsync();
        var elixirPreviousList = prevElixirVersion != 0
            ? await _dbContext.ElixirUsersHistories
                .Where(e => e.CompanyId == companyId && e.RoleId != (int)Roles.SuperAdmin &&  !e.IsDeleted && e.Version == prevElixirVersion)
                .ToListAsync()
            : new List<ElixirUsersHistory>();

        var userIds = elixirCurrentList.Select(e => e.UserId)
            .Concat(elixirPreviousList.Select(e => e.UserId))
            .Distinct()
            .ToList();
        var userMap = userIds.Count > 0
            ? (await _dbContext.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.Email, u.LastName, u.TelephoneCodeId, u.PhoneNumber })
                .ToListAsync())
                .ToDictionary(u => u.Id, u => (dynamic)u)
            : new Dictionary<int, dynamic>();

        var emailOld = elixirPreviousList
            .Select(e => userMap.TryGetValue(e.UserId, out var u) ? $"{u.Email}" : null)
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct()
            .ToList();
        var emailNew = elixirCurrentList
            .Select(e => userMap.TryGetValue(e.UserId, out var u) ? $"{u.Email}" : null)
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct()
            .ToList();
        var phoneOld = elixirPreviousList
            .Select(e => userMap.TryGetValue(e.UserId, out var u) && u.TelephoneCodeId != null && !string.IsNullOrEmpty(u.PhoneNumber)
                ? $"+{u.TelephoneCodeId} {u.PhoneNumber}" : null)
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct()
            .ToList();
        var phoneNew = elixirCurrentList
            .Select(e => userMap.TryGetValue(e.UserId, out var u) && u.TelephoneCodeId != null && !string.IsNullOrEmpty(u.PhoneNumber)
                ? $"+{u.TelephoneCodeId} {u.PhoneNumber}" : null)
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct()
            .ToList();

        // Remove values that are present in both old and new
        // Instead of filtering out values present in both old and new, show all old and new values if there is any change
        var elixirUser = new List<Dictionary<string, object>>();
        bool emailChanged = !emailOld.SequenceEqual(emailNew);
        bool phoneChanged = !phoneOld.SequenceEqual(phoneNew);

        //if (emailChanged && (emailOld.Count > 0 || emailNew.Count > 0))
        //    elixirUser.Add(new Dictionary<string, object> { { "EmailId", new { OldValue = emailOld, NewValue = emailNew } } });
        //if (phoneChanged && (phoneOld.Count > 0 || phoneNew.Count > 0))
        //    elixirUser.Add(new Dictionary<string, object> { { "PhoneNumber", new { OldValue = phoneOld, NewValue = phoneNew } } });
        // Show all old and new values if there is any change (do not remove same values)
        elixirUser = new List<Dictionary<string, object>>();
        if (emailOld.Count > 0 || emailNew.Count > 0)
        {
            // If both lists have the same count and same values (order-insensitive), skip adding
            var emailOldSorted = emailOld.OrderBy(e => e).ToList();
            var emailNewSorted = emailNew.OrderBy(e => e).ToList();
            if (!(emailOldSorted.Count == emailNewSorted.Count && emailOldSorted.SequenceEqual(emailNewSorted)))
            {
                elixirUser.Add(new Dictionary<string, object> { { "EmailId", new { OldValue = emailOld, NewValue = emailNew } } });
            }
        }
        //if (phoneOld.Count > 0 || phoneNew.Count > 0)
        //    elixirUser.Add(new Dictionary<string, object> { { "PhoneNumber", new { OldValue = phoneOld, NewValue = phoneNew } } });
        //var emailOldFiltered = emailOld.Except(emailNew).ToList();
        //var emailNewFiltered = emailNew.Except(emailOld).ToList();
        //var phoneOldFiltered = phoneOld.Except(phoneNew).ToList();
        //var phoneNewFiltered = phoneNew.Except(phoneOld).ToList();

        //elixirUser = new List<Dictionary<string, object>>();
        //if (emailOldFiltered.Count > 0 || emailNewFiltered.Count > 0)
        //    elixirUser.Add(new Dictionary<string, object> { { "EmailId", new { OldValue = emailOldFiltered, NewValue = emailNewFiltered } } });
        //if (phoneOldFiltered.Count > 0 || phoneNewFiltered.Count > 0)
        //    elixirUser.Add(new Dictionary<string, object> { { "PhoneNumber", new { OldValue = phoneOldFiltered, NewValue = phoneNewFiltered } } });

        // ModuleMapping (must have both old and new)
        var moduleCurrentList = await _dbContext.ModuleMappingHistories
            .Where(m => m.CompanyId == companyId && !m.IsDeleted && m.Version == versionNumber && m.SubModuleId.HasValue)
            .Join(_dbContext.Modules, mm => mm.ModuleId, m => m.Id, (mm, m) => new { mm, m })
            .Join(_dbContext.SubModules, temp => temp.mm.SubModuleId, sm => sm.Id, (temp, sm) => new { temp.m.ModuleName, sm.SubModuleName })
            .ToListAsync();

        if (moduleCurrentList.Count == 0)
        {
            var latestModuleVersion = await _dbContext.ModuleMappingHistories
                .Where(m => m.CompanyId == companyId && !m.IsDeleted && m.SubModuleId.HasValue)
                .OrderByDescending(m => m.Version)
                .Select(m => m.Version)
                .FirstOrDefaultAsync();
            if (latestModuleVersion != 0)
            {
                moduleCurrentList = await _dbContext.ModuleMappingHistories
                    .Where(m => m.CompanyId == companyId && !m.IsDeleted && m.Version == latestModuleVersion && m.SubModuleId.HasValue)
                    .Join(_dbContext.Modules, mm => mm.ModuleId, m => m.Id, (mm, m) => new { mm, m })
                    .Join(_dbContext.SubModules, temp => temp.mm.SubModuleId, sm => sm.Id, (temp, sm) => new { temp.m.ModuleName, sm.SubModuleName })
                    .ToListAsync();
            }
        }

        var prevModuleVersion = await _dbContext.ModuleMappingHistories
            .Where(m => m.CompanyId == companyId && !m.IsDeleted && m.Version < versionNumber && m.SubModuleId.HasValue)
            .OrderByDescending(m => m.Version)
            .Select(m => m.Version)
            .FirstOrDefaultAsync();

        List<(string ModuleName, string SubModuleName)> modulePreviousList;
        if (prevModuleVersion != 0)
        {
            modulePreviousList = await (
                from mm in _dbContext.ModuleMappingHistories
                join m in _dbContext.Modules on mm.ModuleId equals m.Id
                join sm in _dbContext.SubModules on mm.SubModuleId equals sm.Id
                where mm.CompanyId == companyId
                    && !mm.IsDeleted
                    && mm.Version == prevModuleVersion
                    && mm.SubModuleId.HasValue
                select new { m.ModuleName, sm.SubModuleName }
            )
            .Where(x => x.ModuleName != null && x.SubModuleName != null)
            .Select(x => new ValueTuple<string, string>(x.ModuleName!, x.SubModuleName!))
            .ToListAsync();
        }
        else
        {
            modulePreviousList = new List<(string, string)>();
        }

        var oldModulesByName = modulePreviousList
            .GroupBy(x => x.ModuleName)
            .ToDictionary(g => g.Key, g => g.Select(x => x.SubModuleName).Distinct().ToList());

        var newModulesByName = moduleCurrentList
            .GroupBy(x => x.ModuleName)
            .ToDictionary(g => g.Key, g => g.Select(x => x.SubModuleName).Distinct().ToList());

        var allModuleNames = oldModulesByName.Keys.Union(newModulesByName.Keys).Distinct();
        var moduleMapping = new List<Dictionary<string, object>>();
        foreach (var moduleName in allModuleNames)
        {
            var oldSubModules = oldModulesByName.TryGetValue(moduleName, out var o) ? o : new List<string>();
            var newSubModules = newModulesByName.TryGetValue(moduleName, out var n) ? n : new List<string>();
            var oldOrdered = oldSubModules.ToList();
            oldOrdered.Sort(StringComparer.Ordinal);
            var newOrdered = newSubModules.ToList();
            newOrdered.Sort(StringComparer.Ordinal);
            // Only if both old and new have values and they differ
            if ((oldOrdered.Count > 0 || newOrdered.Count > 0) && !oldOrdered.SequenceEqual(newOrdered))
            {
                moduleMapping.Add(new Dictionary<string, object>
                {
                    { moduleName, new { OldValue = oldSubModules, NewValue = newSubModules } }
                });
            }
        }

        // CompanyUser: companyAdmin (must have both old and new)
        var adminCurrent = await _dbContext.CompanyAdminUsersHistories
            .Where(a => a.CompanyId == companyId && !a.IsDeleted && a.Version == versionNumber)
            .FirstOrDefaultAsync();
        if (adminCurrent == null)
        {
            var latestAdminVersion = await _dbContext.CompanyAdminUsersHistories
                .Where(a => a.CompanyId == companyId && !a.IsDeleted)
                .OrderByDescending(a => a.Version)
                .Select(a => a.Version)
                .FirstOrDefaultAsync();
            if (latestAdminVersion != 0)
                adminCurrent = await _dbContext.CompanyAdminUsersHistories
                    .Where(a => a.CompanyId == companyId && !a.IsDeleted && a.Version == latestAdminVersion)
                    .FirstOrDefaultAsync();
        }
        int adminCurrentVersion = adminCurrent?.Version ?? companyCurrentVersion;
        var adminPrevious = await _dbContext.CompanyAdminUsersHistories
            .Where(a => a.CompanyId == companyId && !a.IsDeleted && a.Version < adminCurrentVersion)
            .OrderByDescending(a => a.Version)
            .FirstOrDefaultAsync();
        var companyUser = new List<Dictionary<string, object>>();
        if (adminCurrent != null && adminPrevious != null)
        {
            string? currentEmail = null, previousEmail = null;
            if (!string.IsNullOrEmpty(adminCurrent.PhoneNumber))
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == adminCurrent.PhoneNumber);
                currentEmail = user?.Email;
            }
            if (!string.IsNullOrEmpty(adminPrevious.PhoneNumber))
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == adminPrevious.PhoneNumber);
                previousEmail = user?.Email;
            }
            var fields2 = new[] { "FirstName", "LastName", "PhoneNumber", "Designation", "TelephoneCodeId" };

            var adminDiff = new Dictionary<string, object>();
            foreach (var field in fields2)
            {
                var oldValue = adminPrevious?.GetType().GetProperty(field)?.GetValue(adminPrevious)?.ToString();
                var newValue = adminCurrent?.GetType().GetProperty(field)?.GetValue(adminCurrent)?.ToString();
                if ((oldValue ?? "") != (newValue ?? ""))
                {
                    adminDiff[field] = new { oldValue, newValue };
                }
            }
            if ((previousEmail ?? "") != (currentEmail ?? ""))
            {
                adminDiff["EmailId"] = new { oldValue = previousEmail, newValue = currentEmail };
            }
            var keysToDelete = new List<string>();
            var keysToAppend = new Dictionary<string, object>();
            foreach (var key in adminDiff.Keys.ToList()) // use ToList to avoid modifying the collection during iteration
            {
                var value = adminDiff[key];

                if (value is { } && value.GetType().GetProperty("oldValue") != null)
                {
                    var oldValCodeIdStr = value.GetType().GetProperty("oldValue")?.GetValue(value)?.ToString();
                    var newValCodeIdStr = value.GetType().GetProperty("newValue")?.GetValue(value)?.ToString();

                    // Handle TelephoneCodeId
                    if (key == "TelephoneCodeId")
                    {
                        int? oldId = int.TryParse(oldValCodeIdStr, out var tempOldId) ? tempOldId : (int?)null;
                        int? newId = int.TryParse(newValCodeIdStr, out var tempNewId) ? tempNewId : (int?)null;

                        var oldCode = oldId.HasValue
                            ? _dbContext.TelephoneCodeMasters.FirstOrDefault(c => c.Id == oldId.Value)?.TelephoneCode
                            : null;

                        var newCode = newId.HasValue
                            ? _dbContext.TelephoneCodeMasters.FirstOrDefault(c => c.Id == newId.Value)?.TelephoneCode
                            : null;

                        // Replace the original TelephoneCodeId entry with resolved telephone codes
                        adminDiff[key] = new { oldValue = oldCode, newValue = newCode };
                        var newKey = "TelephoneCode";
                        keysToDelete.Add(key);
                        keysToAppend[newKey] = new { oldValue = oldCode, newValue = newCode };
                    }
                }
            }
            foreach (var key in keysToDelete)
            {
                adminDiff.Remove(key);
            }
            foreach (var kvp in keysToAppend)
            {
                adminDiff[kvp.Key] = kvp.Value;
            }
            if (adminDiff.Count > 0)
            {
                companyUser.Add(new Dictionary<string, object> { { "companyAdmin", adminDiff } });
            }
        }

        // CompanyUser: escalationContacts (allow new only)
        var fields = new[] { "FirstName", "LastName", "PhoneNumber", "Designation", "Department", "Remarks", "TelephoneCodeId" };
        var escalationCurrentList = await _dbContext.EscalationContactsHistories
            .Where(e => e.CompanyId == companyId && e.FirstName != AppConstants.EMPTYRECORDS && !e.IsDeleted && e.Version == versionNumber)
            .ToListAsync();

        int escalationCurrentVersion = escalationCurrentList.FirstOrDefault()?.Version ?? companyCurrentVersion;
        var prevEscalationVersion = await _dbContext.EscalationContactsHistories
            .Where(e => e.CompanyId == companyId && !e.IsDeleted && e.Version < versionNumber)
            .OrderByDescending(e => e.Version)
            .Select(e => e.Version)
            .FirstOrDefaultAsync();
        var escalationPreviousList = prevEscalationVersion != 0
            ? await _dbContext.EscalationContactsHistories
                .Where(e => e.CompanyId == companyId && e.FirstName != AppConstants.EMPTYRECORDS && !e.IsDeleted && e.Version == prevEscalationVersion)
                .ToListAsync()
            : new List<EscalationContactsHistory>();

        // Only include specified fields
        List<Dictionary<string, object?>> FilterFields(List<EscalationContactsHistory> list)
        {
            var result = new List<Dictionary<string, object?>>();
            foreach (var item in list)
            {
                var dict = new Dictionary<string, object?>();
                foreach (var field in fields)
                {
                    var prop = item.GetType().GetProperty(field);
                    if (prop != null)
                    {
                        dict[field] = prop.GetValue(item);
                    }
                }
                result.Add(dict);
            }
            return result;
        }

        // Compare contacts by all fields
        string GetContactKey(Dictionary<string, object?> contact)
        {
            return string.Join("|", fields.Select(f => contact.TryGetValue(f, out var v) ? v?.ToString() ?? "" : ""));
        }

        var filteredOld = FilterFields(escalationPreviousList);
        var filteredNew = FilterFields(escalationCurrentList);

        var oldKeys = new HashSet<string>(filteredOld.Select(GetContactKey));
        var newKeys = new HashSet<string>(filteredNew.Select(GetContactKey));
        bool contactsChanged = (!oldKeys.Any() && newKeys.Any()) ||
                              (oldKeys.Any() && newKeys.Any() && !oldKeys.SetEquals(newKeys));

        void ResolveTelephoneCode(List<Dictionary<string, object?>> list)
        {
            foreach (var item in list)
            {
                if (item.TryGetValue("TelephoneCodeId", out var codeIdObj) && codeIdObj != null)
                {
                    int codeId = 0;

                    if (codeIdObj is int)
                    {
                        codeId = (int)codeIdObj;
                    }
                    else if (int.TryParse(codeIdObj.ToString(), out var parsedId))
                    {
                        codeId = parsedId;
                    }

                    if (codeId != 0)
                    {
                        var telephoneCode = _dbContext.TelephoneCodeMasters
                            .Where(c => c.Id == codeId)
                            .Select(c => c.TelephoneCode)
                            .FirstOrDefault();

                        if (!string.IsNullOrEmpty(telephoneCode))
                        {
                            // Option 1: Replace TelephoneCodeId with code
                            // item["TelephoneCodeId"] = telephoneCode;
                            //item["TelephoneCodeId"] = telephoneCode;
                            // Add new key "TelephoneCode" with resolved value
                            item["TelephoneCode"] = telephoneCode;

                            // Remove the old key "TelephoneCodeId"
                            item.Remove("TelephoneCodeId");
                        }
                    }
                }
            }
        }
        ResolveTelephoneCode(filteredOld);
        ResolveTelephoneCode(filteredNew);
        if (contactsChanged && (filteredOld.Count > 0 || filteredNew.Count > 0))
        {
            companyUser.Add(new Dictionary<string, object>
            {
                { "escalationContacts", new { oldValue = filteredOld, newValue = filteredNew } }
            });
        }

        //companyUser.Add(new Dictionary<string, object>
        //{
        //    { "escalationContacts", new { oldValue = FilterFields(escalationPreviousList), newValue = FilterFields(escalationCurrentList) } }
        //});
       

        // Only add non-empty modules
        var company5TabHistory = new Dictionary<string, object>();
        if (companyInfo.Count > 0) company5TabHistory["companyInfo"] = companyInfo;
        if (accountInfo.Count > 0) company5TabHistory["accountInfo"] = accountInfo;
        if (elixirUser.Count > 0) company5TabHistory["elixirUser"] = elixirUser;
        if (moduleMapping.Count > 0) company5TabHistory["moduleMapping"] = moduleMapping;
        if (companyUser.Count > 0) company5TabHistory["companyUser"] = companyUser;

        return company5TabHistory;
    }

}
