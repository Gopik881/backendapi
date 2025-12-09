using Elixir.Application.Common.Models;
using Elixir.Application.Features.Master.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Master.Queries.GetPagedMaster;

public record GetPagedMasterQuery(string? SearchTerm, int PageNumber, int PageSize) : IRequest<PaginatedResponse<MasterDto>>;
public class GetPagedMasterQueryHandler : IRequestHandler<GetPagedMasterQuery, PaginatedResponse<MasterDto>>
{
    private readonly IMasterRepository _masterRepository;
    public GetPagedMasterQueryHandler(IMasterRepository masterRepository)
    {
        _masterRepository = masterRepository;
    }
    public async Task<PaginatedResponse<MasterDto>> Handle(GetPagedMasterQuery request, CancellationToken cancellationToken)
    {
        // Implement the logic to get paginated master records with filters
        var result = await _masterRepository.GetFilteredMasterAsync(request.SearchTerm, request.PageNumber, request.PageSize);
        // Apply Pagination
        return new PaginatedResponse<MasterDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
        
    }
}
