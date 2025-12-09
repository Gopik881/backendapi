using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Features.Currency.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Currency.Queries.GetValidateDuplicateCurrencies;

public record GetValidateDuplicateCurrenciesQuery(CreateUpdateCurrencyDto Currencies) : IRequest<bool>;
public class GetValidateDuplicateCurrenciesQueryHandler : IRequestHandler<GetValidateDuplicateCurrenciesQuery, bool>
{
    private readonly ICurrencyMasterRepository _currencyMasterRepository;
    public GetValidateDuplicateCurrenciesQueryHandler(ICurrencyMasterRepository currencyMasterRepository)
    {
        _currencyMasterRepository = currencyMasterRepository;
    }
    public async Task<bool> Handle(GetValidateDuplicateCurrenciesQuery request, CancellationToken cancellationToken)
    {
        return await _currencyMasterRepository.IsDuplicateCurrencyExistsAsync(request.Currencies);
    }
}
