using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Currency.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;


namespace Elixir.Infrastructure.Persistance.Repositories;

public class CurrencyMasterRepository : ICurrencyMasterRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public CurrencyMasterRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // PSEUDOCODE / PLAN (detailed):
    // 1. Goal: ensure newest records appear at the top when retrieving currencies (both filtered and full list).
    // 2. Determine "newest" as the most recently updated record; if UpdatedAt is null, fallback to CreatedAt.
    // 3. For GetAllCurrenciesAsync:
    //    - Query CurrencyMasters where !IsDeleted
    //    - Join with CountryMasters to get country name
    //    - Order the joined results by (UpdatedAt ?? CreatedAt) descending
    //    - Project into CurrencyDto and return list
    // 4. For GetFilteredCurrenciesAsync:
    //    - Build the base join query (currency + country) for non-deleted currencies
    //    - Apply searchTerm filtering on country name, currency name, or currency short name (case-insensitive)
    //    - Count results after filtering
    //    - Order filtered results by (UpdatedAt ?? CreatedAt) descending
    //    - Apply Skip/Take for pagination
    //    - Project to CurrencyDto and return the list and total count as a Tuple
    // 5. Keep existing behavior (search, pagination, projections) intact, only change ordering to show latest updates first.
    // 6. Use EF Core-compatible expressions: (s.currency.UpdatedAt ?? s.currency.CreatedAt) to allow translation to SQL.

    public async Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync()
    {
        try
        {
            var query = _dbContext.CurrencyMasters
                .Where(currency => !currency.IsDeleted)
                .Join(_dbContext.CountryMasters,
                    currency => currency.CountryId,
                    country => country.Id,
                    (currency, country) => new { currency, country });

            // Order by latest updated (fallback to created) first
            var result = await query
                .OrderByDescending(x => x.currency.UpdatedAt ?? x.currency.CreatedAt)
                .Select(x => new CurrencyDto
                {
                    CurrencyId = x.currency.Id,
                    CountryId = x.currency.CountryId,
                    CountryName = x.country.CountryName,
                    CurrencyName = x.currency.CurrencyName,
                    CurrencyShortName = x.currency.CurrencyShortName,
                    Description = x.currency.Description,
                    CreatedOn = x.currency.CreatedAt,
                })
                .ToListAsync();

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(AppConstants.ErrorCodes.CLIENT_ACCOUNT_MANAGERS_RETRIEVAL_FAILED);
        }
    }
    public async Task<bool> AnyDuplicateCurrenciesExistsAsync(List<CreateUpdateCurrencyDto> currencies)
    {
        try
        {
            // Get all country IDs and currency names/short names from the input
            var countryIds = currencies.Select(c => c.CountryId).Distinct().ToList();
            var currencyNames = currencies.Select(c => c.CurrencyName).Distinct().ToList();
            var currencyShortNames = currencies.Select(c => c.CurrencyShortName).Distinct().ToList();

            // Check if any of these countries already have a currency (not deleted)
            bool exists = await _dbContext.CurrencyMasters
                .AnyAsync(c => !c.IsDeleted &&
                    (
                        countryIds.Contains(c.CountryId) ||
                        currencyNames.Contains(c.CurrencyName) ||
                        currencyShortNames.Contains(c.CurrencyShortName)
                    ));

            return exists;
        }
        catch (Exception ex)
        {
            // Optionally log ex here
            throw new Exception("Error checking for duplicate currencies.", ex);
        }
    }

    public async Task<CurrencyMaster> GetCurrencyByIdAsync(int currencyId)
    {
        return await _dbContext.CurrencyMasters.Where(c => c.Id == currencyId && !c.IsDeleted).FirstOrDefaultAsync();
    }

    public async Task<int> CreateCurrencyAsync(CurrencyMaster currency)
    {
        await _dbContext.CurrencyMasters.AddAsync(currency);
        await _dbContext.SaveChangesAsync();
        return currency.Id;
    }

    public async Task<bool> UpdateAsync(CurrencyMaster currency)
    {
        if (currency == null) return false;
        _dbContext.CurrencyMasters.Update(currency);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int currencyId)
    {
        var currency = await _dbContext.CurrencyMasters.Where(c => c.Id == currencyId && !c.IsDeleted).FirstOrDefaultAsync();
        if (currency == null) return false;
        currency.CurrencyName = $"{currency.CurrencyName}_DELETED_{Guid.NewGuid()}"; // Append GUID for uniqueness on delete
        currency.CurrencyShortName = $"{currency.CurrencyShortName}_DELETED_{Guid.NewGuid()}"; // Append GUID for uniqueness on delete
        currency.IsDeleted = true;
        _dbContext.CurrencyMasters.Update(currency);
        await _dbContext.SaveChangesAsync();
        return true;
    }
    public async Task<bool> ExistsWithCurrencyNameAsync(string CurrencyName, int CountryId)
    {
        return await _dbContext.CurrencyMasters.AnyAsync(c => EF.Functions.Like(c.CurrencyName, CurrencyName) && !c.IsDeleted && c.CountryId == CountryId);
    }

    public async Task<Tuple<List<CurrencyDto>, int>> GetFilteredCurrenciesAsync(string searchTerm, int pageNumber, int pageSize)
    {
        // Projection step
        var query = _dbContext.CurrencyMasters.Where(cu => !cu.IsDeleted)
            .Join(_dbContext.CountryMasters,
                currency => currency.CountryId,
                country => country.Id,
                (currency, country) => new { currency, country });

        // Apply searchTerm logic before pagination for efficiency
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var loweredSearchTerm = searchTerm.Trim().ToLower();
            query = query.Where(s =>
                (!string.IsNullOrEmpty(s.country.CountryName) && s.country.CountryName.ToLower().Contains(loweredSearchTerm)) ||
                (!string.IsNullOrEmpty(s.currency.CurrencyName) && s.currency.CurrencyName.ToLower().Contains(loweredSearchTerm)) ||
                (!string.IsNullOrEmpty(s.currency.CurrencyShortName) && s.currency.CurrencyShortName.ToLower().Contains(loweredSearchTerm))
            );
        }

        // Get the count after filtering
        var totalCount = await query.CountAsync();

        // Apply ordering by latest updated (fallback to created) first, then pagination, then projection
        var currencies = await query
            .OrderByDescending(s => s.currency.UpdatedAt ?? s.currency.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new CurrencyDto
            {
                CurrencyId = s.currency.Id,
                CountryId = s.currency.CountryId,
                CountryName = s.country.CountryName,
                CurrencyName = s.currency.CurrencyName,
                CurrencyShortName = s.currency.CurrencyShortName,
                Description = s.currency.Description,
                CreatedOn = s.currency.CreatedAt,
            })
            .ToListAsync();

        return new Tuple<List<CurrencyDto>, int>(currencies, totalCount);
    }

    public async Task<bool> BulkInsertCurrenciesAsync(List<CurrencyBulkUploadDto> currencies)
    {
        var currencyEntities = currencies.Select(c => new CurrencyMaster
        {
            CountryId = c.CountryId,
            CurrencyName = c.CurrencyName,
            CurrencyShortName = c.CurrencyShortName,
            Description = c.Description,
            IsDeleted = false
        }).ToList();
        await _dbContext.CurrencyMasters.AddRangeAsync(currencyEntities);
        await _dbContext.SaveChangesAsync();
        return true;
    }
    public async Task<bool> IsDuplicateCurrencyExistsAsync(CreateUpdateCurrencyDto currency)
    {
        try
        {
            if (currency.CurrencyId != 0)
            {
                // Exclude the current record from duplicate checks
                return await _dbContext.CurrencyMasters.AnyAsync(c =>
                    !c.IsDeleted &&
                    c.Id != currency.CurrencyId &&
                    (c.CurrencyName == currency.CurrencyName && c.CurrencyShortName == currency.CurrencyShortName));
            }
            // Check if any currency exists with the same CountryId, CurrencyName, or CurrencyShortName (not deleted)
            bool exists = await _dbContext.CurrencyMasters
                .AnyAsync(c => !c.IsDeleted &&
                    (
                        c.CountryId == currency.CountryId ||
                        c.CurrencyName == currency.CurrencyName ||
                        c.CurrencyShortName == currency.CurrencyShortName
                    ));

            return exists;
        }
        catch (Exception ex)
        {
            // Optionally log ex here
            throw new Exception("Error checking for duplicate currency.", ex);
        }
    }

}
