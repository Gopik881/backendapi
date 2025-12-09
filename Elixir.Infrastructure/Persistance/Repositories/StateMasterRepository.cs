using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;
public class StateMasterRepository : IStateMasterRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public StateMasterRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Add a new state  
    public async Task<int> CreateStateAsync(StateMaster state)
    {
        await _dbContext.StateMasters.AddAsync(state);
        await _dbContext.SaveChangesAsync();
        return state.Id;
    }

    // Get all states  
    public async Task<IEnumerable<StateDto>> GetAllStatesAsync()
    {
        var result = await _dbContext.StateMasters.Where(state => !state.IsDeleted)
        .Join(_dbContext.CountryMasters,
            state => state.CountryId,
            country => country.Id,
            (state, country) => new StateDto
            {
                StateId = state.Id,
                CountryId = state.CountryId,
                CountryName = country.CountryName,
                StateName = state.StateName,
                StateShortName = state.StateShortName,
                Description = state.Description
            })
        .ToListAsync();
        return result;
    }

    // Pseudocode / Plan:
    // 1. If incoming list is null or empty, return false (no duplicates).
    // 2. Normalize incoming state names and short names (trim + to lower) for reliable comparisons.
    // 3. Check duplicates inside the incoming list per CountryId:
    //    - Group by (CountryId, normalized StateName) and if any group count > 1 -> duplicate.
    //    - Group by (CountryId, normalized StateShortName) and if any group count > 1 -> duplicate.
    // 4. If no duplicates inside the list, query the DB for any existing states belonging to any of the CountryIds in the incoming list:
    //    - Load existing states with CountryId in incoming CountryIds and IsDeleted == false.
    //    - Compare in-memory: for each incoming state, check if any existing state for the same CountryId has the same normalized StateName or StateShortName.
    // 5. Return true if any duplicate found (either inside the list or against DB), otherwise false.
    public async Task<bool> AnyDuplicateStatesExistsAsync(List<CreateUpdateStateDto> states)
    {
        if (states == null || states.Count == 0)
            return false;

        // Normalize incoming values
        var normalized = states
            .Select(s => new
            {
                CountryId = s.CountryId,
                Name = s.StateName?.Trim(),
                NameNorm = string.IsNullOrWhiteSpace(s.StateName) ? null : s.StateName.Trim().ToLowerInvariant(),
                Short = s.StateShortName?.Trim(),
                ShortNorm = string.IsNullOrWhiteSpace(s.StateShortName) ? null : s.StateShortName.Trim().ToLowerInvariant()
            })
            .ToList();

        // Check duplicates within the incoming list by (CountryId + StateName)
        var duplicateNameInList = normalized
            .Where(x => x.NameNorm != null)
            .GroupBy(x => new { x.CountryId, x.NameNorm })
            .Any(g => g.Count() > 1);

        if (duplicateNameInList) return true;

        // Check duplicates within the incoming list by (CountryId + StateShortName)
        var duplicateShortInList = normalized
            .Where(x => x.ShortNorm != null)
            .GroupBy(x => new { x.CountryId, x.ShortNorm })
            .Any(g => g.Count() > 1);

        if (duplicateShortInList) return true;

        // Prepare country ids to query DB
        var countryIds = normalized.Select(x => x.CountryId).Distinct().ToList();

        if (!countryIds.Any()) return false;

        // Load existing states for those countries (only needed columns)
        var existing = await _dbContext.StateMasters
            .Where(s => !s.IsDeleted && countryIds.Contains(s.CountryId))
            .Select(s => new
            {
                s.CountryId,
                Name = s.StateName,
                Short = s.StateShortName
            })
            .ToListAsync();

        // Compare incoming normalized values against existing (case-insensitive)
        var existingLookup = existing
            .Select(e => new
            {
                e.CountryId,
                NameNorm = string.IsNullOrWhiteSpace(e.Name) ? null : e.Name.Trim().ToLowerInvariant(),
                ShortNorm = string.IsNullOrWhiteSpace(e.Short) ? null : e.Short.Trim().ToLowerInvariant()
            })
            .ToList();

        // Check if any incoming item matches an existing one for same country
        foreach (var inc in normalized)
        {
            if (inc.NameNorm != null && existingLookup.Any(e => e.CountryId == inc.CountryId && e.NameNorm == inc.NameNorm))
                return true;

            if (inc.ShortNorm != null && existingLookup.Any(e => e.CountryId == inc.CountryId && e.ShortNorm == inc.ShortNorm))
                return true;
        }

        return false;
    }
    // Get a state by ID  
    public async Task<StateMaster> GetStateByIdAsync(int stateId)
    {
        return await _dbContext.StateMasters.Where(state => state.Id == stateId && !state.IsDeleted).FirstOrDefaultAsync();
    }
    // Update an existing state  
    public async Task<bool> UpdateStateAsync(StateMaster state)
    {
        if (state == null) return false;
        _dbContext.StateMasters.Update(state);
        await _dbContext.SaveChangesAsync();
        return true;
    }
    // Delete a state by ID  
    public async Task<bool> DeleteStateAsync(int stateId)
    {
        var state = await _dbContext.StateMasters.Where(state => state.Id == stateId && !state.IsDeleted).FirstOrDefaultAsync();
        if (state == null) return false;
        // Append GUID for uniqueness on delete
        state.StateName = $"{state.StateName}_DELETED_{Guid.NewGuid()}";
        state.StateShortName = $"{state.StateShortName}_DELETED_{Guid.NewGuid()}";
        state.IsDeleted = true;
        _dbContext.StateMasters.Update(state);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsWithStateNameAsync(string StateName, int CountryId)
    {
        return await _dbContext.StateMasters.AnyAsync(s => EF.Functions.Like(s.StateName, StateName) && !s.IsDeleted && s.CountryId == CountryId);
    }

    public async Task<Tuple<List<StateDto>, int>> GetFilteredStatesAsync(string searchTerm, int pageNumber, int pageSize)
    {
        // Projection step
        var query = _dbContext.StateMasters
            .Join(_dbContext.CountryMasters,
                state => state.CountryId,
                country => country.Id,
                (state, country) => new { state, country });

        // Apply IsDeleted filter only
        query = query.Where(s => !s.state.IsDeleted && !s.country.IsDeleted);

        // Get the count before selecting specific columns
        //var totalCount = await query.CountAsync();

        // Apply pagination and projection
        var states = await query
            //.Skip((pageNumber - 1) * pageSize)
            //.Take(pageSize)
            .Select(s => new StateDto
            {
                StateId = s.state.Id,
                CountryId = s.state.CountryId,
                CountryName = s.country.CountryName,
                StateName = s.state.StateName,
                StateShortName = s.state.StateShortName,
                Description = s.state.Description,
                CreatedOn = s.state.CreatedAt,
            })
            .ToListAsync();

        // Apply searchTerm logic on all DTO columns after projection
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLowerInvariant();
            states = states.Where(dto =>
                (dto.StateId.ToString().Contains(lowerSearchTerm)) ||
                (dto.CountryId.ToString().Contains(lowerSearchTerm)) ||
                (!string.IsNullOrEmpty(dto.CountryName) && dto.CountryName.ToLowerInvariant().Contains(lowerSearchTerm)) ||
                (!string.IsNullOrEmpty(dto.StateName) && dto.StateName.ToLowerInvariant().Contains(lowerSearchTerm)) ||
                (!string.IsNullOrEmpty(dto.StateShortName) && dto.StateShortName.ToLowerInvariant().Contains(lowerSearchTerm)) //||
                                                                                                                               //(!string.IsNullOrEmpty(dto.Description) && dto.Description.ToLowerInvariant().Contains(lowerSearchTerm)) ||
                                                                                                                               //(dto.CreatedOn != null && dto.CreatedOn.Value.ToString("dd/MM/yyyy").ToLowerInvariant().Contains(lowerSearchTerm))
            ).ToList();
        }

        // Update totalCount to reflect filtered results
        var filteredCount = states.Count;
        states = states
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new Tuple<List<StateDto>, int>(states, filteredCount);
    }
    public async Task<CountryWithStatesDto?> GetCountryByIdWithStatesAsync(int countryId)
    {
        var states = await _dbContext.StateMasters
            .Where(s => s.CountryId == countryId && !s.IsDeleted)
            .OrderBy(s => s.StateName)
            .Select(s => new StateSummaryDto
            {
                StateId = s.Id,
                StateName = s.StateName,
                StateShortName = s.StateShortName,
                Description = s.Description
            }).ToListAsync();

        var country = await _dbContext.CountryMasters
            .Where(c => c.Id == countryId)
            .Select(c => new CountryWithStatesDto
            {
                CountryId = c.Id,
                CountryName = c.CountryName,
                States = states
            }).FirstOrDefaultAsync();

        return country;
    }


    public async Task<bool> BulkInsertStatesAsync(List<StateBulkUploadDto> states)
    {
        try
        {
            var stateEntities = states.Select(c => new StateMaster
            {
                CountryId = c.CountryId,
                StateName = c.StateName,
                StateShortName = c.StateShortName,
                Description = c.Description,
                IsDeleted = false
            }).ToList();
            await _dbContext.StateMasters.AddRangeAsync(stateEntities);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Duplicate entry detected. Cannot add the same record.");
        }
    }
    public async Task<bool> IsDuplicateStateExistsAsync(CreateUpdateStateDto stateDto)
    {
        //    return await _dbContext.StateMasters.AnyAsync(c =>
        //        !c.IsDeleted &&
        //        (c.StateName == stateDto.StateName || c.StateShortName == stateDto.StateShortName));
        if (stateDto == null)
            return false;

        // Normalize incoming values
        var nameNorm = string.IsNullOrWhiteSpace(stateDto.StateName) ? null : stateDto.StateName.Trim().ToLowerInvariant();
        var shortNorm = string.IsNullOrWhiteSpace(stateDto.StateShortName) ? null : stateDto.StateShortName.Trim().ToLowerInvariant();

        // Nothing to compare
        if (nameNorm == null && shortNorm == null)
            return false;

        // Ensure CountryId exists on DTO; if not present, we fall back to checking across all countries (preserve previous behavior expectations).
        // Most callers supply CountryId; using reflection only if property exists would be overkill here.
        var countryIdProperty = stateDto.GetType().GetProperty("CountryId");
        int? countryId = null;
        if (countryIdProperty != null)
        {
            var val = countryIdProperty.GetValue(stateDto);
            if (val is int v) countryId = v;
            else if (val is int?) countryId = (int?)val;
        }

        // Load existing states for the country (or all if countryId not provided)
        var query = _dbContext.StateMasters.Where(s => !s.IsDeleted);
        
        if(stateDto.StateId != 0)
            query = query.Where(s => s.Id != stateDto.StateId);

        if (countryId.HasValue)
            query = query.Where(s => s.CountryId == countryId.Value);

        var existing = await query
            .Select(s => new { s.CountryId, s.StateName, s.StateShortName })
            .ToListAsync();

        // Normalize existing values
        var existingLookup = existing
            .Select(e => new
            {
                e.CountryId,
                NameNorm = string.IsNullOrWhiteSpace(e.StateName) ? null : e.StateName.Trim().ToLowerInvariant(),
                ShortNorm = string.IsNullOrWhiteSpace(e.StateShortName) ? null : e.StateShortName.Trim().ToLowerInvariant()
            })
            .ToList();

        // If countryId is provided, compare only those rows; otherwise compare all rows
        foreach (var e in existingLookup)
        {
            if (countryId.HasValue && e.CountryId != countryId.Value)
                continue;

            if (nameNorm != null && e.NameNorm != null && e.NameNorm == nameNorm)
                return true;

            if (shortNorm != null && e.ShortNorm != null && e.ShortNorm == shortNorm)
                return true;
        }

        return false;
    }

}