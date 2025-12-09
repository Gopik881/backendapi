using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Telephone.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Elixir.Infrastructure.Persistance.Repositories;

public class TelephoneCodeMasterRepository : ITelephoneCodeMasterRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public TelephoneCodeMasterRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<TelephoneCodeMaster> GetByIdAsync(int TelephoneCodeId)
    {
        return await _dbContext.TelephoneCodeMasters.Where(tc => tc.Id == TelephoneCodeId && !tc.IsDeleted).FirstOrDefaultAsync();
    }
    public async Task<int> CreateTelephoneCodeAsync(TelephoneCodeMaster telephoneCode)
    {
        await _dbContext.TelephoneCodeMasters.AddAsync(telephoneCode);
        await _dbContext.SaveChangesAsync();
        return telephoneCode.Id;
    }
    public async Task<int> UpdateTelephoneAsync(TelephoneCodeMaster telephoneCode)
    {
        _dbContext.TelephoneCodeMasters.Update(telephoneCode);
        await _dbContext.SaveChangesAsync();
        return telephoneCode.Id;
    }
    public async Task<bool> DeleteTelephoneCodeAsync(int TelephoneCodeId)
    {
        var telephoneCode = await _dbContext.TelephoneCodeMasters
            .Where(tc => tc.Id == TelephoneCodeId && !tc.IsDeleted)
            .FirstOrDefaultAsync();
        if (telephoneCode == null) return false;

        // Append GUID for uniqueness on delete
        telephoneCode.TelephoneCode = $"{telephoneCode.TelephoneCode}_DELETED_{Guid.NewGuid()}";

        telephoneCode.IsDeleted = true;
        _dbContext.TelephoneCodeMasters.Update(telephoneCode);
        await _dbContext.SaveChangesAsync();
        return true;
    }
    public async Task<IEnumerable<TelephoneCodeMasterDto>> GetAllTelephoneCodesAsync()
    {
        var result = await _dbContext.TelephoneCodeMasters
            .Where(tc => !tc.IsDeleted)
            .Join(_dbContext.CountryMasters,
                tc => tc.CountryId,
                country => country.Id,
                (tc, country) => new TelephoneCodeMasterDto
                {
                    TelephoneCodeId = tc.Id,
                    CountryId = tc.CountryId,
                    CountryName = country.CountryName,
                    TelephoneCode = tc.TelephoneCode,
                    Description = tc.Description
                })
            .ToListAsync();
        return result;
    }
    public async Task<bool> ExistsWithTelephoneCodeAsync(string TelephoneCode, int CountryId)
    {
        return await _dbContext.TelephoneCodeMasters.AnyAsync(tc => EF.Functions.Like(tc.TelephoneCode, TelephoneCode) && !tc.IsDeleted && tc.CountryId == CountryId);
    }
    public async Task<bool> AnyDuplicateTelephoneCodesExistsAsync(List<CreateUpdateTelephoneCodeDto> telephoneCodes)
    {
        var countryNames = telephoneCodes.Select(c => c.TelephoneCode).ToList();

        return await _dbContext.TelephoneCodeMasters.AnyAsync(c =>
            !c.IsDeleted &&
            (countryNames.Contains(c.TelephoneCode)));
    }
    // Pseudocode / Plan (detailed):
    // 1. Build a query joining TelephoneCodeMasters and CountryMasters, filtering out deleted records.
    // 2. If searchTerm provided, apply search at the database level using EF.Functions.Like against:
    //    - CountryName
    //    - TelephoneCode
    //    - Description
    //   (This keeps filtering efficient and avoids pulling all rows into memory.)
    // 3. Compute totalCount from the filtered query before pagination.
    // 4. Order the query so the most recently created or updated records appear first.
    //    - Prefer UpdatedAt when available; otherwise use CreatedAt.
    //    - Use OrderByDescending with a coalesce (UpdatedAt ?? CreatedAt).
    // 5. Apply pagination (Skip/Take) and project the final page to TelephoneCodeMasterDto.
    // 6. Return the page and the totalCount as Tuple<List<TelephoneCodeMasterDto>, int>.
    // Note: This preserves existing functionality but ensures latest (created/updated) records are on top
    // and avoids pulling the entire table into memory.

    public async Task<Tuple<List<TelephoneCodeMasterDto>, int>> GetFilteredTelephoneCodesAsync(string searchTerm, int pageNumber, int pageSize)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        // Base query joining telephone codes with country
        var baseQuery = _dbContext.TelephoneCodeMasters
            .Where(tc => !tc.IsDeleted)
            .Join(_dbContext.CountryMasters,
                telephone => telephone.CountryId,
                country => country.Id,
                (telephone, country) => new { telephone, country });

        // Apply search (database-side) if provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm.Trim()}%";
            baseQuery = baseQuery.Where(x =>
                EF.Functions.Like(x.country.CountryName, term) ||
                EF.Functions.Like(x.telephone.TelephoneCode, term) ||
                EF.Functions.Like(x.telephone.Description ?? string.Empty, term));
        }

        // Count total records after filtering
        var totalCount = await baseQuery.CountAsync();

        // Order by UpdatedAt (if present) then CreatedAt descending so newest (created or updated) on top
        // Use null-coalescing so UpdatedAt is preferred when available.
        var pageItems = await baseQuery
            .OrderByDescending(x => x.telephone.UpdatedAt ?? x.telephone.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new TelephoneCodeMasterDto
            {
                TelephoneCodeId = s.telephone.Id,
                CountryId = s.telephone.CountryId,
                CountryName = s.country.CountryName,
                TelephoneCode = s.telephone.TelephoneCode,
                Description = s.telephone.Description,
                CreatedOn = s.telephone.CreatedAt,
            })
            .ToListAsync();

        return new Tuple<List<TelephoneCodeMasterDto>, int>(pageItems, totalCount);
    }
    public async Task<bool> BulkInsertTelephoneCodesAsync(List<TelephoneCodeBulkUploadDto> telephoneCodes)
    {
        try
        {
            var tcEntities = telephoneCodes.Select(c => new TelephoneCodeMaster
            {
                CountryId = c.CountryId,
                TelephoneCode = !string.IsNullOrWhiteSpace(c.TelephoneCode) && !c.TelephoneCode.StartsWith("+")
                        ? "+" + c.TelephoneCode
                        : c.TelephoneCode,
                Description = c.Description,
                IsDeleted = false
            }).ToList();
            await _dbContext.TelephoneCodeMasters.AddRangeAsync(tcEntities);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            throw new Exception(AppConstants.ErrorCodes.TELEPHONE_CODE_MASTER_DUPLICATE_FOUND);
        }
    }
    public async Task<bool> IsDuplicateTelephoneCodeExistsAsync(CreateUpdateTelephoneCodeDto telephoneCode)
    {
        if (telephoneCode.TelephoneCodeId != 0)
        {
            // Exclude the current record from duplicate checks
            return await _dbContext.TelephoneCodeMasters.AnyAsync(c =>
                !c.IsDeleted &&
                c.Id != telephoneCode.TelephoneCodeId &&
                c.TelephoneCode == telephoneCode.TelephoneCode);
        }
        return await _dbContext.TelephoneCodeMasters.AnyAsync(c =>
            !c.IsDeleted &&
            c.TelephoneCode == telephoneCode.TelephoneCode &&
            c.CountryId == telephoneCode.CountryId);
    }

}
