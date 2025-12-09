using Elixir.Application.Common.Models;
using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Country.Queries.GetPagedCountriesWithFilters;

public record GetPagedCountriesWithFiltersQuery(string SearchTerm,int PageNumber,int PageSize) : IRequest<PaginatedResponse<CountryDto>>;
public class GetPagedCountriesWithFiltersQueryHandler:IRequestHandler<GetPagedCountriesWithFiltersQuery, PaginatedResponse<CountryDto>>
{
    private readonly ICountryMasterRepository _countryMasterRepository;
    public GetPagedCountriesWithFiltersQueryHandler(ICountryMasterRepository countryMasterRepository)
    {
        _countryMasterRepository = countryMasterRepository;
    }

    public async Task<PaginatedResponse<CountryDto>> Handle(GetPagedCountriesWithFiltersQuery request, CancellationToken cancellationToken)
    {
        // Implement the logic to get paginated countries with filters
        var result = await _countryMasterRepository.GetFilteredCountriesAsync(request.SearchTerm, request.PageNumber,request.PageSize);
        // Apply Pagination
        return new PaginatedResponse<CountryDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
    }
}
