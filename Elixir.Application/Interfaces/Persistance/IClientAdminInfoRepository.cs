using Elixir.Application.Features.Clients.DTOs;

namespace Elixir.Application.Interfaces.Persistance;
public interface IClientAdminInfoRepository
{
    Task<bool> CreateClientAdminInfoDataAsync(int userId, int clientId, ClientAdminInfoDto clientAdminInfoDto);
    Task<bool> DeleteClientAdminInfosAsync(int clientId);
    Task<bool> UpdateClientAdminInfoDataAsync(int userId, int clientId, ClientAdminInfoDto clientAdminInfoDto);
    Task<ClientAdminInfoDto?> GetClientAdminInfoByClientIdAsync(int clientId);


}