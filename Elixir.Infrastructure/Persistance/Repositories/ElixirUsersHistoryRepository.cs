using Elixir.Application.Common.Enums;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.Company5TabHistoryMapEntityToDictionary;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class ElixirUsersHistoryRepository : IElixirUsersHistoryRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public ElixirUsersHistoryRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Company5TabCreateElixirUserDataAsync(int companyId, List<Company5TabElixirUserDto> company5TabElixirUser, int userId, CancellationToken cancellationToken = default)
    {
        int lastVersion = _dbContext.ElixirUsersHistories
            .Where(uh => uh.CompanyId == companyId && !uh.IsDeleted)
            .OrderByDescending(uh => uh.Version)
            .Select(uh => uh.Version)
            .FirstOrDefault() ?? 0;

        if(company5TabElixirUser.Count == 0)
        {
            var emptyContacts = new ElixirUsersHistory
            {
                CompanyId = companyId,
                UserGroupId = 3,
                UserId = userId,
                RoleId = (int)Roles.SuperAdmin,
                Version = lastVersion + 1
            };
            _dbContext.ElixirUsersHistories.AddRange(emptyContacts);
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }
        var newContacts = company5TabElixirUser.Select(ec => new ElixirUsersHistory
        {
            //Client Id cannot be added at this point
            CompanyId = companyId,
            UserGroupId = ec.GroupId,
            UserId = ec.UserId,
            RoleId = (int)Roles.DelegateAdmin, //ByDefault the Elixir User will be created with DelegateAdmin Role, once he is mapped the Role will change
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            Version = lastVersion + 1
        }).ToList();

        _dbContext.ElixirUsersHistories.AddRange(newContacts);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<List<Company5TabElixirUserDto>> GetCompany5TabLatestElixirUsersHistoryAsync(int companyId, bool isPrevious, CancellationToken cancellationToken = default)
    {
        int? latestVersion = 0;
        var versionQuery = _dbContext.ElixirUsersHistories
            .Where(e => e.CompanyId == companyId && !e.IsDeleted);
        
        if (await versionQuery.AnyAsync(cancellationToken))
        {
            latestVersion = isPrevious
                ? await versionQuery
                    .Select(e => e.Version)
                    .Distinct()
                    .OrderByDescending(v => v)
                    .Skip(1)
                    .FirstOrDefaultAsync(cancellationToken)
                : await versionQuery
                    .Select(e => e.Version)
                    .Distinct()
                    .MaxAsync(cancellationToken);
        }

        var usersList = await _dbContext.ElixirUsersHistories
            .Where(eu => eu.CompanyId == companyId && !eu.IsDeleted && eu.RoleId != (int)Roles.SuperAdmin && eu.Version == latestVersion)
            .OrderByDescending(eu => eu.Version)
            .Join(_dbContext.Users, eu => eu.UserId, u => u.Id, (eu, u) => new { eu, u })
            .Join(_dbContext.UserGroups, eau => eau.eu.UserGroupId, ug => ug.Id, (eau, ug) => new Company5TabElixirUserDto
            {
                GroupId = eau.eu.UserGroupId,
                GroupName = ug.GroupName,
                UserId = eau.eu.UserId,
                FirstName = eau.u.FirstName,
                createdBy = eau.eu.CreatedBy,
                CreatedAt = eau.eu.CreatedAt,
                Email = eau.u.Email
            })
            .ToListAsync(cancellationToken);

        return usersList;
    }
    public async Task<bool> WithdrawCompany5TabElixirUsersHistoryAsync(int companyId, int userId, CancellationToken cancellationToken = default)
    {
        // Get the latest version for the given company and user
        int? latestVersion = await _dbContext.ElixirUsersHistories
            .Where(e => e.CompanyId == companyId)
            .MaxAsync(e => (int?)e.Version, cancellationToken);

        if (latestVersion == null) return true;

        // Find all records to remove
        var recordsToRemove = _dbContext.ElixirUsersHistories
            .Where(e => e.CompanyId == companyId && e.Version == latestVersion);
        if(recordsToRemove.Count() == 0) return true; // No records to remove
        _dbContext.ElixirUsersHistories.RemoveRange(recordsToRemove);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
    public async Task<Company5TabHistoryDto> GetCompany5TabElixirUsersHistoryByVersionAsync(
        int userId,
        int companyId,
        int versionNumber)
    {
        // Fetch the two most recent versions: requested and previous
        var histories = await _dbContext.ElixirUsersHistories
            .Where(e => e.CompanyId == companyId && (e.Version == versionNumber || e.Version == versionNumber - 1))
            .OrderByDescending(e => e.Version)
            .ToListAsync();

        Company5TabHistoryMapEntitytoDictionary company5TabHistoryMapEntitytoDictionary = new Company5TabHistoryMapEntitytoDictionary();

        var company5TabElixirUsersHistory = new Company5TabHistoryDto();

        foreach (var history in histories)
        {
            var mapped = company5TabHistoryMapEntitytoDictionary.MapEntityToDictionary(history)
                .ToDictionary(
                    keyValue => keyValue.Key,
                    keyValue => (object)new Dictionary<string, string> { { keyValue.Key, keyValue.Value } }
                );
            company5TabElixirUsersHistory.Company5TabHistory[history.Version.ToString()] = mapped;
        }

        return company5TabElixirUsersHistory;
    }
    public async Task<bool> CreateClientAccountManagerDataAsync(int userId, int clientId, List<ClientAccountManagersDto> clientAccountManagers)
    {
        // Remove existing ElixirUsers for the client
        var existingClientAMDetails = await _dbContext.ElixirUsers
            .Where(eu => eu.ClientId == clientId)
            .ToListAsync();

        if (existingClientAMDetails.Any())
        {
            _dbContext.ElixirUsers.RemoveRange(existingClientAMDetails);
            await _dbContext.SaveChangesAsync();
        }

        // Add new client account managers
        var elixirClientAccountManagers = clientAccountManagers.Select(ec => new ElixirUser
        {
            ClientId = clientId,
            UserGroupId = ec.GroupId,
            UserId = ec.UserId,
            CompanyId = clientId
        }).ToList();

        if (elixirClientAccountManagers.Any())
        {
            await _dbContext.ElixirUsers.AddRangeAsync(elixirClientAccountManagers);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        return true;
    }
    public async Task<bool> UpdateClientAccountManagerDataAsync(int userId, int clientId, List<ClientAccountManagersDto> clientAccountManagers)
    {
        // Remove all existing mappings for the client
        var existingMappings = await _dbContext.ElixirUsers
            .Where(ccm => ccm.ClientId == clientId)
            .ToListAsync();

        if (existingMappings.Count > 0)
        {
            _dbContext.ElixirUsers.RemoveRange(existingMappings);
            await _dbContext.SaveChangesAsync();
        }

        if (clientAccountManagers == null || clientAccountManagers.Count == 0)
            return true;

        var newManagers = clientAccountManagers.Select(manager => new ElixirUser
        {
            ClientId = clientId,
            UserGroupId = manager.GroupId,
            UserId = manager.UserId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = userId,
            CompanyId = clientId
        }).ToList();

        await _dbContext.ElixirUsers.AddRangeAsync(newManagers);
        return await _dbContext.SaveChangesAsync() > 0;
    }
    

}
