using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class ClientContactDetailsRepository : IClientContactDetailsRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public ClientContactDetailsRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> CreateClientContactDataAsync(int userId, int clientId, List<ClientContactInfoDto> clientContactInfoDtos)
    {
        // Remove existing ClientContactDetails for the client
        var existingContactDetails = await _dbContext.ClientContactDetails
            .Where(ccd => ccd.ClientId == clientId)
            .ToListAsync();

        if (existingContactDetails.Any())
        {
            _dbContext.ClientContactDetails.RemoveRange(existingContactDetails);
            await _dbContext.SaveChangesAsync();
        }

        // Add new client contact details
        var newContactDetails = clientContactInfoDtos.Select(ccd => new ClientContactDetail
        {
            ClientId = clientId,
            FirstName = ccd.FirstName,
            LastName = ccd.LastName,
            PhoneNumber = ccd.PhoneNumber,
            Email = ccd.Email,
            Designation = ccd.Designation,
            Department = ccd.Department,
            Remarks = ccd.Remarks,
            CreatedBy = Convert.ToInt32(userId),
            UpdatedBy = Convert.ToInt32(userId),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        }).ToList();

        if (newContactDetails.Any())
        {
            await _dbContext.ClientContactDetails.AddRangeAsync(newContactDetails);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        return true;
    }
    public async Task<bool> DeleteClientContactDetailsAsync(int clientId)
    {
        var clientContactDetails = await _dbContext.ClientContactDetails
            .Where(c => c.ClientId == clientId)
            .ToListAsync();

        if (!clientContactDetails.Any())
            return true;

        _dbContext.ClientContactDetails.RemoveRange(clientContactDetails);
        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<bool> UpdateClientContactDataAsync(int userId, int clientId, List<ClientContactInfoDto> clientContactInfoDtos)
    {
        // Fetch existing contact details for the client
        var existingContactDetails = await _dbContext.ClientContactDetails
            .Where(ccd => ccd.ClientId == clientId)
            .ToListAsync();

        // Remove all existing contact details
        if (existingContactDetails.Any())
        {
            _dbContext.ClientContactDetails.RemoveRange(existingContactDetails);
            await _dbContext.SaveChangesAsync();
        }

        // Prepare new/updated contact details
        var newContactDetails = clientContactInfoDtos.Select(dto => new ClientContactDetail
        {
            ClientId = clientId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            Designation = dto.Designation,
            Department = dto.Department,
            Remarks = dto.Remarks,
            CreatedBy = userId,
            UpdatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToList();

        if (newContactDetails.Any())
        {
            await _dbContext.ClientContactDetails.AddRangeAsync(newContactDetails);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        return true;
    }
    public async Task<List<ClientContactInfoDto>> GetClientContactDataByClientIdAsync(int clientId)
    {
        return await _dbContext.ClientContactDetails
            .Where(c => c.ClientId == clientId)
            .Select(c => new ClientContactInfoDto
            {
                ClientContactId = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                PhoneNumber = c.PhoneNumber,
                Email = c.Email,
                Designation = c.Designation,
                Department = c.Department,
                Remarks = c.Remarks,
                CreatedBy = c.CreatedBy,
                UpdatedBy = c.UpdatedBy,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                ClientId = c.ClientId ?? 0
            })
            .ToListAsync();
    }
}
