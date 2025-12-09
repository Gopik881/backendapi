using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Features.Company.DTOs;

namespace Elixir.Application.Interfaces.Persistance;

public interface IElixirUsersHistoryRepository
{
    Task<bool> Company5TabCreateElixirUserDataAsync(int companyId, List<Company5TabElixirUserDto> company5TabElixirUser, int userId, CancellationToken cancellationToken = default);
    Task<List<Company5TabElixirUserDto>> GetCompany5TabLatestElixirUsersHistoryAsync(int companyId, bool isPrevious, CancellationToken cancellationToken = default);
    Task<bool> WithdrawCompany5TabElixirUsersHistoryAsync(int companyId, int userId, CancellationToken cancellationToken = default);
    Task<Company5TabHistoryDto> GetCompany5TabElixirUsersHistoryByVersionAsync(int userId,int companyId,int versionNumber);
    Task<bool> CreateClientAccountManagerDataAsync(int userId, int clientId, List<ClientAccountManagersDto> clientAccountManagers);
    Task<bool> UpdateClientAccountManagerDataAsync(int userId, int clientId, List<ClientAccountManagersDto> clientAccountManagers);

}