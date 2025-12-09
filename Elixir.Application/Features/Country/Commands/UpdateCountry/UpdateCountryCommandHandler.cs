using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;


namespace Elixir.Application.Features.Country.Commands.UpdateCountry;

public record UpdateCountryCommand(int CountryId, CreateUpdateCountryDto UpdateCountryDto) : IRequest<bool>;

public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, bool>
{
    private readonly ICountryMasterRepository _countryMasterRepository;
    public readonly IEntityReferenceService _entityReferenceService;
    public UpdateCountryCommandHandler(ICountryMasterRepository countryMasterRepository, IEntityReferenceService entityReferenceService)
    {
        _countryMasterRepository = countryMasterRepository;
        _entityReferenceService = entityReferenceService;
    }
    public async Task<bool> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var country = await _countryMasterRepository.GetCountryByIdAsync(request.CountryId);
            if (country == null) return false;

            // Check if the country is actively referenced before restricting change
            int? activeReferenceId = await _entityReferenceService.GetActiveReferenceIdAsync(nameof(country.Id), country.Id);
            if (activeReferenceId.HasValue && country.Id != request.CountryId)
            {
                throw new Exception(AppConstants.ErrorCodes.COMMON_MASTER_LIST_LINKED_ITEM_DELETE_CANNOT_REMAP);
            }

            country.CountryName = request.UpdateCountryDto.CountryName;
            country.CountryShortName = request.UpdateCountryDto.CountryShortName;
            country.Description = request.UpdateCountryDto.Description;

            return await _countryMasterRepository.UpdateCountryAsync(country);
        }
        catch (Exception ex)
        {
            throw new Exception(AppConstants.ErrorCodes.COUNTRY_UPDATE_FAILED);
        }

    }
}
