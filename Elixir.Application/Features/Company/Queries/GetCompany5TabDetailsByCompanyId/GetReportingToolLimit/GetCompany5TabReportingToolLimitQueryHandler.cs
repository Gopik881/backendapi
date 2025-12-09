using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetReportingToolLimit;

public record GetCompany5TabReportingToolLimitQuery(int CompanyId, bool IsPrevious) : IRequest<Company5TabReportingToolLimitsDto>;

public class GetCompany5TabReportingToolLimitQueryHandler : IRequestHandler<GetCompany5TabReportingToolLimitQuery, Company5TabReportingToolLimitsDto>
{
    private readonly IReportingToolLimitsHistoryRepository _companyReportingToolHistoryRepository;

    public GetCompany5TabReportingToolLimitQueryHandler(IReportingToolLimitsHistoryRepository companyReportingToolHistoryRepository)
    {
        _companyReportingToolHistoryRepository = companyReportingToolHistoryRepository;
    }

    public async Task<Company5TabReportingToolLimitsDto> Handle(GetCompany5TabReportingToolLimitQuery request, CancellationToken cancellationToken)
    {
        return await _companyReportingToolHistoryRepository.GetCompany5TabLatestReportingToolLimitsHistoryAsync(request.CompanyId, request.IsPrevious);
    }
}
