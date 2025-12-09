using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Queries.GetClientById.Client
{
    public record GetClientDetailsByClientIdQuery(int ClientId) : IRequest<ClientInfoDto>;

    public class GetClientDetailsByClientIdQueryHandler : IRequestHandler<GetClientDetailsByClientIdQuery, ClientInfoDto>
    {
        private readonly IClientsRepository _clientRepository;

        public GetClientDetailsByClientIdQueryHandler(IClientsRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<ClientInfoDto> Handle(GetClientDetailsByClientIdQuery request, CancellationToken cancellationToken)
        {
            return await _clientRepository.GetClientDetailsByIdAsync(request.ClientId);
        }
    }
}
