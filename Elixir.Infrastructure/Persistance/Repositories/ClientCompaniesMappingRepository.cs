using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Enums;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class ClientCompaniesMappingRepository : IClientCompaniesMappingRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public ClientCompaniesMappingRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> CreateClientCompanyMapDataAsync(int userId, int clientId, List<ClientCompanyMappingDto> clientCompanyMappingDtos, string ClientName)
    {
        // Remove existing ClientCompaniesMappings for the client
        var existingMappings = await _dbContext.ClientCompaniesMappings
            .Where(ccm => ccm.ClientId == clientId)
            .ToListAsync();

        if (existingMappings.Any())
        {
            _dbContext.ClientCompaniesMappings.RemoveRange(existingMappings);
            await _dbContext.SaveChangesAsync();
        }

        // Add new client company mapping details
        var newMappings = clientCompanyMappingDtos.Select(ccm => new ClientCompaniesMapping
        {
            ClientId = clientId,
            CompanyId = ccm.CompanyId,
            CreatedBy = userId,
            UpdatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        }).ToList();

        if (newMappings.Any())
        {
            await _dbContext.ClientCompaniesMappings.AddRangeAsync(newMappings);
            await _dbContext.SaveChangesAsync();
        }

        // Only select valid company IDs (greater than 0)
        var companyIds = clientCompanyMappingDtos
            .Where(dto => dto.CompanyId > 0)
            .Select(dto => dto.CompanyId)
            .ToList();

        if (!companyIds.Any())
            return true;

        var companies = await _dbContext.Companies
            .Where(c => companyIds.Contains(c.Id))
            .ToListAsync();

        if (!companies.Any())
            return true;

       
        foreach (var company in companies)
        {
            company.ClientName = ClientName;
            company.UpdatedAt = DateTime.UtcNow;
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<bool> DeleteClientCompaniesMappingsAsync(int clientId)
    {
        // Step 1: Fetch clientName for the given clientId
        var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        if (client == null)
            return false;

        var clientName = client.ClientName;
        // Step 2: Fetch all clientIds with the same clientName
        var clientIds = await _dbContext.Clients
            .Where(c => c.ClientName == clientName)
            .Select(c => c.Id)
            .ToListAsync();

        // Delete companies where ClientId is in the given clientIds list AND CompanyName is null or empty
        var companiesToDelete = await _dbContext.Companies
            .Where(c => clientIds.Contains(c.ClientId ?? 0) && (string.IsNullOrEmpty(c.CompanyName)))
            .ToListAsync();

        if (companiesToDelete.Any())
        {
            _dbContext.Companies.RemoveRange(companiesToDelete);
            await _dbContext.SaveChangesAsync();
        }

        await RemoveMappingsByCompanyNameAsync(clientName);

       

        if (!clientIds.Any())
            return true;

        // Step 3: Fetch all companyIds from Companies table for these clientIds
        var companyIds = await _dbContext.Companies
            .Where(co => clientIds.Contains(co.ClientId ?? 0))
            .Select(co => co.Id)
            .ToListAsync();

        if (!companyIds.Any())
            return true;

        // Step 4: Delete these companyIds from ClientCompaniesMappings table
        var mappingsToDelete = await _dbContext.ClientCompaniesMappings
            .Where(ccm => companyIds.Contains(ccm.CompanyId))
            .ToListAsync();

        if (!mappingsToDelete.Any())
            return true;

        _dbContext.ClientCompaniesMappings.RemoveRange(mappingsToDelete);
        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<bool> UpdateClientCompanyMapDataAsync(int userId, int clientId, List<ClientCompanyMappingDto> clientCompanyMappingDtos, string clientName, bool IsSuperUser)
    {
        // Validate input
        if (clientCompanyMappingDtos == null)
            throw new ArgumentNullException(nameof(clientCompanyMappingDtos));
        if (clientCompanyMappingDtos.Count == 0)
        {      
            await RemoveMappingsByCompanyNameAsync(clientName);
            return true;
        }

        // Get all companyIds for the given clientName
        var allCompanyIds = await _dbContext.Companies
            .Where(c => c.ClientName == clientName)
            .Select(c => c.Id)
            .ToListAsync()
            .ConfigureAwait(false);

        // Get companyIds from the provided DTOs
        var dtoCompanyIds = clientCompanyMappingDtos.Select(dto => dto.CompanyId).ToList();

        // Find companyIds to remove (all except those in DTOs)
        var companyIdsToRemove = allCompanyIds.Except(dtoCompanyIds).ToList();

        if (companyIdsToRemove.Any())
        {
            // Remove mappings for companyIdsToRemove from ClientCompaniesMappings
            var mappingsToRemove = await _dbContext.ClientCompaniesMappings
                .Where(ccm => companyIdsToRemove.Contains(ccm.CompanyId))
                .ToListAsync()
                .ConfigureAwait(false);

            if (mappingsToRemove.Any())
            {
                _dbContext.ClientCompaniesMappings.RemoveRange(mappingsToRemove);
            }

            // Update ClientName and ClientCode for companies to AppConstants.NOTAVAILABLE
            var companiesToUpdate = await _dbContext.Companies
                .Where(c => companyIdsToRemove.Contains(c.Id))
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var company in companiesToUpdate)
            {
                company.ClientName = AppConstants.NOTAVAILABLE;
                company.UpdatedAt = DateTime.UtcNow;

                // Update ClientCode in Clients table where CompanyId matches
                var client = await _dbContext.Clients.FirstOrDefaultAsync(cl => cl.Id == company.ClientId);
                if (client != null)
                {
                    client.ClientCode = $"DUMMYCMP-{Guid.NewGuid():N}".Substring(0, 10).ToUpper();//AppConstants.NOTAVAILABLE;
                    client.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        // Add new mappings from DTOs for the given clientId (if any)
        if (dtoCompanyIds.Any())
        {
            var existingMappingCompanyIds = await _dbContext.ClientCompaniesMappings
                .Where(ccm => ccm.ClientId == clientId && dtoCompanyIds.Contains(ccm.CompanyId))
                .Select(ccm => ccm.CompanyId)
                .ToListAsync()
                .ConfigureAwait(false);

            var newCompanyIds = dtoCompanyIds.Except(existingMappingCompanyIds).ToList();

            if (newCompanyIds.Any())
            {
                var utcNow = DateTime.UtcNow;
                var newMappings = newCompanyIds.Select(companyId => new ClientCompaniesMapping
                {
                    ClientId = clientId,
                    CompanyId = companyId,
                    CreatedAt = utcNow,
                    CreatedBy = userId,
                    UpdatedAt = utcNow,
                    UpdatedBy = userId
                }).ToList();

                await _dbContext.ClientCompaniesMappings.AddRangeAsync(newMappings).ConfigureAwait(false);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        // update clientName for dtoCompanyIds in companies table
        if (dtoCompanyIds.Any())
        {
            var companiesToUpdate = await _dbContext.Companies
                .Where(c => dtoCompanyIds.Contains(c.Id))
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var company in companiesToUpdate)
            {
                company.ClientName = clientName;
                company.UpdatedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        return true;

    }
    //public async Task<List<ClientCompanyMappingDto>> GetClientCompanyMappingByClientIdAsync(int clientId)
    //{
    //    // Step 1: Fetch clientName for the given clientId
    //    var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
    //    if (client == null)
    //        return new List<ClientCompanyMappingDto>();

    //    var clientName = client.ClientName;

    //    // Step 2: Fetch all clientIds with the same clientName
    //    var clientIds = await _dbContext.Clients
    //        .Where(c => c.ClientName == clientName)
    //        .Select(c => c.Id)
    //        .ToListAsync();

    //    if (!clientIds.Any())
    //        return new List<ClientCompanyMappingDto>();

    //    // Step 3: Fetch all companyIds from ClientCompaniesMappings for these clientIds
    //    var companyIds = await _dbContext.ClientCompaniesMappings
    //        .Where(m => clientIds.Contains(m.ClientId))
    //        .Select(m => m.CompanyId)
    //        .Distinct()
    //        .ToListAsync();

    //    if (!companyIds.Any())
    //        return new List<ClientCompanyMappingDto>();


    //    // Step 4: Fetch mappings and related company info with distinct CompanyNames
    //    var mappingCompanyJoin = await _dbContext.ClientCompaniesMappings
    //        .Where(m => companyIds.Contains(m.CompanyId))
    //        .Join(
    //            _dbContext.Companies,
    //            mapping => mapping.CompanyId,
    //            company => company.Id,
    //            (mapping, company) => new
    //            {
    //                mapping.Id,
    //                mapping.ClientId,
    //                mapping.CompanyId,
    //                company.CompanyName,
    //                company.AccountManagerId
    //            }
    //        )
    //        .ToListAsync();

    //    // Group by CompanyName and select the first entry for each group
    //    var distinctMappings = mappingCompanyJoin
    //        .GroupBy(x => x.CompanyName)
    //        .Select(g => g.First())
    //        .ToList();

    //    var clientCompanyMappings = new List<ClientCompanyMappingDto>();

    //    foreach (var joined in distinctMappings)
    //    {
    //        // Fetch all account managers for this company (RoleId == 2)
    //        var accountManagers = await (
    //                                from eu in _dbContext.ElixirUsers
    //                                join u in _dbContext.Users on eu.UserId equals u.Id
    //                                where eu.CompanyId == joined.CompanyId
    //                                    //&& eu.RoleId == (int)Roles.AccountManager
    //                                    && eu.UserGroupId == (int)UserGroupRoles.AccountManager
    //                                select (object)new
    //                                {
    //                                    u.Id,
    //                                    UserName = u.FirstName + " " + u.LastName
    //                                }
    //                            ).ToListAsync();

    //        clientCompanyMappings.Add(new ClientCompanyMappingDto
    //        {
    //            ClientCompanyMappingId = joined.Id,
    //            ClientId = joined.ClientId,
    //            CompanyId = joined.CompanyId,
    //            CompanyName = joined.CompanyName,
    //            Users = accountManagers
    //        });
    //    }
    //    return clientCompanyMappings;
    //}

    public async Task<List<ClientCompanyMappingDto>> GetClientCompanyMappingByClientIdAsync(int clientId)
    {
        // Step 1: Fetch clientName for the given clientId
        var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        if (client == null)
            return new List<ClientCompanyMappingDto>();

        var clientName = client.ClientName;

        // Step 2: Fetch all clientIds with the same clientName
        var clientIds = await _dbContext.Clients
            .Where(c => c.ClientName == clientName)
            .Select(c => c.Id)
            .ToListAsync();

        if (!clientIds.Any())
            return new List<ClientCompanyMappingDto>();

        // Step 3: Fetch all companyIds from ClientCompaniesMappings for these clientIds
        var companyIds = await _dbContext.ClientCompaniesMappings
            .Where(m => clientIds.Contains(m.ClientId))
            .Select(m => m.CompanyId)
            .Distinct()
            .ToListAsync();

        if (!companyIds.Any())
            return new List<ClientCompanyMappingDto>();


        // Step 4: Fetch mappings and related company info with distinct CompanyNames
        var mappingCompanyJoin = await _dbContext.ClientCompaniesMappings
            .Where(m => companyIds.Contains(m.CompanyId))
            .Join(
                _dbContext.Companies,
                mapping => mapping.CompanyId,
                company => company.Id,
                (mapping, company) => new
                {
                    mapping.Id,
                    mapping.ClientId,
                    mapping.CompanyId,
                    company.CompanyName,
                    company.AccountManagerId
                }
            )
            .ToListAsync();

        // Group by CompanyName and select the first entry for each group
        var distinctMappings = mappingCompanyJoin
            .GroupBy(x => x.CompanyName)
            .Select(g => g.First())
            .ToList();

        var clientCompanyMappings = new List<ClientCompanyMappingDto>();

        // PSEUDOCODE / PLAN:
        // 1. Gather distinctMappings as before (already computed above).
        // 2. Extract all distinct companyIds from distinctMappings.
        // 3. Query ElixirUsers join Users once for all those companyIds to get triples: CompanyId, UserId, UserName.
        // 4. Materialize the query to a list, then on the client side:
        //    a. Group results by CompanyId.
        //    b. Within each group, ensure uniqueness by UserId (i.e., remove duplicates).
        //    c. Project each user to an object matching the DTO's Users type (List<object>).
        // 5. Build clientCompanyMappings by iterating distinctMappings and attaching the corresponding unique Users list (empty if none).
        // 6. Return clientCompanyMappings.
        //
        // This replaces the previous per-company query and ensures Users lists contain unique user entries.

        foreach (var joined in distinctMappings)
        {
            // This foreach kept for parity; actual user fetching is done in bulk below.
            // (The original per-iteration query has been replaced by a bulk query implementation.)
        }

        // Bulk fetch account managers for all companies in distinctMappings to avoid N+1 and to deduplicate users
        var companyIdsForMappings = distinctMappings
            .Select(x => x.CompanyId)
            .Distinct()
            .ToList();

        clientCompanyMappings = new List<ClientCompanyMappingDto>();

        if (!companyIdsForMappings.Any())
            return clientCompanyMappings;

        // Fetch account managers for all relevant companies in a single query
        var accountManagersRaw = await (
            from eu in _dbContext.ElixirUsers
            join u in _dbContext.Users on eu.UserId equals u.Id
            where companyIdsForMappings.Contains(eu.CompanyId)
                  && eu.UserGroupId == (int)UserGroupRoles.AccountManager
            select new
            {
                eu.CompanyId,
                u.Id,
                UserName = (u.FirstName ?? "") + " " + (u.LastName ?? "")
            }
        ).ToListAsync().ConfigureAwait(false);

        // Group by company and ensure unique users per company (unique by user Id)
        var accountManagersByCompany = accountManagersRaw
            .GroupBy(a => a.CompanyId)
            .ToDictionary(
                g => g.Key,
                g => g
                    .GroupBy(x => x.Id)
                    .Select(xg => (object)new
                    {
                        Id = xg.Key,
                        UserName = xg.First().UserName
                    })
                    .ToList()
            );

        // Build the final DTO list using distinctMappings, attaching deduplicated Users
        foreach (var joined in distinctMappings)
        {
            accountManagersByCompany.TryGetValue(joined.CompanyId, out var accountManagers);

            clientCompanyMappings.Add(new ClientCompanyMappingDto
            {
                ClientCompanyMappingId = joined.Id,
                ClientId = joined.ClientId,
                CompanyId = joined.CompanyId,
                CompanyName = joined.CompanyName,
                Users = accountManagers ?? new List<object>()
            });
        }

        return clientCompanyMappings;
    }
    public async Task<bool> RemoveMappingsByCompanyNameAsync(string clientName)
    {
        // Step 1: Fetch all companyIds with the given companyName
        var companyIds = await _dbContext.Companies
            .Where(c => c.ClientName == clientName)
            .Select(c => c.Id)
            .ToListAsync();

        var clientIds = await _dbContext.Companies
            .Where(c => c.ClientName == clientName && c.ClientId.HasValue)
            .Select(c => c.ClientId.Value)
            .ToListAsync();

        // Update ClientName for matching records
        if (companyIds.Any())
        {
            // Update ClientName in Companies table
            await _dbContext.Companies
                .Where(c => companyIds.Contains(c.Id))
                .ForEachAsync(c => c.ClientName = AppConstants.NOTAVAILABLE);

            // Update ClientCode in Clients table for affected clientIds (from Companies.ClientId)
            if (clientIds.Any())
            {
                await _dbContext.Clients
                    .Where(cl => clientIds.Contains(cl.Id))
                    .ForEachAsync(cl => cl.ClientCode = $"DUMMYCMP-{Guid.NewGuid():N}".Substring(0, 10).ToUpper());
            }

            // Save changes to the database
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
        else
        {
            return true;
        }

        // Step 2: Fetch all mappings with those companyIds
        var mappingsToRemove = await _dbContext.ClientCompaniesMappings
            .Where(ccm => companyIds.Contains(ccm.CompanyId))
            .ToListAsync()
            .ConfigureAwait(false);

        if (!mappingsToRemove.Any())
            return true;

        // Step 3: Remove the mappings
        _dbContext.ClientCompaniesMappings.RemoveRange(mappingsToRemove);
        return await _dbContext.SaveChangesAsync().ConfigureAwait(false) > 0;
    } 
}
