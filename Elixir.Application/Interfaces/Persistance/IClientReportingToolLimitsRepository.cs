using Elixir.Application.Features.Clients.DTOs;

namespace Elixir.Application.Interfaces.Persistance;

public interface IClientReportingToolLimitsRepository
{
    Task<bool> CreateClientReportingToolLimitDataAsync(int userId, int clientId, ReportingToolLimitsDto clientReportingToolLimitDto);
    Task<bool> DeleteClientReportingToolLimitsAsync(int clientId);

    Task<bool> UpdateClientReportingToolLimitDataAsync(int userId, int clientId, ReportingToolLimitsDto clientReportingToolLimitDto);
    Task<ReportingToolLimitsDto?> GetClientReportingToolLimitDataAsync(int clientId);
}