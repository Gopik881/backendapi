
using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Module.Queries.GetPagedCompanyModuleSubModules;

public record GetPagedModuleInfoByCompany(int CompanyId, string SearchTerm, int PageNumber, int PageSize) : IRequest<PaginatedResponse<CompanyModuleDto>>;
public class GetPagedModuleInfoByCompanyQueryHandler : IRequestHandler<GetPagedModuleInfoByCompany, PaginatedResponse<CompanyModuleDto>>
{
    ICompaniesRepository _companiesRepository;
    IModuleMappingRepository _modulesMappingRepository;

    public GetPagedModuleInfoByCompanyQueryHandler(ICompaniesRepository companiesRepository,IModuleMappingRepository modulesMappingRepository)
    {
        _companiesRepository = companiesRepository;
        _modulesMappingRepository = modulesMappingRepository;
    }

    public async Task<PaginatedResponse<CompanyModuleDto>> Handle(GetPagedModuleInfoByCompany request, CancellationToken cancellationToken)
    {
        var companyInfo = await _companiesRepository.GetCompanyBasicInfoAsync(request.CompanyId);
        if (companyInfo == null) return null;

        var result = await _modulesMappingRepository.GetFilteredCompanyModulesAsync(AppConstants.COMPANY, request.CompanyId, request.SearchTerm, request.PageNumber, request.PageSize);

        // Apply Pagination
        return new PaginatedResponse<CompanyModuleDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));

    }
}
