using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetCompany
{
    public record GetCompany5TabCompanyDataQuery(int CompanyId, int userId, bool IsPrevious) : IRequest<Company5TabCompanyDto>;

    public class GetCompany5TabCompanyDataQueryHandler : IRequestHandler<GetCompany5TabCompanyDataQuery, Company5TabCompanyDto>
    {
        private readonly ICompanyHistoryRepository _companyHistoryRepository;

        public GetCompany5TabCompanyDataQueryHandler(ICompanyHistoryRepository companyHistoryRepository)
        {
            _companyHistoryRepository = companyHistoryRepository;
        }

        public async Task<Company5TabCompanyDto> Handle(GetCompany5TabCompanyDataQuery request, CancellationToken cancellationToken)
        {
            return await _companyHistoryRepository.GetCompany5TabLatestCompanyHistoryAsync(request.CompanyId, request.userId, request.IsPrevious);
        }
    }
}
