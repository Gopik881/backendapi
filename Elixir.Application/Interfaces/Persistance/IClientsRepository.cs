using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Domain.Entities;

namespace Elixir.Application.Interfaces.Persistance;

public interface IClientsRepository
{
    Task<bool> UpdateClientInformationAsync(ClientInfoDto clientInfoDto, int clientId, int userId);
    Task<bool> ExistsWithClientNameAsync(string clientName);
    Task<bool> ExistsWithClientNameIdsAsync(string clientName, List<int> clientIds);
    Task<bool> ExistsWithClientCodeAsync(string clientCode);

    Task<bool> ExistsWithUpdateClientCodeAsync(string clientCode, List<int> ClientIds);
    Task<List<int>> GetDistinctClientIdsByCompanyIdsAsync(IEnumerable<int> companyIds);
    Task<int?> GetClientIdByClientNameAsync(string clientName);
    Task<IEnumerable<ClientGroupswithAccountManagersDto>> GetClientGroupswithAccountManagersAsync();
    Task<IEnumerable<ClientUnmappedCompaniesDto>> GetClientUnmappedCompaniesAsync();
    Task<bool> DeleteClientAccountManagersAsync(int clientId);
    Task<bool> DeleteClientInfoAsync(int clientId);
    Task<Client?> GetClientByIdAsync(int clientId);
    Task<bool> UpdateClientAsync(Client existingClient);
    Task<ClientInfoDto?> GetClientDetailsByIdAsync(int clientId);
    Task<ClientPopupDetailsDto?> GetClientDetailsAsync(int clientId);

    Task<Tuple<List<ClientDto>, int>> GetFilteredClientsAsync(int pageNumber, int pageSize, string searchTerm);
    Task<Tuple<List<ClientCompanyDto>, int>> GetFilteredClientCompaniesAsync(int clientId, int pageNumber, int pageSize, string? searchTerm);

    Task<List<int>> GetListOfClientIdsByCompanyNameAsync(string clientName);

    Task<Tuple<List<ClientBasicInfoDto>, int>> GetFilteredClientsByUsersAsync(int userId, int groupId, string groupName, string searchTerm, int pageNumber, int pageSize);

    Task<string?> GetCompanyNameByClientCodeAsync(string clientCode);
}