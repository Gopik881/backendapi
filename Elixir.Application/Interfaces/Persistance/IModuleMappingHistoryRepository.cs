using Elixir.Application.Features.Company.DTOs;

namespace Elixir.Application.Interfaces.Persistance;

public interface IModuleMappingHistoryRepository
{
    Task<bool> Company5TabCreateModuleMappingDataAsync(int companyId, List<Company5TabModuleMappingDto> moduleMappings, int userId, CancellationToken cancellationToken = default);
    Task<List<Company5TabModuleMappingDto>?> GetCompany5TabLatestModuleMappingHistoryAsync(int companyId, bool isPrevious, CancellationToken cancellationToken = default);
    Task<bool> WithdrawCompany5TabModuleMappingHistoryAsync(int companyId, int userId, CancellationToken cancellationToken = default);
    Task<Company5TabHistoryDto> GetCompany5TabModuleMappingHistoryByVersionAsync(int userId,int companyId,int versionNumber);
}