using Elixir.Application.Common.Models;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;


namespace Elixir.Application.Features.Module.Queries.GetPagedModuleUsage;

public record GetPagedModuleUsageQuery(string SearchTerm, int PageNumber, int PageSize) : IRequest<PaginatedResponse<ModuleUsageDto>>;
public class GetPagedModuleUsageQueryHandler : IRequestHandler<GetPagedModuleUsageQuery, PaginatedResponse<ModuleUsageDto>>
{
    private readonly IModuleMappingRepository _modulesMappingRepository;
    public GetPagedModuleUsageQueryHandler(IModuleMappingRepository modulesMappingRepository)
    {
        _modulesMappingRepository = modulesMappingRepository;
    }
    public async Task<PaginatedResponse<ModuleUsageDto>> Handle(GetPagedModuleUsageQuery request, CancellationToken cancellationToken)
    {
        // Implement the logic to get paginated company Users with filters
        var result = await _modulesMappingRepository.GetFilteredModulesUsageAsync(request.SearchTerm, request.PageNumber, request.PageSize);

        // Apply Pagination
        return new PaginatedResponse<ModuleUsageDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
    }
}
