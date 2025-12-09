using Elixir.Application.Common.Models;
using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;


namespace Elixir.Application.Features.Country.Queries.GetCountryWithStates;
public record GetCountryWithStatesQuery(string SearchTerm, int PageNumber, int PageSize,int CountryId) : IRequest<PaginatedResponse<CountryWithStatesDto>>;
public class GetCountryWithStatesQueryHandler : IRequestHandler<GetCountryWithStatesQuery, PaginatedResponse<CountryWithStatesDto>>
{
    private readonly ICountryMasterRepository _countryMasterRepository;

    public GetCountryWithStatesQueryHandler(ICountryMasterRepository countryMasterRepository)
    {
        _countryMasterRepository = countryMasterRepository;
    }

    public async Task<PaginatedResponse<CountryWithStatesDto>> Handle(GetCountryWithStatesQuery request, CancellationToken cancellationToken)
    {

        // Implement the logic to get paginated countries with filters
        var result = await _countryMasterRepository.GetCountryByIdWithStatesAsync(request.SearchTerm, request.PageNumber, request.PageSize, request.CountryId);
        // Apply Pagination
        return new PaginatedResponse<CountryWithStatesDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
        //return await _countryMasterRepository.GetCountryByIdWithStatesAsync(request.CountryId);
    }
}
