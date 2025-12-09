using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompanyPopupDetails;

public record GetCompanyPopupDetailsByCompanyIdQuery(int CompanyId) : IRequest<object>;

public class GetCompanyPopupDetailsByCompanyIdQueryHandler : IRequestHandler<GetCompanyPopupDetailsByCompanyIdQuery, object>
{
    private readonly ICompaniesRepository _companyRepository;

    public GetCompanyPopupDetailsByCompanyIdQueryHandler(ICompaniesRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<object> Handle(GetCompanyPopupDetailsByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        return await _companyRepository.GetCompanyPopupDetailsByCompanyIdAsync(request.CompanyId);
    }
}
