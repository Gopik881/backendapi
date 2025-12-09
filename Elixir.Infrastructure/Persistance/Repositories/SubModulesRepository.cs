using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Common.Constants;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class SubModulesRepository : ISubModulesRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public SubModulesRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    /*
    PSEUDOCODE / PLAN (detailed):
    - Goal: Ensure the most recently created or updated SubModule appears first without changing other behavior.
    - Approach:
      1. Build the same IQueryable 'query' as before to respect ModuleId, IsEnabled, IsDeleted and exclusions.
      2. Determine the "last modified" timestamp for ordering:
         - Prefer UpdatedAt when present (including shadow property), otherwise use CreatedAt.
         - Use EF.Property<DateTime?> to safely access UpdatedAt whether it's a CLR property or shadow property.
      3. Apply ordering by that last-modified timestamp in descending order so newest (created or updated) appear first.
      4. Keep the existing projection to SubModuleDto and existing post-query search/pagination logic intact to avoid disturbing functionality.
      5. Return tuple with paged list and total count (count after search filtering, as before).
    - Notes:
      - Minimally invasive: only change is ordering expression to use last-modified timestamp descending.
      - Uses EF.Property to avoid compile issues if UpdatedAt is not a CLR property.
    */

    public async Task<Tuple<List<SubModuleDto>, int>> GetFilteredSubModulesByModuleAsync(int moduleId, string searchTerm, int pageNumber, int pageSize)
    {
        // Build the queryable, not a list
        var query = _dbContext.SubModules
                    .Where(sm => sm.ModuleId == moduleId && (sm.IsEnabled ?? false) && !sm.IsDeleted)
                    // Exclude FUNDAMENTALS and HORIZONTALS for ModuleId 1
                    .Where(sm => moduleId != AppConstants.CORE_HR_ID || (sm.Id != AppConstants.FUNDAMENTALS_ID && sm.Id != AppConstants.HORIZONTALS_ID))
                    .Distinct();

        // Determine last-modified timestamp: prefer UpdatedAt (if present/shadow), fallback to CreatedAt
        // Use EF.Property to safely access UpdatedAt whether it's a CLR property or shadow property.
        var submodules = await query
            .OrderByDescending(sm => (EF.Property<System.DateTime?>(sm, "UpdatedAt") ?? sm.CreatedAt))
            .Select(sm => new SubModuleDto
            {
                SubModuleId = sm.Id,
                SubModuleName = sm.SubModuleName,
                CreatedOn = sm.CreatedAt,
            })
            .ToListAsync();

        // Apply searchTerm logic on the final DTO list for all DTO columns
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            submodules = submodules
                .Where(dto =>
                    (!string.IsNullOrEmpty(dto.SubModuleName) && dto.SubModuleName.ToLower().Contains(lowerSearchTerm)) //||
                                                                                                                        //(dto.CreatedOn != null && dto.CreatedOn.ToString().ToLower().Contains(lowerSearchTerm))
                )
                .ToList();
        }
        var totalCount = submodules.Count; // Get the count after filtering
        submodules = submodules
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new Tuple<List<SubModuleDto>, int>(submodules, totalCount);
    }
}
