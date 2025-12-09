using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Queries.GetClientById.ClientAccess
{
    public record GetClientAccessByClientIdQuery(int ClientId) : IRequest<ClientAccessDto>;

    public class GetClientAccessByClientIdQueryHandler : IRequestHandler<GetClientAccessByClientIdQuery, ClientAccessDto>
    {
        private readonly IClientAccessRepository _clientAccessRepository;

        public GetClientAccessByClientIdQueryHandler(IClientAccessRepository clientAccessRepository)
        {
            _clientAccessRepository = clientAccessRepository;
        }

        public async Task<ClientAccessDto> Handle(GetClientAccessByClientIdQuery request, CancellationToken cancellationToken)
        {
            return await _clientAccessRepository.GetClientAccessByClientIdAsync(request.ClientId);
        }
    }
}
