using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.CompanyHistory;

public record GetCompany5TabCompanyHistoryQuery(int UserId, int CompanyId, int VersionNumber) : IRequest<object>;

public class GetCompany5TabCompanyHistoryQueryHandler : IRequestHandler<GetCompany5TabCompanyHistoryQuery, object>
{
    private readonly ICompanyHistoryRepository _companyHistoryRepository;

    public GetCompany5TabCompanyHistoryQueryHandler(ICompanyHistoryRepository companyHistoryRepository)
    {
        _companyHistoryRepository = companyHistoryRepository;
    }

    public async Task<object> Handle(GetCompany5TabCompanyHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _companyHistoryRepository.GetCompany5TabCompanyHistoryByVersionJsonAsync(request.UserId, request.CompanyId, request.VersionNumber);
    }
}
