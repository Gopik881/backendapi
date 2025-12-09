using Elixir.Application.Features.Currency.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Currency.Queries.GetAllCurrency;
public record GetAllCurrencyQuery : IRequest<IEnumerable<CurrencyDto>>;
public class GetAllCurrencyQueryHandler : IRequestHandler<GetAllCurrencyQuery, IEnumerable<CurrencyDto>>
{
    private readonly ICurrencyMasterRepository _currencyMasterRepository;

    public GetAllCurrencyQueryHandler(ICurrencyMasterRepository currencyMasterRepository)
    {
        _currencyMasterRepository = currencyMasterRepository;
    }

    public async Task<IEnumerable<CurrencyDto>> Handle(GetAllCurrencyQuery request, CancellationToken cancellationToken)
    {
        return await _currencyMasterRepository.GetAllCurrenciesAsync();
    }
}

