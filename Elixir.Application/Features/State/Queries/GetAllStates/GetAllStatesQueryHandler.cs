using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.State.Queries.GetAllStates;
public record GetAllStatesQuery : IRequest<IEnumerable<StateDto>>;
public class GetAllStatesQueryHandler : IRequestHandler<GetAllStatesQuery, IEnumerable<StateDto>>
{
    private readonly IStateMasterRepository _stateMasterRepository;

    public GetAllStatesQueryHandler(IStateMasterRepository stateMasterRepository)
    {
        _stateMasterRepository = stateMasterRepository;
    }

    public async Task<IEnumerable<StateDto>> Handle(GetAllStatesQuery request, CancellationToken cancellationToken)
    {
        return await _stateMasterRepository.GetAllStatesAsync();
    }
    
}
