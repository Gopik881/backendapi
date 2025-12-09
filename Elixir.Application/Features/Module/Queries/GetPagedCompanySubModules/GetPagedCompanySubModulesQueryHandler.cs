using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Module.Queries.GetPagedCompanySubModules;

public record GetPagedCompanySubModulesQuery(int ModuleId, string SearchTerm, int PageNumber, int PageSize) : IRequest<PaginatedResponse<CompanyModuleDto>>;
public class GetPagedSubModulesQueryHandler : IRequestHandler<GetPagedCompanySubModulesQuery, PaginatedResponse<CompanyModuleDto>>
{
    private readonly IModuleMappingRepository _modulesMappingRepository;
    public GetPagedSubModulesQueryHandler(IModuleMappingRepository modulesMappingRepository)
    {
        _modulesMappingRepository = modulesMappingRepository;
    }
    public async Task<PaginatedResponse<CompanyModuleDto>> Handle(GetPagedCompanySubModulesQuery request, CancellationToken cancellationToken)
    {
        // Implement the logic to get paginated company Users with filters
        var result = await _modulesMappingRepository.GetFilteredCompanyModulesAsync(AppConstants.MODULE, request.ModuleId, request.SearchTerm, request.PageNumber, request.PageSize);

        // Apply Pagination
        return new PaginatedResponse<CompanyModuleDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));

    }
}
