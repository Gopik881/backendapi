using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Currency.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

namespace Elixir.Application.Features.Currency.Commands.UpdateCurrency;

public record UpdateCurrencyCommand(int CurrencyId, CreateUpdateCurrencyDto UpdateCurrencyDto) : IRequest<bool>;

public class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand, bool>
{
    private readonly ICurrencyMasterRepository _currencyMasterRepository;
    private readonly ICountryMasterRepository _countryMasterRepository;
    private readonly IEntityReferenceService _entityReferenceService;
    public UpdateCurrencyCommandHandler(ICurrencyMasterRepository currencyMasterRepository, ICountryMasterRepository countryMasterRepository, IEntityReferenceService entityReferenceService)
    {
        _currencyMasterRepository = currencyMasterRepository;
        _countryMasterRepository = countryMasterRepository;
        _entityReferenceService = entityReferenceService;
    }

    public async Task<bool> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currency = await _currencyMasterRepository.GetCurrencyByIdAsync(request.CurrencyId);
        if (currency == null) return false;

        var countryExists = await _countryMasterRepository.ExistsAsync(request.UpdateCurrencyDto.CountryId);
        if (!countryExists) return false;

        // Check if the currency is actively referenced before restricting change
        int? activeReferenceId = await _entityReferenceService.GetActiveReferenceIdAsync(nameof(currency.CountryId), currency.CountryId);
        if (activeReferenceId.HasValue && currency.CountryId != request.UpdateCurrencyDto.CountryId)
        {
            throw new Exception(AppConstants.ErrorCodes.COMMON_MASTER_LIST_LINKED_ITEM_DELETE_CANNOT_REMAP);
        }

        currency.CountryId = request.UpdateCurrencyDto.CountryId;
        currency.CurrencyName = request.UpdateCurrencyDto.CurrencyName;
        currency.CurrencyShortName = request.UpdateCurrencyDto.CurrencyShortName;
        currency.Description = request.UpdateCurrencyDto.Description;
        return await _currencyMasterRepository.UpdateAsync(currency);
    }
}
