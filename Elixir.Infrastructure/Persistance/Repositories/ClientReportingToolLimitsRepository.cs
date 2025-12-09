using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class ClientReportingToolLimitsRepository : IClientReportingToolLimitsRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public ClientReportingToolLimitsRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> CreateClientReportingToolLimitDataAsync(int userId, int clientId, ReportingToolLimitsDto clientReportingToolLimitDto)
    {
        // 1. Get the next ClientReportingToolLimit Id
        int nextId = (await _dbContext.ClientReportingToolLimits
            .Select(c => (int?)c.Id)
            .MaxAsync()) ?? 0;
        nextId += 1;

        // 2. Map DTO to entity and set audit fields
        var clientReportingToolLimit = new ClientReportingToolLimit
        {
            ClientReportingAdmins = clientReportingToolLimitDto.ClientReportingAdmins,
            ClientCustomerReportCreators = clientReportingToolLimitDto.ClientCustomerReportCreators,
            ClientDashboardLibrary = clientReportingToolLimitDto.ClientDashboardLibrary,
            ClientDashboardPersonalLibrary = clientReportingToolLimitDto.ClientDashboardPersonalLibrary,
            ClientSavedReportQueriesPerUser = clientReportingToolLimitDto.ClientSavedReportQueriesPerUser,
            ClientSavedReportQueriesLibrary = clientReportingToolLimitDto.ClientSavedReportQueriesLibrary,
            CreatedBy = userId,
            UpdatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ClientId = clientId,
        };

        // 3. Add and save
        await _dbContext.ClientReportingToolLimits.AddAsync(clientReportingToolLimit);
        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<bool> DeleteClientReportingToolLimitsAsync(int clientId)
    {
        var reportingToolLimits = await _dbContext.ClientReportingToolLimits
            .Where(r => r.ClientId == clientId)
            .ToListAsync();

        if (!reportingToolLimits.Any())
            return true;

        _dbContext.ClientReportingToolLimits.RemoveRange(reportingToolLimits);
        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<bool> UpdateClientReportingToolLimitDataAsync(int userId, int clientId, ReportingToolLimitsDto clientReportingToolLimitDto)
    {
        var existingLimit = await _dbContext.ClientReportingToolLimits
            .FirstOrDefaultAsync(crtl => crtl.ClientId == clientId);

        if (clientReportingToolLimitDto == null)
        {
            return false;
        }

        if (existingLimit != null)
        {
            existingLimit.ClientReportingAdmins = clientReportingToolLimitDto.ClientReportingAdmins;
            existingLimit.ClientCustomerReportCreators = clientReportingToolLimitDto.ClientCustomerReportCreators;
            existingLimit.ClientDashboardLibrary = clientReportingToolLimitDto.ClientDashboardLibrary;
            existingLimit.ClientDashboardPersonalLibrary = clientReportingToolLimitDto.ClientDashboardPersonalLibrary;
            existingLimit.ClientSavedReportQueriesPerUser = clientReportingToolLimitDto.ClientSavedReportQueriesPerUser;
            existingLimit.ClientSavedReportQueriesLibrary = clientReportingToolLimitDto.ClientSavedReportQueriesLibrary;
            existingLimit.UpdatedAt = DateTime.UtcNow;
            existingLimit.UpdatedBy = userId;

            _dbContext.ClientReportingToolLimits.Update(existingLimit);
        }
        else
        {
            var newLimit = new ClientReportingToolLimit
            {
                // Do NOT set Id here, let the database generate it
                ClientId = clientId,
                ClientReportingAdmins = clientReportingToolLimitDto.ClientReportingAdmins,
                ClientCustomerReportCreators = clientReportingToolLimitDto.ClientCustomerReportCreators,
                ClientDashboardLibrary = clientReportingToolLimitDto.ClientDashboardLibrary,
                ClientDashboardPersonalLibrary = clientReportingToolLimitDto.ClientDashboardPersonalLibrary,
                ClientSavedReportQueriesPerUser = clientReportingToolLimitDto.ClientSavedReportQueriesPerUser,
                ClientSavedReportQueriesLibrary = clientReportingToolLimitDto.ClientSavedReportQueriesLibrary,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = userId
            };

            await _dbContext.ClientReportingToolLimits.AddAsync(newLimit);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<ReportingToolLimitsDto?> GetClientReportingToolLimitDataAsync(int clientId)
    {
        return await _dbContext.ClientReportingToolLimits
            .Where(r => r.ClientId == clientId)
            .Select(r => new ReportingToolLimitsDto
            {
                ClientReportingAdmins = r.ClientReportingAdmins,
                ClientCustomerReportCreators = r.ClientCustomerReportCreators,
                ClientSavedReportQueriesLibrary = r.ClientSavedReportQueriesLibrary,
                ClientSavedReportQueriesPerUser = r.ClientSavedReportQueriesPerUser,
                ClientDashboardLibrary = r.ClientDashboardLibrary,
                ClientDashboardPersonalLibrary = r.ClientDashboardPersonalLibrary,
                CreatedBy = r.CreatedBy,
                UpdatedBy = r.UpdatedBy,
                // Assuming CreatedAt and UpdatedAt exist in the entity, otherwise remove these lines
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                ClientId = r.ClientId ?? 0
            })
            .FirstOrDefaultAsync();
    }
}
