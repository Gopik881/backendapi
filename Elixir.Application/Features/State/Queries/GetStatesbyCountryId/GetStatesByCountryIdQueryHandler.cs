using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Elixir.Application.Features.State.Queries.GetStatesbyCountryId;

public record GetStatesByCountryIdQuery(int CountryId) : IRequest<CountryWithStatesDto>;

public class GetStatesByCountryIdQueryHandler : IRequestHandler<GetStatesByCountryIdQuery, CountryWithStatesDto>
{
    private readonly IStateMasterRepository _stateMasterRepository;

    public GetStatesByCountryIdQueryHandler(IStateMasterRepository stateMasterRepository)
    {
        _stateMasterRepository = stateMasterRepository;
    }

    public async Task<CountryWithStatesDto> Handle(GetStatesByCountryIdQuery request, CancellationToken cancellationToken)
    {
        return await _stateMasterRepository.GetCountryByIdWithStatesAsync(request.CountryId);
    }
}
