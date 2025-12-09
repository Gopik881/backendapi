using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Country.Queries.GetAllCountries;
public record GetAllCountriesQuery : IRequest<IEnumerable<CountryDto>>;
public class GetAllCountriesQueryHandler : IRequestHandler<GetAllCountriesQuery, IEnumerable<CountryDto>>
{
    private readonly ICountryMasterRepository _countryMasterRepository;

    public GetAllCountriesQueryHandler(ICountryMasterRepository countryMasterRepository)
    {
        _countryMasterRepository = countryMasterRepository;
    }

    public async Task<IEnumerable<CountryDto>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
    {
        return await _countryMasterRepository.GetAllCountriesAsync();
    }
    
}
