using Elixir.Application.Common.Models;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Module.Queries.GetPagedSubModules;

public record GetPagedSubModulesQuery(int ModuleId, string SearchTerm, int PageNumber, int PageSize) : IRequest<PaginatedResponse<SubModuleDto>>;
public class GetPagedSubModulesQueryHandler : IRequestHandler<GetPagedSubModulesQuery, PaginatedResponse<SubModuleDto>>
{
    private readonly ISubModulesRepository _subModulesRepository;
    public GetPagedSubModulesQueryHandler(ISubModulesRepository subModulesRepository)
    {
        _subModulesRepository = subModulesRepository;
    }
    public async Task<PaginatedResponse<SubModuleDto>> Handle(GetPagedSubModulesQuery request, CancellationToken cancellationToken)
    {
        // Implement the logic to get paginated company Users with filters
        var result = await _subModulesRepository.GetFilteredSubModulesByModuleAsync(request.ModuleId, request.SearchTerm, request.PageNumber, request.PageSize);

        // Apply Pagination
        return new PaginatedResponse<SubModuleDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));

    }
}
