using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.State.Queries.GetValidateDuplicateStates
{
    // Define the Query record
    public record GetValidateDuplicateStatesQuery(CreateUpdateStateDto States) : IRequest<bool>;

    // Implement the Handler
    public class GetValidateDuplicateStatesQueryHandler : IRequestHandler<GetValidateDuplicateStatesQuery, bool>
    {
        private readonly IStateMasterRepository _stateMasterRepository;

        public GetValidateDuplicateStatesQueryHandler(IStateMasterRepository stateMasterRepository)
        {
            _stateMasterRepository = stateMasterRepository;
        }

        public async Task<bool> Handle(GetValidateDuplicateStatesQuery request, CancellationToken cancellationToken)
        {
            return await _stateMasterRepository.IsDuplicateStateExistsAsync(request.States);
        }
    }
}
