using Elixir.Application.Features.Clients.DTOs;
namespace Elixir.Application.Interfaces.Persistance;

public interface IClientAccessRepository
{

    Task<bool> CreateClientAccessDataAsync(int userId, int clientId, ClientAccessDto clientAccessDto);
    Task<bool> DeleteClientAccessAsync(int clientId);
    Task<bool> GetClientByCompanyIdAsync(int clientId);

    Task<bool> UpdateClientAccessAsync(ClientAccessDto clientAccessDto, int clientId, int userId);
    Task<ClientAccessDto?> GetClientAccessByClientIdAsync(int clientId);
}