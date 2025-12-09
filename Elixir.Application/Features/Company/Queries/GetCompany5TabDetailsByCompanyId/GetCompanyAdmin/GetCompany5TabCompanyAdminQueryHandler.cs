using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetCompanyAdmin
{
    public record GetCompany5TabCompanyAdminQuery(int CompanyId, bool IsPrevious) : IRequest<Company5TabCompanyAdminDto>;

    public class GetCompany5TabCompanyAdminQueryHandler : IRequestHandler<GetCompany5TabCompanyAdminQuery, Company5TabCompanyAdminDto>
    {
        private readonly ICompanyAdminUsersHistoryRepository _companyAdminHistoryRepository;

        public GetCompany5TabCompanyAdminQueryHandler(ICompanyAdminUsersHistoryRepository companyAdminHistoryRepository)
        {
            _companyAdminHistoryRepository = companyAdminHistoryRepository;
        }

        public async Task<Company5TabCompanyAdminDto> Handle(GetCompany5TabCompanyAdminQuery request, CancellationToken cancellationToken)
        {
            return await _companyAdminHistoryRepository.GetCompany5TabLatestCompanyAdminHistoryAsync(request.CompanyId, request.IsPrevious);
        }
    }
}
