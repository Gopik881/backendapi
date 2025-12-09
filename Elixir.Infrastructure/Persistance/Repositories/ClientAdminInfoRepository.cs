using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class ClientAdminInfoRepository : IClientAdminInfoRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public ClientAdminInfoRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<bool> CreateClientAdminInfoDataAsync(int userId, int clientId, ClientAdminInfoDto clientAdminInfoDto)
    {
        // 1. Get the next ClientAdminId by finding the max existing value and incrementing it.
        int nextId = (await _dbContext.ClientAdminInfos
            .Select(c => (int?)c.Id)
            .MaxAsync()) ?? 0;
        nextId += 1;

        // 2. Remove all existing ClientAdminInfo records for this client.
        var existingClientAdminDetails = await _dbContext.ClientAdminInfos
            .Where(ca => ca.ClientId == clientId)
            .ToListAsync();

        if (existingClientAdminDetails.Any())
        {
            _dbContext.ClientAdminInfos.RemoveRange(existingClientAdminDetails);
            await _dbContext.SaveChangesAsync();
        }

        // 3. Create new ClientAdminInfo entity and map properties.
        var clientAdminInfo = new ClientAdminInfo
        {
            // Uncomment if ClientAdminId is a property in the model.
            // ClientAdminId = nextId,
            FirstName = clientAdminInfoDto.FirstName,
            LastName = clientAdminInfoDto.LastName,
            PhoneNumber = clientAdminInfoDto.PhoneNumber,
            Email = clientAdminInfoDto.Email,
            Designation = clientAdminInfoDto.Designation,
            CreatedBy = userId,
            UpdatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ClientId = clientId
        };

        await _dbContext.ClientAdminInfos.AddAsync(clientAdminInfo);
        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<bool> DeleteClientAdminInfosAsync(int clientId)
    {
        var clientAdminInfos = await _dbContext.ClientAdminInfos
            .Where(a => a.ClientId == clientId)
            .ToListAsync();

        if (!clientAdminInfos.Any())
            return true;

        _dbContext.ClientAdminInfos.RemoveRange(clientAdminInfos);
        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<bool> UpdateClientAdminInfoDataAsync(int userId, int clientId, ClientAdminInfoDto clientAdminInfoDto)
    {
        var existingAdminInfo = await _dbContext.ClientAdminInfos
            .FirstOrDefaultAsync(ca => ca.ClientId == clientId);

        if (existingAdminInfo is not null)
        {
            existingAdminInfo.FirstName = clientAdminInfoDto.FirstName;
            existingAdminInfo.LastName = clientAdminInfoDto.LastName;
            existingAdminInfo.PhoneNumber = clientAdminInfoDto.PhoneNumber;
            existingAdminInfo.Email = clientAdminInfoDto.Email;
            existingAdminInfo.Designation = clientAdminInfoDto.Designation;
            existingAdminInfo.UpdatedAt = DateTime.UtcNow;
            existingAdminInfo.UpdatedBy = userId;
        }
        else
        {
            var newAdminInfo = new ClientAdminInfo
            {
                ClientId = clientId,
                FirstName = clientAdminInfoDto.FirstName,
                LastName = clientAdminInfoDto.LastName,
                PhoneNumber = clientAdminInfoDto.PhoneNumber,
                Email = clientAdminInfoDto.Email,
                Designation = clientAdminInfoDto.Designation,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = userId
            };

            await _dbContext.ClientAdminInfos.AddAsync(newAdminInfo);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<ClientAdminInfoDto?> GetClientAdminInfoByClientIdAsync(int clientId)
    {
        return await _dbContext.ClientAdminInfos
            .Where(a => a.ClientId == clientId)
            .Select(a => new ClientAdminInfoDto
            {
                FirstName = a.FirstName,
                LastName = a.LastName,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email,
                Designation = a.Designation,
                CreatedBy = a.CreatedBy,
                UpdatedBy = a.UpdatedBy,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                ClientId = a.ClientId ?? 0
            })
            .FirstOrDefaultAsync();
    }
}
