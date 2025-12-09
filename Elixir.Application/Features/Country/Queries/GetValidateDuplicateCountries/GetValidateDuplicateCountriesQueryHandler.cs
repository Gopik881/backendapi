using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Country.Queries.GetValidateDuplicateCountries
{
    public record GetValidateDuplicateCountriesQuery(CreateUpdateCountryDto Country) : IRequest<bool>;

    public class GetValidateDuplicateCountriesQueryHandler : IRequestHandler<GetValidateDuplicateCountriesQuery, bool>
    {
        private readonly ICountryMasterRepository _countryMasterRepository;

        public GetValidateDuplicateCountriesQueryHandler(ICountryMasterRepository countryMasterRepository)
        {
            _countryMasterRepository = countryMasterRepository;
        }

        public async Task<bool> Handle(GetValidateDuplicateCountriesQuery request, CancellationToken cancellationToken)
        {
            return await _countryMasterRepository.IsDuplicateCountryExistsAsync(request.Country);
        }
    }
}
