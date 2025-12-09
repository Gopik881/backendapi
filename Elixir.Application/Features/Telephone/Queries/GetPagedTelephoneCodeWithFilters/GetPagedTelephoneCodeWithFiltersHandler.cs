using Elixir.Application.Common.Models;
using Elixir.Application.Features.Telephone.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Telephone.Queries.GetPagedTelephoneCodeWithFilters;


public record GetPagedTelephoneWithFiltersQuery(string SearchTerm, int PageNumber, int PageSize) : IRequest<PaginatedResponse<TelephoneCodeMasterDto>>;
public class GetPagedTelephonesWithFiltersQueryHandler : IRequestHandler<GetPagedTelephoneWithFiltersQuery, PaginatedResponse<TelephoneCodeMasterDto>>
{
    private readonly ITelephoneCodeMasterRepository _telephoneCodeMasterRepository;

    public GetPagedTelephonesWithFiltersQueryHandler(ITelephoneCodeMasterRepository telephoneCodeMasterRepository)
    {
        _telephoneCodeMasterRepository = telephoneCodeMasterRepository;
    }

    public async Task<PaginatedResponse<TelephoneCodeMasterDto>> Handle(GetPagedTelephoneWithFiltersQuery request, CancellationToken cancellationToken)
    {
        // Implement the logic to get paginated countries with filters
        var result = await _telephoneCodeMasterRepository.GetFilteredTelephoneCodesAsync(request.SearchTerm, request.PageNumber, request.PageSize);
        // Apply Pagination
        return new PaginatedResponse<TelephoneCodeMasterDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
    }
}
