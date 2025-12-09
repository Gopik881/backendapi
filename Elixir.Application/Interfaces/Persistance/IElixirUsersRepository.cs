using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Features.Company.DTOs;

namespace Elixir.Application.Interfaces.Persistance;

public interface IElixirUsersRepository
{
    Task<Tuple<List<CompanyUserDto>, int>> GetFilteredAccountManagersAsync(int CompanyId, string searchTerm, int pageNumber, int pageSize);
    Task<bool> Company5TabApproveElixirUserDataAsync(int companyId, List<Company5TabElixirUserDto> company5TabElixirUser, int userId, CancellationToken cancellationToken = default);
    Task<bool> ReplaceClientAccountManagersAsync(int userId, int clientId, List<ClientAccountManagersDto> clientAccountManagers, CancellationToken cancellationToken = default);
    Task<List<ClientAccountManagersDto>> GetClientAccountManagersByClientIdAsync(int clientId);
    Task<ElixirUserListDto> GetElixirUserListsByCompanyIdAsync(int companyId);
    //Task<ElixirUserListDto> GetUserListsFromUserGroupMappingAsync();
    Task<ElixirUserListDto> GetUserListsFromUserGroupMappingAsync(string? ScreenName = "");
}