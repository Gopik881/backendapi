using Elixir.Application.Common.Models;
using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.State.Queries.GetPagedStatesWithFilters;

public record GetPagedStatesWithFiltersQuery(string SearchTerm, int PageNumber, int PageSize) : IRequest<PaginatedResponse<StateDto>>;
public class GetPagedStatesWithFiltersQueryHandler : IRequestHandler<GetPagedStatesWithFiltersQuery, PaginatedResponse<StateDto>>
{
    private IStateMasterRepository _stateMasterRepository;

    public GetPagedStatesWithFiltersQueryHandler(IStateMasterRepository stateMasterRepository)
    {
        _stateMasterRepository = stateMasterRepository;
    }

    public async Task<PaginatedResponse<StateDto>> Handle(GetPagedStatesWithFiltersQuery request, CancellationToken cancellationToken)
    {
        // Implement the logic to get paginated countries with filters
        var result = await _stateMasterRepository.GetFilteredStatesAsync(request.SearchTerm, request.PageNumber, request.PageSize);
        // Apply Pagination
        return new PaginatedResponse<StateDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
    }
}
