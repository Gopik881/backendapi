using Elixir.Application.Features.Currency.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using MediatR;


namespace Elixir.Application.Features.Currency.Commands.CreateCurrency;
public record CreateCurrencyCommand(List<CreateUpdateCurrencyDto> CreateCurrencyDto) : IRequest<bool>;
public class CreateCurrencyCommandHandler : IRequestHandler<CreateCurrencyCommand, bool>
{
    private readonly ICurrencyMasterRepository _currencyMasterRepository;
    private readonly ICountryMasterRepository _countryMasterRepository;

    public CreateCurrencyCommandHandler(ICountryMasterRepository countryMasterRepository, ICurrencyMasterRepository currencyMasterRepository)
    {
        _countryMasterRepository = countryMasterRepository;
        _currencyMasterRepository = currencyMasterRepository;
    }

    public async Task<bool> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
            bool anyDuplicateCurrenciesExists = await _currencyMasterRepository.AnyDuplicateCurrenciesExistsAsync(request.CreateCurrencyDto);
            if (anyDuplicateCurrenciesExists) return false;
            
            foreach (var currency in request.CreateCurrencyDto)
            {
                //var countryExists = await _countryMasterRepository.ExistsAsync(currency.CountryId);
                //if (!countryExists) return 0;

                //var currencyExists = await _currencyMasterRepository.ExistsWithCurrencyNameAsync(currency.CurrencyName, currency.CountryId);
                //if (!currencyExists) return 0;

                var newCurrency = new CurrencyMaster
                {
                    CountryId = currency.CountryId,
                    CurrencyName = currency.CurrencyName,
                    CurrencyShortName = currency.CurrencyShortName,
                    Description = currency.Description,
                };
                await _currencyMasterRepository.CreateCurrencyAsync(newCurrency);
            }
            return true;
    }
}

