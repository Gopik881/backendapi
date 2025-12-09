using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.ReportingToolHistory;

public record GetCompany5TabReportingToolLimitHistoryQuery(int UserId, int CompanyId, int VersionNumber) : IRequest<Company5TabHistoryDto>;

public class GetCompany5TabReportingTollLimitHistoryQueryHandler : IRequestHandler<GetCompany5TabReportingToolLimitHistoryQuery, Company5TabHistoryDto>
{
    private readonly IReportingToolLimitsHistoryRepository _companyReportingToolHistoryRepository;

    public GetCompany5TabReportingTollLimitHistoryQueryHandler(IReportingToolLimitsHistoryRepository companyReportingToolHistoryRepository)
    {
        _companyReportingToolHistoryRepository = companyReportingToolHistoryRepository;
    }

    public async Task<Company5TabHistoryDto> Handle(GetCompany5TabReportingToolLimitHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _companyReportingToolHistoryRepository.GetCompany5TabReportingToolLimitsHistoryByVersionAsync(
            request.UserId,
            request.CompanyId,
            request.VersionNumber
        );
    }
}
