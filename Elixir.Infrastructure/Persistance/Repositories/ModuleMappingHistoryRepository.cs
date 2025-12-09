using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.Company5TabHistoryMapEntityToDictionary;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class ModuleMappingHistoryRepository : IModuleMappingHistoryRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public ModuleMappingHistoryRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Company5TabCreateModuleMappingDataAsync(int companyId, List<Company5TabModuleMappingDto> moduleMappings, int userId, CancellationToken cancellationToken = default)
    {
        // Get the last version number
        int lastVersion = _dbContext.ModuleMappingHistories
            .Where(mh => mh.CompanyId == companyId && !mh.IsDeleted)
            .OrderByDescending(mh => mh.Version)
            .Select(mh => (int?)mh.Version)
            .FirstOrDefault() ?? 0;

        // Get all valid SubModule IDs from the database
        var validSubModuleIds = await _dbContext.Set<SubModule>()
            .Select(sm => sm.SubModuleParentId)
            .Where(id => id != null)
            .ToListAsync(cancellationToken);

        // Create new mappings based on the DTO structure
        var newMappings = moduleMappings.SelectMany(module =>
        {
            var mappings = new List<ModuleMappingHistory>();

            // Add the module itself (if no submodules or if you want to track the module independently)
            mappings.Add(new ModuleMappingHistory
            {
                CompanyId = companyId,
                ModuleId = module.ModuleId,
                SubModuleId = null, // No submodule for the parent module
                Version = lastVersion + 1,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            });

            // Add submodules if they exist and are valid
            if (module.SubModules != null && module.SubModules.Any())
            {
                mappings.AddRange(module.SubModules
                    .Where(sub => sub.SubModuleId != 0)
                    .Select(sub => new ModuleMappingHistory
                    {
                        CompanyId = companyId,
                        ModuleId = module.ModuleId,
                        SubModuleId = sub.SubModuleId,
                        Version = lastVersion + 1,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = userId
                    }));
            }

            return mappings;
        });

        // Add mappings to the database and save
        _dbContext.ModuleMappingHistories.AddRange(newMappings);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
    public async Task<List<Company5TabModuleMappingDto>?> GetCompany5TabLatestModuleMappingHistoryAsync(int companyId, bool isPrevious, CancellationToken cancellationToken = default)
    {
        var versionQueryModuleMapping = _dbContext.ModuleMappingHistories
            .Where(e => e.CompanyId == companyId && !e.IsDeleted);

        int? latestVersion = 0;
        if (await versionQueryModuleMapping.AnyAsync(cancellationToken))
        {
            latestVersion = isPrevious
                ? await versionQueryModuleMapping
                    .Select(e => e.Version)
                    .Distinct()
                    .OrderByDescending(v => v)
                    .Skip(1)
                    .FirstOrDefaultAsync(cancellationToken)
                : await versionQueryModuleMapping
                    .Select(e => e.Version)
                    .Distinct()
                    .MaxAsync(cancellationToken);
        }

        return await _dbContext.ModuleMappingHistories
            .Where(mh => mh.CompanyId == companyId && !mh.IsDeleted && mh.Version == latestVersion)
            .GroupBy(mh => mh.ModuleId)
            .Select(g => new Company5TabModuleMappingDto
            {
                ModuleId = g.Key,
                ModuleName = _dbContext.Modules
                    .Where(m => m.Id == g.Key)
                    .Select(m => m.ModuleName)
                    .FirstOrDefault() ?? string.Empty,
                SubModules = g
                    .Where(mh => mh.SubModuleId != null)
                    .Join(
                        _dbContext.SubModules,
                        mh => mh.SubModuleId,
                        sm => sm.Id,
                        (mh, sm) => new Company5TabSubModulesDto
                        {
                            SubModuleId = mh.SubModuleId.Value,
                            SubModuleName = sm.SubModuleName
                        }
                    )
                    .GroupBy(sub => sub.SubModuleId)
                    .Select(subGroup => subGroup.First())
                    .ToList()
            })
            .ToListAsync(cancellationToken);
    }
    public async Task<bool> WithdrawCompany5TabModuleMappingHistoryAsync(int companyId, int userId, CancellationToken cancellationToken = default)
    {
        // Get the latest version for the given company and user
        int? latestVersion = await _dbContext.ModuleMappingHistories
            .Where(e => e.CompanyId == companyId)
            .MaxAsync(e => (int?)e.Version, cancellationToken);

        if (latestVersion == null) return true;

        // Find all records to remove
        var recordsToRemove = _dbContext.ModuleMappingHistories
            .Where(e => e.CompanyId == companyId && e.Version == latestVersion);
        if(recordsToRemove.Count() == 0) return true;
        _dbContext.ModuleMappingHistories.RemoveRange(recordsToRemove);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
    public async Task<Company5TabHistoryDto> GetCompany5TabModuleMappingHistoryByVersionAsync(
        int userId,
        int companyId,
        int versionNumber)
    {
        // Fetch the two most recent versions: requested and previous
        var histories = await _dbContext.ModuleMappingHistories
            .Where(m => m.CompanyId == companyId && (m.Version == versionNumber || m.Version == versionNumber - 1))
            .OrderByDescending(m => m.Version)
            .ToListAsync();

        Company5TabHistoryMapEntitytoDictionary company5TabHistoryMapEntitytoDictionary = new Company5TabHistoryMapEntitytoDictionary();

        var company5TabModuleMappingHistory = new Company5TabHistoryDto();

        foreach (var history in histories)
        {
            var mapped = company5TabHistoryMapEntitytoDictionary.MapEntityToDictionary(history)
                .ToDictionary(
                    keyValue => keyValue.Key,
                    keyValue => (object)new Dictionary<string, string> { { keyValue.Key, keyValue.Value } }
                );
            company5TabModuleMappingHistory.Company5TabHistory[history.Version.ToString()] = mapped;
        }

        return company5TabModuleMappingHistory;
    }

}
