using Elixir.Application.Features.Company.DTOs;

namespace Elixir.Application.Interfaces.Persistance;
public interface IReportingToolLimitsHistoryRepository
{
    Task<bool> Company5TabCreateReportingToolLimitsDataAsync(int companyId, int userId, Company5TabReportingToolLimitsDto Company5TabReportingTool, CancellationToken cancellationToken = default);
    Task<Company5TabReportingToolLimitsDto?> GetCompany5TabLatestReportingToolLimitsHistoryAsync(int companyId, bool isPrevious, CancellationToken cancellationToken = default);
    Task<bool> WithdrawCompany5TabReportingToolLimitsHistoryAsync(int companyId, int userId, CancellationToken cancellationToken = default);

    Task<Company5TabHistoryDto> GetCompany5TabReportingToolLimitsHistoryByVersionAsync(int userId,int companyId,int versionNumber);
}