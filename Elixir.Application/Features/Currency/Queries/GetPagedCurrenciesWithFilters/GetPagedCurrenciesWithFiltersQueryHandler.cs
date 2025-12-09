using Elixir.Application.Common.Models;
using Elixir.Application.Features.Currency.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Currency.Queries.GetPagedCurrenciesWithFilters;

public record GetPagedCurrenciesWithFiltersQuery(string SearchTerm, int PageNumber, int PageSize) : IRequest<PaginatedResponse<CurrencyDto>>;

public class GetPagedCurrenciesWithFiltersQueryHandler : IRequestHandler<GetPagedCurrenciesWithFiltersQuery, PaginatedResponse<CurrencyDto>>
{
    private readonly ICurrencyMasterRepository _currencyMasterRepository;
    public GetPagedCurrenciesWithFiltersQueryHandler(ICurrencyMasterRepository currencyMasterRepository)
    {
        _currencyMasterRepository = currencyMasterRepository;
    }
    public async Task<PaginatedResponse<CurrencyDto>> Handle(GetPagedCurrenciesWithFiltersQuery request, CancellationToken cancellationToken)
    {
        // Implement the logic to get paginated currencies with filters
        var result = await _currencyMasterRepository.GetFilteredCurrenciesAsync(request.SearchTerm, request.PageNumber, request.PageSize);

        // Apply Pagination
        return new PaginatedResponse<CurrencyDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));

    }
}
