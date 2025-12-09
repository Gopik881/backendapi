using Elixir.Application.Features.Company.DTOs;

namespace Elixir.Application.Interfaces.Persistance;
public interface IReportingToolLimitsRepository
{
    Task<bool> Company5TabApproveReportingToolLimitsDataAsync(int companyId, int userId, Company5TabReportingToolLimitsDto Company5TabReportingTool, CancellationToken cancellationToken = default);
}