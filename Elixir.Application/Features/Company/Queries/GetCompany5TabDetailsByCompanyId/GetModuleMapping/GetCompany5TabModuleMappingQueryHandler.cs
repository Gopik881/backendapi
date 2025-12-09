using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetModuleMapping;

public record GetCompany5TabModuleMappingQuery(int CompanyId, bool IsPrevious) : IRequest<List<Company5TabModuleMappingDto>>;

public class GetCompany5TabModuleMappingQueryHandler : IRequestHandler<GetCompany5TabModuleMappingQuery, List<Company5TabModuleMappingDto>>
{
    private readonly IModuleMappingHistoryRepository _companyModuleMappingHistoryRepository;

    public GetCompany5TabModuleMappingQueryHandler(IModuleMappingHistoryRepository companyModuleMappingHistoryRepository)
    {
        _companyModuleMappingHistoryRepository = companyModuleMappingHistoryRepository;
    }

    public async Task<List<Company5TabModuleMappingDto>> Handle(GetCompany5TabModuleMappingQuery request, CancellationToken cancellationToken)
    {
        return await _companyModuleMappingHistoryRepository.GetCompany5TabLatestModuleMappingHistoryAsync(request.CompanyId, request.IsPrevious);
    }
}
