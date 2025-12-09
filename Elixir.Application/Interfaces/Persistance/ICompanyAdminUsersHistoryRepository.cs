using Elixir.Application.Features.Company.DTOs;

namespace Elixir.Application.Interfaces.Persistance;
public interface ICompanyAdminUsersHistoryRepository
{
    Task<bool> Company5TabCreateCompanyAdminDataAsync(int companyId, int userId, Company5TabCompanyAdminDto company5TabCompanyAdmin, CancellationToken cancellationToken = default);
    Task<Company5TabCompanyAdminDto?> GetCompany5TabLatestCompanyAdminHistoryAsync(int companyId, bool isPrevious, CancellationToken cancellationToken = default);
    Task<bool> WithdrawCompany5TabCompanyAdminHistoryAsync(int companyId, int userId, CancellationToken cancellationToken = default);
    Task<Company5TabHistoryDto> GetCompany5TabCompanyAdminHistoryByVersionAsync(int userId,int companyId,int versionNumber);
}