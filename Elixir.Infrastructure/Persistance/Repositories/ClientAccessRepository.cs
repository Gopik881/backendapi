using Elixir.Infrastructure.Data;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Clients.DTOs;
using Microsoft.EntityFrameworkCore;
using Elixir.Domain.Entities;
using Elixir.Application.Common.Constants;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class ClientAccessRepository : IClientAccessRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public ClientAccessRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> CreateClientAccessDataAsync(int userId, int clientId, ClientAccessDto clientAccessDto)
    {
        // Pseudocode plan:
        // 1. Retrieve claims from HttpContext and extract userId (prefer JwtRegisteredClaimNames.Sub, fallback to ClaimTypes.NameIdentifier).
        // 2. Get the next ClientAccessId by finding the max existing value and incrementing it.
        // 3. Create a new ClientAccess entity, mapping all relevant properties from the DTO and setting audit fields.
        // 4. Add the new entity to the DbContext and save changes.

       
        int nextId = (await _dbContext.ClientAccesses
            .Select(c => (int?)c.Id)
            .MaxAsync()) ?? 0;
        nextId += 1;

        var clientAccess = new ClientAccess
        {
            EnableWebQuery = clientAccessDto.EnableWebQuery,
            EnableReportAccess = clientAccessDto.EnableWebReportAccess,
            ClientUserLimit = clientAccessDto.ClientUserLimit,
            CreatedBy = userId,
            UpdatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ClientId = clientId,
        };

        await _dbContext.ClientAccesses.AddAsync(clientAccess);
        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<bool> DeleteClientAccessAsync(int clientId)
    {
        var clientAccess = await _dbContext.ClientAccesses
            .FirstOrDefaultAsync(a => a.ClientId == clientId);

        if (clientAccess is null)
            return true;

        _dbContext.ClientAccesses.Remove(clientAccess);
        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<bool> GetClientByCompanyIdAsync(int clientId)
    {
        // Pseudocode plan:
        // 1. Use async/await for all database operations.
        // 2. Use FirstOrDefaultAsync for async DB access.
        // 3. Only update and save if the entity exists and the value actually changes.
        // 4. Return true if update is successful, false otherwise.

        var existingClientDetails = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        if (existingClientDetails == null)
            return false;

        if (existingClientDetails.ClientName != AppConstants.NOTAVAILABLE)
        {
            existingClientDetails.ClientName = AppConstants.NOTAVAILABLE;
            _dbContext.Clients.Update(existingClientDetails);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        return false;
    }
    public async Task<bool> UpdateClientAccessAsync(ClientAccessDto clientAccessDto, int clientId, int userId)
    {
        // 1. Retrieve the existing ClientAccess entity for the given clientId.
        // 2. If found, update its properties and audit fields.
        // 3. If not found, create a new ClientAccess entity with the provided data (do NOT set Id explicitly).
        // 4. Save changes and return true if any changes were made.

        var existingClientAccess = await _dbContext.ClientAccesses
            .FirstOrDefaultAsync(ca => ca.ClientId == clientId);

        if (existingClientAccess != null)
        {
            existingClientAccess.EnableWebQuery = clientAccessDto.EnableWebQuery;
            existingClientAccess.EnableReportAccess = clientAccessDto.EnableWebReportAccess;
            existingClientAccess.ClientUserLimit = clientAccessDto.ClientUserLimit;
            existingClientAccess.UpdatedAt = DateTime.UtcNow;
            existingClientAccess.UpdatedBy = userId;

            _dbContext.ClientAccesses.Update(existingClientAccess);
        }
        else
        {
            var newClientAccess = new ClientAccess
            {
                // Do NOT set Id here; let the database auto-generate it
                ClientId = clientId,
                EnableWebQuery = clientAccessDto.EnableWebQuery,
                EnableReportAccess = clientAccessDto.EnableWebReportAccess,
                ClientUserLimit = clientAccessDto.ClientUserLimit,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = userId
            };

            await _dbContext.ClientAccesses.AddAsync(newClientAccess);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<ClientAccessDto?> GetClientAccessByClientIdAsync(int clientId)
    {
        return await _dbContext.ClientAccesses
            .Where(a => a.ClientId == clientId)
            .Select(a => new ClientAccessDto
            {
                EnableWebQuery = a.EnableWebQuery,
                EnableWebReportAccess = a.EnableReportAccess,
                ClientUserLimit = a.ClientUserLimit,
                CreatedBy = a.CreatedBy,
                UpdatedBy = a.UpdatedBy,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                ClientId = a.ClientId
            })
            .FirstOrDefaultAsync();
    }
}
