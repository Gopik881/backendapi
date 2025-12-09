using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Enums;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class ClientsRepository : IClientsRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public ClientsRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> UpdateClientInformationAsync(ClientInfoDto clientInfoDto, int clientId, int userId)
    {
        // 1. Try to find existing client by clientId
        var existingClient = await _dbContext.Clients
            .FirstOrDefaultAsync(c => c.Id == clientId);

        if (existingClient == null)
        {
            // 2. Create new Client entity and set properties
            var newClient = new Client
            {
                StateId = clientInfoDto.StateId,
                CountryId = clientInfoDto.CountryId,
                ClientName = clientInfoDto.ClientName,
                ClientInfo = clientInfoDto.ClientInfo,
                IsEnabled = clientInfoDto.Status,
                //CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = userId,
                ClientCode = clientInfoDto.ClientCode,
                Address1 = clientInfoDto.Address1,
                Address2 = clientInfoDto.Address2,
                ZipCode = clientInfoDto.ZipCode,
                PhoneNumber = clientInfoDto.PhoneNumber
               
            };

            // Do not set Id manually for new entity (let DB generate it)
            await _dbContext.Clients.AddAsync(newClient);
        }
        else
        {
            // 3. Map properties from DTO for update
            existingClient.StateId = clientInfoDto.StateId;
            existingClient.CountryId = clientInfoDto.CountryId;
            existingClient.ClientName = clientInfoDto.ClientName;
            existingClient.ClientInfo = clientInfoDto.ClientInfo;
            // Do not update Id for existing entity
            existingClient.IsEnabled = clientInfoDto.Status;
            //existingClient.CreatedAt = DateTime.UtcNow;
            existingClient.UpdatedAt = DateTime.UtcNow;
            existingClient.UpdatedBy = userId;
            existingClient.ClientCode = clientInfoDto.ClientCode;
            existingClient.Address1 = clientInfoDto.Address1;
            existingClient.Address2 = clientInfoDto.Address2;
            existingClient.ZipCode = clientInfoDto.ZipCode;
            existingClient.PhoneNumber = clientInfoDto.PhoneNumber;

            _dbContext.Clients.Update(existingClient);
        }

        // 4. Save changes
        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<bool> ExistsWithClientNameAsync(string clientName)
    {
        return await _dbContext.Clients.AnyAsync(c => c.ClientName == clientName);
    }

    public async Task<bool> ExistsWithClientNameIdsAsync(string clientName, List<int>clientIds)
    {
        return await _dbContext.Clients.AnyAsync(c => c.ClientName == clientName && !clientIds.Contains(c.Id));
    }

    public async Task<bool> ExistsWithClientCodeAsync(string clientCode)
    {
        return await _dbContext.Clients.AnyAsync(c => c.ClientCode == clientCode && c.ClientName != AppConstants.NOTAVAILABLE);
    }
    public async Task<List<int>> GetDistinctClientIdsByCompanyIdsAsync(IEnumerable<int> companyIds)
    {
        return await _dbContext.Companies
            .Where(c => companyIds.Contains(c.Id) && c.ClientId.HasValue)
            .Select(c => c.ClientId.Value)
            .Distinct()
            .ToListAsync();
    }
    public async Task<int?> GetClientIdByClientNameAsync(string clientName)
    {
        return await _dbContext.Clients
            .Where(c => c.ClientName == clientName)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync();
    }
    public async Task<IEnumerable<ClientGroupswithAccountManagersDto>> GetClientGroupswithAccountManagersAsync()
    {
        // 1. Get the AccountManager group (by role id)
        var groupId = (int)UserGroupRoles.AccountManager;

        // 2. Get the group info
        var group = await _dbContext.UserGroups
            .Where(ug => ug.Id == groupId)
            .Select(ug => new UserGroupDto
            {
                GroupName = ug.GroupName,
                GroupId = ug.Id
            })
            .FirstOrDefaultAsync();

        // 3. Get all users in the AccountManager group
        var users = await (from u in _dbContext.Users
                           join ugm in _dbContext.UserGroupMappings on u.Id equals ugm.UserId
                           where ugm.UserGroupId == groupId && !u.IsDeleted && (u.IsEnabled ?? false)
                           select new CompanyUserDto
                           {
                               UserId = u.Id,
                               UserName = u.FirstName + " " + u.LastName,
                               Email = u.Email
                           })
                           .Distinct()
                           .ToListAsync();

        // 4. Return a single DTO (since only one group is relevant)
        return new List<ClientGroupswithAccountManagersDto>
        {
            new ClientGroupswithAccountManagersDto
            {
                clientUserGroups = group != null ? new List<UserGroupDto> { group } : new List<UserGroupDto>(),
                clientAccountManagers = users
            }
        };
    }
    public async Task<IEnumerable<ClientUnmappedCompaniesDto>> GetClientUnmappedCompaniesAsync()
    {
        var unmappedCompanies = await _dbContext.Companies
            .Where(c => !_dbContext.ClientCompaniesMappings.Any(ccm => ccm.CompanyId == c.Id)
                        && !string.IsNullOrEmpty(c.CompanyName) && !c.IsDeleted)
            .OrderBy(c => c.Id)
            .Skip(1)
            .Select(c => new ClientUnmappedCompaniesDto
            {
                CompanyId = c.Id,
                CompanyName = c.CompanyName,
                AccountManagers = (from am in _dbContext.ElixirUsers
                                   join u in _dbContext.Users on am.UserId equals u.Id
                                   where am.CompanyId == c.Id && am.UserGroupId == (int)UserGroupRoles.AccountManager && !u.IsDeleted && (u.IsEnabled ?? false)
                                   select new CompanyUserDto
                                   {
                                       UserId = u.Id,
                                       UserName = u.FirstName + " " + u.LastName
                                   }).ToList()
            })
            .ToListAsync();

        return unmappedCompanies;
    }
    public async Task<bool> DeleteClientAccountManagersAsync(int clientId)
    {
        var clientAccountManagers = await _dbContext.ElixirUsers
            .Where(a => a.ClientId == clientId && !a.IsDeleted)
            .ToListAsync();

        if (clientAccountManagers.Count == 0)
            return true;

        _dbContext.ElixirUsers.RemoveRange(clientAccountManagers);
        var changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<bool> DeleteClientInfoAsync(int clientId)
    {
        // 1. Get the client by id (not deleted)
        var client = await _dbContext.Clients
            .FirstOrDefaultAsync(c => c.Id == clientId && !c.IsDeleted);
        if (client == null)
            return false;
        // 2. Mark as deleted
        client.ClientName = AppConstants.NOTAVAILABLE;
        client.UpdatedAt = DateTime.UtcNow;
        _dbContext.Clients.Update(client);
        // 3. Save changes
        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<Client?> GetClientByIdAsync(int clientId)
    {
        return await _dbContext.Clients
            .FirstOrDefaultAsync(c => c.Id == clientId && !c.IsDeleted);
    }
    public async Task<bool> UpdateClientAsync(Client existingClient)
    {
        if (existingClient == null)
            return false;

        _dbContext.Clients.Update(existingClient);
        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<ClientInfoDto?> GetClientDetailsByIdAsync(int clientId)
    {
        return await _dbContext.Clients
            .Where(c => c.Id == clientId && !c.IsDeleted)
            .Select(c => new ClientInfoDto
            {
                ClientId = c.Id,
                ClientName = c.ClientName,
                ClientInfo = c.ClientInfo,
                Status = c.IsEnabled,
                ClientCode = c.ClientCode,
                CompanyId = c.Id,
                Address1 = c.Address1,
                Address2 = c.Address2,
                StateId = c.StateId,
                StateName = _dbContext.StateMasters.Where(sm => sm.Id == c.StateId).Select(sm => sm.StateName).FirstOrDefault(),
                CountryId = c.CountryId,
                CountryName = _dbContext.CountryMasters.Where(sm => sm.Id == c.CountryId).Select(sm => sm.CountryName).FirstOrDefault(),
                ZipCode = c.ZipCode,
                PhoneNumber = c.PhoneNumber,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedBy = c.UpdatedBy,
                UpdatedAt = c.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    // Pseudocode / Plan (detailed):
    // 1. Load client by id. If null return null.
    // 2. Get all client ids that share the same ClientName (and are not deleted).
    // 3. Compute the number of distinct companies for those clientIds by combining two sources:
    //    a) CompanyId values present in ElixirUsers for those clientIds (where ElixirUsers not deleted).
    //    b) CompanyId values present in ClientCompaniesMappings for those clientIds.
    //    Use a Union to combine both sets and CountAsync() distinct company ids.
    // 4. Compute number of distinct account managers (distinct UserId) from ElixirUsers for those clientIds.
    // 5. Compute account manager names by joining distinct UserIds to Users (filtering users not deleted and enabled).
    // 6. Map results into ClientPopupDetailsDto and return.
    //
    // This replaces the broken/invalid lines that attempted to use an undefined 'c' variable and
    // instead performs a correct, efficient server-side union of company ids.

    public async Task<ClientPopupDetailsDto?> GetClientDetailsAsync(int clientId)
    {
        // 1. Get client by id (not deleted)
        var client = await _dbContext.Clients
            .Where(c => c.Id == clientId && !c.IsDeleted)
            .FirstOrDefaultAsync();

        if (client == null)
            return null;

        // 2. Get all client ids with the same client name (not deleted)
        var clientIds = await _dbContext.Clients
            .Where(c => c.ClientName == client.ClientName && !c.IsDeleted)
            .Select(c => c.Id)
            .ToListAsync();

        // 3. Get number of companies for these client ids from ClientCompaniesMappings
        var companyIdsFromMappings = _dbContext.ClientCompaniesMappings
            .Where(ccm => clientIds.Contains(ccm.ClientId) /* consider adding && !ccm.IsDeleted */)
            .Select(ccm => ccm.CompanyId);

        var noOfCompanies = await companyIdsFromMappings
            .Distinct()
            .CountAsync();

        // 4. Get number of account managers for these client ids
        var noOfAccountManagers = await _dbContext.ElixirUsers
            .Where(eu => eu.ClientId.HasValue && clientIds.Contains(eu.ClientId.Value) && !eu.IsDeleted)
            .Select(eu => eu.UserId)
            .Distinct()
            .CountAsync();

        // 5. Get account manager names
        var accountManagers = await _dbContext.ElixirUsers
            .Where(eu => eu.ClientId.HasValue && clientIds.Contains(eu.ClientId.Value) && !eu.IsDeleted)
            .Select(eu => eu.UserId)
            .Distinct()
            .Join(_dbContext.Users.Where(u => !u.IsDeleted && (u.IsEnabled ?? false)),
                  userId => userId,
                  user => user.Id,
                  (userId, user) => user.FirstName + " " + user.LastName)
            .Distinct()
            .ToArrayAsync();

        // 6. Map to DTO (preserve original field names used in the repository)
        return new ClientPopupDetailsDto
        {
            ClientId = client.Id,
            ClineName = client.ClientName,
            NoOfCompanies = noOfCompanies,
            NoOfAccountManagers = noOfAccountManagers,
            AccountManagers = accountManagers,
            CreatedOn = client.CreatedAt,
            CreatedAt = client.CreatedAt,
            Status = client.IsEnabled
        };
    }


    public async Task<Tuple<List<ClientDto>, int>> GetFilteredClientsAsync(int pageNumber, int pageSize, string? searchTerm)
    {
        // Plan / Pseudocode (detailed):
        // 1. Build baseQuery same as before (exclude deleted and placeholder, keep Skip(1)).
        // 2. Materialize a small projection of clients: Id, ClientName, ClientCode, Status, CreatedOn (UpdatedAt ?? CreatedAt).
        // 3. Collect all clientIds from that projection.
        // 4. Fetch all ClientCompaniesMappings for those clientIds as list of (ClientId, CompanyId).
        // 5. Fetch all ElixirUsers for those clientIds as list of (ClientId, UserId) filtering out deleted.
        // 6. Group the clients in-memory by ClientName.
        //    For each group:
        //      a) Determine representative row (latest CreatedOn, then tie-breakers).
        //      b) Compute NoOfCompanies = distinct company ids across all clientIds in the group (use mappings list).
        //      c) Compute NoOfUsers = distinct user ids across all clientIds in the group (use elixir users list).
        //      d) Build ClientDto using representative's fields and aggregated counts.
        // 7. Apply search filter on the grouped DTOs (case-insensitive) matching ClientName, numbers, status text.
        // 8. totalCount = count after filtering.
        // 9. Order by CreatedOn desc then tie-breakers, apply pagination, return tuple.
        //
        // This ensures NoOfCompanies and NoOfUsers are aggregated across all clientIds that share the same ClientName,
        // avoiding the issue where picking a single client row yields 0 companies while the union across duplicates is non-zero.

        // 1. Base query (preserve Skip(1) like original)
        var baseQuery = _dbContext.Clients
                        .Where(c => !c.IsDeleted && c.ClientName != AppConstants.NOTAVAILABLE)
                        .OrderBy(c => c.Id)
                        .Skip(1);

        // 2. Materialize small projection
        var clientProjections = await baseQuery.Select(c => new
        {
            c.Id,
            c.ClientName,
            c.ClientCode,
            Status = c.IsEnabled,
            CreatedOn = c.UpdatedAt ?? c.CreatedAt
        }).ToListAsync();

        var clientIds = clientProjections.Select(c => c.Id).ToList();

        if (!clientIds.Any())
            return new Tuple<List<ClientDto>, int>(new List<ClientDto>(), 0);

        // 4. Fetch all company mappings for these client ids
        var companyMappings = await _dbContext.ClientCompaniesMappings
            .Where(ccm => clientIds.Contains(ccm.ClientId))
            .Select(ccm => new { ccm.ClientId, ccm.CompanyId })
            .ToListAsync();

        // 5. Fetch all elixir user mappings for these client ids (exclude deleted elixir users)
        var elixirUserMappings = await _dbContext.ElixirUsers
            .Where(eu => eu.ClientId.HasValue && clientIds.Contains(eu.ClientId.Value) && !eu.IsDeleted)
            .Select(eu => new { ClientId = eu.ClientId!.Value, eu.UserId })
            .ToListAsync();

        // 6. Group by ClientName and compute aggregated counts per group
        var groupedClients = clientProjections
            .GroupBy(c => c.ClientName)
            .Select(g =>
            {
                var groupClientIds = g.Select(x => x.Id).ToList();

                // Representative row: latest CreatedOn, then by Id to be deterministic
                var rep = g
                    .OrderByDescending(x => x.CreatedOn)
                    .ThenBy(x => x.Id)
                    .First();

                // Distinct company ids across all clientIds in this group
                var companyCount = companyMappings
                    .Where(cm => groupClientIds.Contains(cm.ClientId))
                    .Select(cm => cm.CompanyId)
                    .Distinct()
                    .Count();

                // Distinct user ids across all clientIds in this group
                var userCount = elixirUserMappings
                    .Where(em => groupClientIds.Contains(em.ClientId))
                    .Select(em => em.UserId)
                    .Distinct()
                    .Count();

                return new ClientDto
                {
                    Id = rep.Id,
                    ClientName = g.Key,
                    ClientCode = rep.ClientCode,
                    NoOfCompanies = companyCount,
                    NoOfUsers = userCount,
                    Status = rep.Status,
                    CreatedOn = rep.CreatedOn
                };
            })
            .ToList();

        // 7. Apply search filter (case-insensitive) on final DTO list
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearch = searchTerm.ToLowerInvariant();
            groupedClients = groupedClients.Where(c =>
                (!string.IsNullOrEmpty(c.ClientName) && c.ClientName.ToLowerInvariant().Contains(lowerSearch)) ||
                c.NoOfCompanies.ToString().Contains(lowerSearch) ||
                c.NoOfUsers.ToString().Contains(lowerSearch) ||
                ((!string.IsNullOrEmpty(lowerSearch)) && (
                     (c.Status == true && "enabled".Contains(lowerSearch, StringComparison.OrdinalIgnoreCase)) ||
                     (c.Status == false && "disabled".Contains(lowerSearch, StringComparison.OrdinalIgnoreCase))
                ))
            ).ToList();
        }

        // 8. Total count after grouping & filtering
        var totalCount = groupedClients.Count;

        // 9. Order by latest first and apply pagination
        var pagedClients = groupedClients
            .OrderByDescending(c => c.CreatedOn) // latest records at the top
            .ThenByDescending(c => c.NoOfCompanies)
            .ThenByDescending(c => c.NoOfUsers)
            .ThenBy(c => c.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new Tuple<List<ClientDto>, int>(pagedClients, totalCount);
    }


    public async Task<Tuple<List<ClientCompanyDto>, int>> GetFilteredClientCompaniesAsync(int clientId, int pageNumber, int pageSize, string? searchTerm)
    {
        // 1. Get base query for companies mapped to the client
        var query = from mapping in _dbContext.ClientCompaniesMappings
                    join company in _dbContext.Companies on mapping.CompanyId equals company.Id
                    join client in _dbContext.Clients on mapping.ClientId equals client.Id
                    where mapping.ClientId == clientId && !company.IsDeleted && !client.IsDeleted
                    select new { client.ClientName, company };

        //// 2. Apply search filter if provided (on company name or code)
        //if (!string.IsNullOrWhiteSpace(searchTerm))
        //{
        //    query = query.Where(x =>
        //        x.company.CompanyName.Contains(searchTerm) ||
        //        (x.company.ZipCode != null && x.company.ZipCode.Contains(searchTerm)) ||
        //        (x.company.PhoneNumber != null && x.company.PhoneNumber.Contains(searchTerm))
        //    );
        //}

        // 3. Get total count before pagination
        //var totalCount = await query.CountAsync();

        // 4. Apply pagination and projection
        var grouped = await query
            .OrderBy(x => x.company.Id)
            //.Skip((pageNumber - 1) * pageSize)
            //.Take(pageSize)
            .GroupBy(x => x.ClientName)
            .Select(g => new ClientCompanyDto
            {
                ClientName = g.Key,
                CreatedOn = g.Min(x => x.company.CreatedAt),
                Companies = g.Select(x => new CompanyDto
                {
                    CompanyId = x.company.Id,
                    CompanyName = x.company.CompanyName,
                    ZipCode = x.company.ZipCode,
                    PhoneNumber = x.company.PhoneNumber
                }).ToList()
            })
            .ToListAsync();

        // Additional: Apply searchTerm logic on all columns of the DTO
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearch = searchTerm.ToLowerInvariant();
            grouped = grouped.Where(dto =>
                (!string.IsNullOrEmpty(dto.ClientName) && dto.ClientName.ToLowerInvariant().Contains(lowerSearch)) ||
                //(dto.CreatedOn != null && dto.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(lowerSearch)) ||
                (dto.Companies != null && dto.Companies.Any(company =>
                    company.CompanyId.ToString().Contains(lowerSearch) ||
                    (!string.IsNullOrEmpty(company.CompanyName) && company.CompanyName.ToLowerInvariant().Contains(lowerSearch)) //||
                    //(!string.IsNullOrEmpty(company.ZipCode) && company.ZipCode.ToLowerInvariant().Contains(lowerSearch)) ||
                    //(!string.IsNullOrEmpty(company.PhoneNumber) && company.PhoneNumber.ToLowerInvariant().Contains(lowerSearch))
                ))
            ).ToList();
            
        }
        var totalCount = grouped.Count();
        grouped = grouped
            .OrderBy(dto => dto.ClientName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        
        return new Tuple<List<ClientCompanyDto>, int>(grouped, totalCount);
    }

    public async Task<List<int>> GetListOfClientIdsByCompanyNameAsync(string clientName)
    {
        return await _dbContext.Clients
            .Where(c => c.ClientName == clientName)
            .Select(c => c.Id)
            .ToListAsync();
    }

    public async Task<bool> ExistsWithUpdateClientCodeAsync(string clientCode, List<int> ClientIds)
    {
        return await _dbContext.Clients.AnyAsync(c => c.ClientCode == clientCode && !ClientIds.Contains(c.Id) && c.ClientName != AppConstants.NOTAVAILABLE);
    }

    public async Task<Tuple<List<ClientBasicInfoDto>, int>> GetFilteredClientsByUsersAsync(int userId, int groupId, string groupName, string searchTerm, int pageNumber, int pageSize)
    {
        // Fetch clients mapped to the user based on groupId (AccountManager only)
        var query = _dbContext.ElixirUsers
            .Where(eu => eu.UserId == userId && !eu.IsDeleted &&
                eu.UserGroupId == (int)UserGroupRoles.AccountManager
                && eu.ClientId != null
            )
            .Join(_dbContext.Clients.Where(cl => !cl.IsDeleted),
                eu => eu.ClientId,
                cl => cl.Id,
                (eu, cl) => new ClientBasicInfoDto
                {
                    //ClientId = cl.Id,
                    ClientName = cl.ClientName ?? string.Empty,
                    //ClientCode = cl.ClientCode ?? string.Empty,
                    //CreatedOn = cl.CreatedAt
                })
            .Distinct();

        var clients = await query.ToListAsync();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            clients = clients
                .Where(c =>
                    (!string.IsNullOrEmpty(c.ClientName) && c.ClientName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                //(!string.IsNullOrEmpty(c.ClientCode) && c.ClientCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                //(c.CreatedOn.HasValue && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                //c.ClientId.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                )
                .ToList();
        }

        var totalCount = clients.Count;
        clients = clients
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new Tuple<List<ClientBasicInfoDto>, int>(clients, totalCount);
    }

    public async Task<string?> GetCompanyNameByClientCodeAsync(string clientCode)
    {
        // Assuming _dbContext is your EF Core DbContext and has DbSet<Company> and DbSet<CompanyHistory>
        var company = await _dbContext.Companies
            .Where(c => c.CompanyCode == clientCode)
            .Select(c => c.CompanyName)
            .FirstOrDefaultAsync();

        if (!string.IsNullOrEmpty(company))
            return company;

        var companyHistory = await _dbContext.CompanyHistories
            .Where(ch => ch.CompanyCode == clientCode)
            .Select(ch => ch.CompanyName)
            .FirstOrDefaultAsync();

        return companyHistory;
    }
}

