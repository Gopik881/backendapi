using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.ModuleMappingHistory;

public record GetCompany5TabModuleMappingHistoryQuery(int UserId, int CompanyId, int VersionNumber) : IRequest<Company5TabHistoryDto>;

public class GetCompany5TabModuleMappingHistoryQueryHandler : IRequestHandler<GetCompany5TabModuleMappingHistoryQuery, Company5TabHistoryDto>
{
    private readonly IModuleMappingHistoryRepository _companyModuleMappingHistoryRepository;

    public GetCompany5TabModuleMappingHistoryQueryHandler(IModuleMappingHistoryRepository companyModuleMappingHistoryRepository)
    {
        _companyModuleMappingHistoryRepository = companyModuleMappingHistoryRepository;
    }

    public async Task<Company5TabHistoryDto> Handle(GetCompany5TabModuleMappingHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _companyModuleMappingHistoryRepository.GetCompany5TabModuleMappingHistoryByVersionAsync(
            request.UserId,
            request.CompanyId,
            request.VersionNumber
        );
    }
}
