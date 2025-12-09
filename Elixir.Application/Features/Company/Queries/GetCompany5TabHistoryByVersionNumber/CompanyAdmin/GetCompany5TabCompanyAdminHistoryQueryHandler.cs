using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.CompanyAdmin;

public record GetCompany5TabCompanyAdminHistoryQuery(int UserId, int CompanyId, int VersionNumber) : IRequest<Company5TabHistoryDto>;

public class GetCompany5TabCompanyAdminHistoryQueryHandler : IRequestHandler<GetCompany5TabCompanyAdminHistoryQuery, Company5TabHistoryDto>
{
    private readonly ICompanyAdminUsersHistoryRepository _companyAdminHistoryRepository;

    public GetCompany5TabCompanyAdminHistoryQueryHandler(ICompanyAdminUsersHistoryRepository companyAdminHistoryRepository)
    {
        _companyAdminHistoryRepository = companyAdminHistoryRepository;
    }

    public async Task<Company5TabHistoryDto> Handle(GetCompany5TabCompanyAdminHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _companyAdminHistoryRepository.GetCompany5TabCompanyAdminHistoryByVersionAsync(request.UserId, request.CompanyId, request.VersionNumber);
    }
}
