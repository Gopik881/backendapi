using Elixir.Application.Features.Clients.DTOs;

namespace Elixir.Application.Interfaces.Persistance;
public interface IClientContactDetailsRepository
{
    Task<bool> CreateClientContactDataAsync(int userId, int clientId, List<ClientContactInfoDto> clientContactInfoDtos);
    Task<bool> DeleteClientContactDetailsAsync(int clientId);
    Task<bool> UpdateClientContactDataAsync(int userId, int clientId, List<ClientContactInfoDto> clientContactInfoDtos);
    Task<List<ClientContactInfoDto>> GetClientContactDataByClientIdAsync(int clientId);
}