using Elixir.Application.Features.Clients.DTOs;

namespace Elixir.Application.Interfaces.Persistance;
public interface IClientCompaniesMappingRepository
{
    Task<bool> CreateClientCompanyMapDataAsync(int userId, int clientId, List<ClientCompanyMappingDto> clientCompanyMappingDtos, string ClientName);
    Task<bool> DeleteClientCompaniesMappingsAsync(int clientId);
    Task<bool> UpdateClientCompanyMapDataAsync(int userId, int clientId, List<ClientCompanyMappingDto> clientCompanyMappingDtos, string clientName, bool IsSuperUser);
    Task<List<ClientCompanyMappingDto>> GetClientCompanyMappingByClientIdAsync(int clientId);
}