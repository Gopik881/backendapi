using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class ModuleMappingRepository : IModuleMappingRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public ModuleMappingRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Pseudocode / Plan (detailed):
    // 1. For each Module, compute a "CreatedOn" that represents the latest activity timestamp:
    //    - Fetch the latest ModuleMapping.CreatedAt for that module (if any).
    //    - Use the module.CreatedAt as fallback.
    //    - This ensures modules which had recent mappings (created/updated) surface on top.
    // 2. Preserve existing DTO projection (ModuleId, ModuleName, Status, CompanyCount, SubModuleCount).
    // 3. Keep existing searchTerm filtering logic unchanged.
    // 4. Apply ordering by CreatedOn descending before pagination so newest records appear first.
    // 5. Count total after filtering and then apply Skip/Take with ordering to get paged results.
    public async Task<Tuple<List<ModuleUsageDto>, int>> GetFilteredModulesUsageAsync(string searchTerm, int pageNumber, int pageSize)
    {
        var query = _dbContext.Modules
            .Where(m => !m.IsDeleted && m.CreatedBy == 1)
            .Select(m => new ModuleUsageDto
            {
                ModuleId = m.Id,
                // Use latest activity: prefer latest mapping.CreatedAt if present, otherwise module.CreatedAt
                CreatedOn = (_dbContext.ModuleMappings
                                 .Where(mm => mm.ModuleId == m.Id)
                                 .Select(mm => (DateTime?)mm.CreatedAt)
                                 .Max() ?? m.CreatedAt),
                ModuleName = m.ModuleName,
                Status = m.IsEnabled,
                CompanyCount = _dbContext.ModuleMappings
                    .Where(mm => mm.ModuleId == m.Id)
                    .Select(mm => mm.CompanyId)
                    .Distinct()
                    .Count(),
                SubModuleCount = m.Id == AppConstants.CORE_HR_ID
                    ? _dbContext.SubModules.Where(sm => sm.ModuleId == m.Id).Select(sm => sm.Id).Distinct().Count() - 2
                    : _dbContext.SubModules.Where(sm => sm.ModuleId == m.Id).Select(sm => sm.Id).Distinct().Count()
            })
            .Distinct();

        // Apply searchTerm logic on all columns of the DTO
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(dto =>
                (dto.ModuleName != null && dto.ModuleName.Contains(searchTerm)) ||
                //dto.ModuleId.ToString().Contains(searchTerm) ||               
                (dto.Status == true && "enabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (dto.Status == false && "disabled".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                dto.CompanyCount.ToString().Contains(searchTerm) ||
                dto.SubModuleCount.ToString().Contains(searchTerm)
            );
        }

        var totalCount = await query.CountAsync();
        var paginatedResult = await query
            .OrderByDescending(dto => dto.CreatedOn) // ensure latest records appear on top
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return new Tuple<List<ModuleUsageDto>, int>(paginatedResult, totalCount);
    }
    // Pseudocode / Plan (detailed):
    // 1. Build baseQuery same as before (filter by company or module when provided).
    // 2. Group by CompanyId and ModuleId (existing behavior).
    // 3. In projection set CreatedOn to the latest mapping timestamp for the group.
    //    - Use g.Max(mm => mm.CreatedAt) to capture the most recently created mapping in the group.
    //    - (This is a minimal, safe change that keeps existing behavior but surfaces latest records first.)
    // 4. Execute query to materialize the DTO list.
    // 5. Apply the existing searchTerm filtering logic on the in-memory DTOs (unchanged).
    // 6. Order the filtered result by CreatedOn descending so the latest records (created/updated) appear on top.
    //    - Ordering is done in-memory to avoid affecting the existing EF expression tree.
    // 7. Compute total distinct company count, then apply paging (Skip/Take) on the ordered list.
    // 8. Return pagedResult and total count as before.
    public async Task<Tuple<List<CompanyModuleDto>, int>> GetFilteredCompanyModulesAsync(string filterBy, int filterId, string searchTerm, int pageNumber, int pageSize)
    {
        IQueryable<ModuleMapping> baseQuery = _dbContext.ModuleMappings
            .Where(mm => !mm.IsDeleted);

        if (String.Equals(filterBy, AppConstants.COMPANY))
        {
            baseQuery = baseQuery.Where(mm => mm.CompanyId == filterId);
        }
        else if (String.Equals(filterBy, AppConstants.MODULE))
        {
            baseQuery = baseQuery.Where(mm => mm.ModuleId == filterId);
        }

        var groupedQuery = baseQuery
            .GroupBy(mm => new { mm.CompanyId, mm.ModuleId });

        var result = await groupedQuery.Select(g => new CompanyModuleDto
        {
            CompanyId = g.Key.CompanyId,
            // Use the latest mapping timestamp in the group so newest records surface first
            CreatedOn = g.Max(mm => mm.CreatedAt),
            CompanyName = _dbContext.Companies
                .Where(c => c.Id == g.Key.CompanyId && !c.IsDeleted)
                .Select(c => c.CompanyName)
                .FirstOrDefault() ?? string.Empty,
            Modules = g.Select(mm => mm.ModuleId).Distinct()
                .Join(_dbContext.Modules.Where(m => !m.IsDeleted),
                    moduleId => moduleId,
                    module => module.Id,
                    (moduleId, module) => new ModuleDto
                    {
                        ModuleId = module.Id,
                        ModuleName = module.ModuleName,
                        SubModules = g.Where(mm => mm.ModuleId == module.Id)
                            //.Join(_dbContext.SubModules.Where(sm => !sm.IsDeleted),
                            .Join(_dbContext.SubModules.Where(sm => !sm.IsDeleted && (module.Id == 1 ? (sm.SubModuleParentId > 0) : true)),
                        mm => mm.SubModuleId,
                                sm => sm.Id,
                                (mm, sm) => new SubModuleDto
                                {
                                    SubModuleId = sm.Id,
                                    SubModuleName = sm.SubModuleName
                                })
                            .Distinct()
                            .ToList()
                    })
                .ToList()
        }).ToListAsync();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var filteredResult = new List<CompanyModuleDto>();

            foreach (var company in result)
            {
                var filteredModules = new List<ModuleDto>();

                foreach (var module in company.Modules)
                {
                    var filteredSubModules = module.SubModules?
                        .Where(sm => !string.IsNullOrEmpty(sm.SubModuleName) &&
                                     sm.SubModuleName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    bool moduleNameMatches = !string.IsNullOrEmpty(module.ModuleName) &&
                                            module.ModuleName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);

                    if (moduleNameMatches || (filteredSubModules != null && filteredSubModules.Any()))
                    {
                        filteredModules.Add(new ModuleDto
                        {
                            ModuleId = module.ModuleId,
                            ModuleName = module.ModuleName,
                            SubModules = filteredSubModules ?? new List<SubModuleDto>()
                        });
                    }
                }

                bool companyNameMatches = !string.IsNullOrEmpty(company.CompanyName) &&
                                          company.CompanyName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);

                if (companyNameMatches)
                {
                    filteredResult.Add(new CompanyModuleDto
                    {
                        CompanyId = company.CompanyId,
                        CompanyName = company.CompanyName,
                        CreatedOn = company.CreatedOn,
                        Modules = company.Modules
                    });
                }
                else if (filteredModules.Any())
                {
                    filteredResult.Add(new CompanyModuleDto
                    {
                        CompanyId = company.CompanyId,
                        CompanyName = company.CompanyName,
                        CreatedOn = company.CreatedOn,
                        Modules = filteredModules
                    });
                }
            }

            result = filteredResult;
        }

        // Order results so the latest records appear on top (by CreatedOn descending)
        result = result.OrderByDescending(r => r.CreatedOn).ToList();

        var totalCompaniesCount = result.Distinct().Count();

        // Pagination after ordering and filtering
        var pagedResult = result
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new Tuple<List<CompanyModuleDto>, int>(pagedResult, totalCompaniesCount);
    }

    //super star code
    //public async Task<Tuple<List<CompanyModuleDto>, int>> GetFilteredCompanyModulesAsync(string FilterBy, int filterId, string searchTerm, int pageNumber, int pageSize)
    //{
    //    IQueryable<IGrouping<int, ModuleMapping>> query;

    //    if (String.Equals(FilterBy, AppConstants.COMPANY))
    //    {
    //        query = _dbContext.ModuleMappings
    //            .Where(m => m.CompanyId == filterId && !m.IsDeleted)
    //            .GroupBy(m => m.CompanyId);
    //    }
    //    else if (String.Equals(FilterBy, AppConstants.MODULE))
    //    {
    //        query = _dbContext.ModuleMappings
    //            .Where(m => m.ModuleId == filterId && !m.IsDeleted)
    //            .GroupBy(m => m.CompanyId);
    //    }
    //    else
    //    {
    //        query = _dbContext.ModuleMappings
    //            .Where(m => !m.IsDeleted)
    //            .GroupBy(m => m.CompanyId);
    //    }

    //    var result = await query.Select(g => new CompanyModuleDto
    //    {
    //        CompanyId = g.Key,
    //        CompanyName = _dbContext.Companies
    //            .Where(c => c.Id == g.Key && !c.IsDeleted && (string.IsNullOrEmpty(searchTerm) || c.CompanyName != null && c.CompanyName.Contains(searchTerm)))
    //            .Select(c => c.CompanyName)
    //            .FirstOrDefault() ?? string.Empty,
    //        Modules = g.Select(m => m.ModuleId).Distinct()
    //            .Join(_dbContext.Modules.Where(mod => !mod.IsDeleted), moduleId => moduleId, mod => mod.Id, (moduleId, mod) => new ModuleDto
    //            {
    //                ModuleId = mod.Id,
    //                ModuleName = mod.ModuleName,
    //                SubModules = g.Join(_dbContext.SubModules.Where(sm => !sm.IsDeleted), m => m.SubModuleId, sm => sm.Id, (m, sm) => new SubModuleDto
    //                {
    //                    SubModuleId = sm.Id,
    //                    SubModuleName = sm.SubModuleName
    //                })
    //                .Where(sm => string.IsNullOrEmpty(searchTerm) || (sm.SubModuleName != null && sm.SubModuleName.Contains(searchTerm)))
    //                .Distinct()
    //                .ToList()
    //            })
    //            .Where(module => string.IsNullOrEmpty(searchTerm) || (module.ModuleName != null && module.ModuleName.Contains(searchTerm)))
    //            .Skip((pageNumber - 1) * pageSize)
    //            .Take(pageSize)
    //            .ToList()
    //    }).ToListAsync();

    //    // Get the total count of distinct modules before pagination
    //    var totalModulesCount = result.SelectMany(g => g.Modules.Select(m => m.ModuleId).Distinct()).Count();
    //    return new Tuple<List<CompanyModuleDto>, int>(result, totalModulesCount);
    //}


    public async Task<bool> Company5TabApproveModuleMappingDataAsync(int companyId, List<Company5TabModuleMappingDto> moduleMappings, int userId, CancellationToken cancellationToken = default)
    {
        // Remove existing mappings for the company
        var existingMappings = _dbContext.ModuleMappings.Where(m => m.CompanyId == companyId);
        _dbContext.ModuleMappings.RemoveRange(existingMappings);

        // Create new mappings based on the DTO structure
        var newMappings = moduleMappings.SelectMany(module =>
        {
            var mappings = new List<ModuleMapping>();

            // Add the module itself (if no submodules or if you want to track the module independently)
            mappings.Add(new ModuleMapping
            {
                CompanyId = companyId,
                ModuleId = module.ModuleId,
                SubModuleId = null, // No submodule for the parent module
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            });

            // Add submodules if they exist
            if (module.SubModules != null && module.SubModules.Any())
            {
                mappings.AddRange(module.SubModules
                    .Select(sub => new ModuleMapping
                    {
                        CompanyId = companyId,
                        ModuleId = module.ModuleId,
                        SubModuleId = sub.SubModuleId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = userId
                    }));
            }

            return mappings;
        });

        // Add mappings to the database and save
        _dbContext.ModuleMappings.AddRange(newMappings);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

}
