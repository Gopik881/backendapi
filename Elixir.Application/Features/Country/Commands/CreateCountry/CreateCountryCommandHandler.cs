using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using MediatR;

namespace Elixir.Application.Features.Country.Commands.CreateCountry;

public record CreateCountryCommand(List<CreateUpdateCountryDto> CreateCountryDto) : IRequest<bool>;
public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, bool>
{
    private readonly ICountryMasterRepository _countryMasterRepository;
    public CreateCountryCommandHandler(ICountryMasterRepository countryMasterRepository)
    {
        _countryMasterRepository = countryMasterRepository;
    }

    public async Task<bool> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            bool anyDuplicateCountrysExists = await _countryMasterRepository.AnyDuplicateDuplicateCountrysExistsAsync(request.CreateCountryDto);
            if (anyDuplicateCountrysExists) return false;

            foreach (var country in request.CreateCountryDto)
            {
                //var countryExists = await _countryMasterRepository.ExistsWithCountryNameAsync(country.CountryName);
                //if (countryExists) return 0;

                var newCountry = new CountryMaster
                {
                    CountryName = country.CountryName,
                    CountryShortName = country.CountryShortName,
                    Description = country.Description
                };
                await _countryMasterRepository.CreateCountryAsync(newCountry);
            }
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(AppConstants.ErrorCodes.COUNTRY_CREATION_FAILED);
        }
    }
}
