using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class CountryMasterRepository : ICountryMasterRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public CountryMasterRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsAsync(int countryId)
    {
        return await _dbContext.CountryMasters.AnyAsync(c => c.Id == countryId && !c.IsDeleted);
    }

    public async Task<bool> AnyDuplicateDuplicateCountrysExistsAsync(List<CreateUpdateCountryDto> countries)
    {
        var countryNames = countries.Select(c => c.CountryName).ToList();
        var shortNames = countries.Select(c => c.CountryShortName).ToList();

        return await _dbContext.CountryMasters.AnyAsync(c =>
            !c.IsDeleted &&
            (countryNames.Contains(c.CountryName) || shortNames.Contains(c.CountryShortName)));
    }


    public async Task<bool> ExistsWithCountryNameAsync(string countryName)
    {
        return await _dbContext.CountryMasters.AnyAsync(c => EF.Functions.Like(c.CountryName, countryName) && !c.IsDeleted);
    }
    public async Task<Tuple<List<CountryWithStatesDto>, int>> GetCountryByIdWithStatesAsync(string searchTerm, int pageNumber, int pageSize, int countryId)
    {
        var query = _dbContext.StateMasters
            .Where(s => s.CountryId == countryId && !s.IsDeleted &&
                (string.IsNullOrEmpty(searchTerm) ||
                 s.StateName.Contains(searchTerm) ||
                 s.StateShortName.Contains(searchTerm) ||
                 (s.Description != null && s.Description.Contains(searchTerm))))
            .OrderBy(s => s.StateName);

        var totalCount = await query.CountAsync();

        var states = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
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

        var resultList = country != null ? new List<CountryWithStatesDto> { country } : new List<CountryWithStatesDto>();
        return new Tuple<List<CountryWithStatesDto>, int>(resultList, totalCount);
    }

    public async Task<int> CreateCountryAsync(CountryMaster country)
    {
        await _dbContext.CountryMasters.AddAsync(country);
        await _dbContext.SaveChangesAsync();
        return country.Id;
    }

    public async Task<bool> DeleteCountryAsync(int countryId)
    {
        var country = await _dbContext.CountryMasters.Where(country => country.Id == countryId && !country.IsDeleted).FirstOrDefaultAsync();
        if (country == null) return false;
        country.CountryName = $"{country.CountryName}_DELETED_{Guid.NewGuid()}"; // Append GUID for uniqueness on delete
        country.CountryShortName = $"{country.CountryShortName}_DELETED_{Guid.NewGuid()}"; // Append GUID for uniqueness on delete
        country.IsDeleted = true;
        _dbContext.CountryMasters.Update(country);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<CountryDto>> GetAllCountriesAsync()
    {
        return await _dbContext.CountryMasters
            .Where(country => !country.IsDeleted)
            .OrderBy(c => c.CountryName)
            .Select(c => new CountryDto
            {
                CountryId = c.Id,
                CountryName = c.CountryName,
                CountryShortName = c.CountryShortName,
                Description = c.Description
            }).ToListAsync();
    }

    public async Task<CountryMaster> GetCountryByIdAsync(int countryId)
    {
        return await _dbContext.CountryMasters.Where(country => country.Id == countryId && !country.IsDeleted).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateCountryAsync(CountryMaster country)
    {
        if (country == null) return false;
        _dbContext.CountryMasters.Update(country);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    //public async Task<Tuple<List<CountryDto>, int>> GetFilteredCountriesAsync(string searchTerm, int pageNumber, int pageSize)
    //{
    //    //single table so projection and filter applied directly
    //    var query = _dbContext.CountryMasters
    //        .Where(c => !c.IsDeleted)
    //        .OrderBy(c => c.CountryName);

    //    // Get the count before selecting specific columns
    //    //var totalCount = await query.CountAsync();
    //    var countries = await query
    //        //.Skip((pageNumber - 1) * pageSize)
    //        //.Take(pageSize)
    //        .Select(c => new CountryDto
    //        {
    //            CountryId = c.Id,
    //            CountryName = c.CountryName,
    //            CountryShortName = c.CountryShortName,
    //            Description = c.Description,
    //            CreatedOn = c.CreatedAt,
    //        }).ToListAsync();

    //    // Apply searchTerm logic on the final DTO list for all columns
    //    if (!string.IsNullOrEmpty(searchTerm))
    //    {
    //        countries = countries.Where(dto =>
    //            (dto.CountryName != null && dto.CountryName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
    //            (dto.CountryShortName != null && dto.CountryShortName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
    //        ).ToList();
    //        //totalCount = countries.Count;
    //    }
    //    var totalCount = countries.Count; // Get the total count after filtering
    //    countries = countries
    //        .Skip((pageNumber - 1) * pageSize)
    //        .Take(pageSize)
    //        .ToList();
    //    return new Tuple<List<CountryDto>, int>(countries, totalCount);
    //}

    public async Task<Tuple<List<CountryDto>, int>> GetFilteredCountriesAsync(string searchTerm, int pageNumber, int pageSize)
    {
        // single table so projection and filter applied directly
        var query = _dbContext.CountryMasters
            .Where(c => !c.IsDeleted)
            // Order primarily by most recently updated/created so created/updated records appear on top,
            // then by CountryName to preserve existing alphabetical behavior for items with same timestamp.
            //.OrderByDescending(c => (DateTime?)(c.CreatedAt))
            .OrderByDescending(c => ((DateTime?)c.UpdatedAt > (DateTime?)c.CreatedAt
                             ? (DateTime?)c.UpdatedAt
                             : (DateTime?)c.CreatedAt))
            .ThenBy(c => c.CountryName);

        var countries = await query
            .Select(c => new CountryDto
            {
                CountryId = c.Id,
                CountryName = c.CountryName,
                CountryShortName = c.CountryShortName,
                Description = c.Description,
                CreatedOn = c.CreatedAt,
            }).ToListAsync();

        // Apply searchTerm logic on the final DTO list for all columns (preserve existing behavior)
        if (!string.IsNullOrEmpty(searchTerm))
        {
            countries = countries.Where(dto =>
                (dto.CountryName != null && dto.CountryName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (dto.CountryShortName != null && dto.CountryShortName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }
        var totalCount = countries.Count; // Get the total count after filtering
        countries = countries
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return new Tuple<List<CountryDto>, int>(countries, totalCount);
    }

    //write bulk insert method here
    public async Task<bool> BulkInsertCountriesAsync(List<CountryBulkUploadDto> countries)
    {
        var countryEntities = countries.Select(c => new CountryMaster
        {
            CountryName = c.CountryName,
            CountryShortName = c.CountryShortName,
            Description = c.Description,
            IsDeleted = false
        }).ToList();
        await _dbContext.CountryMasters.AddRangeAsync(countryEntities);
        await _dbContext.SaveChangesAsync();
        return true;
    }
    public async Task<int?> GetBulkUploadFileSizeLimitMbAsync()
    {
        var fileSizeLimit = await _dbContext.SystemPolicies
            .Select(p => p.FileSizeLimitMb)
            .FirstOrDefaultAsync();
        return fileSizeLimit;
    }
    //public async Task<bool> IsDuplicateCountryExistsAsync(CreateUpdateCountryDto country)
    //{

    //    return await _dbContext.CountryMasters.AnyAsync(c =>
    //        !c.IsDeleted &&
    //        (c.CountryName == country.CountryName || c.CountryShortName == country.CountryShortName));
    //}

    public async Task<bool> IsDuplicateCountryExistsAsync(CreateUpdateCountryDto country)
    {
        if (country.CountryId != 0)
        {
            // Exclude the current record from duplicate checks
            return await _dbContext.CountryMasters.AnyAsync(c =>
                !c.IsDeleted &&
                c.Id != country.CountryId &&
                (c.CountryName == country.CountryName || c.CountryShortName == country.CountryShortName));
        }
        return await _dbContext.CountryMasters.AnyAsync(c =>
            !c.IsDeleted &&
            (c.CountryName == country.CountryName || c.CountryShortName == country.CountryShortName));

    }
}
