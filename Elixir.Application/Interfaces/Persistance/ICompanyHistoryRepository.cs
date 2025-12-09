using Elixir.Application.Features.Company.DTOs;

namespace Elixir.Application.Interfaces.Persistance;

public interface ICompanyHistoryRepository
{
    Task<bool> Company5TabCreateCompanyDataAsync(int userId, Company5TabCompanyDto company5TabCompanyData, int companyStoregeGB, int peruserStoreageMB, CancellationToken cancellationToken = default);
    Task<int> GetCompanyHistoryUserIdByCompanyId(int companyId);
    Task<Company5TabCompanyDto?> GetCompany5TabLatestCompanyHistoryAsync(int companyId, int userId, bool isPrevious, CancellationToken cancellationToken = default);
    Task<bool> WithdrawCompany5TabCompanyHistoryAsync(int companyId, int userId, CancellationToken cancellationToken = default);

    //Task<Company5TabHistoryDto> GetCompany5TabCompanyHistoryByVersionAsync(int userId,int companyId,int versionNumber);
    Task<object> GetCompany5TabCompanyHistoryByVersionJsonAsync(int userId, int companyId, int versionNumber);
}