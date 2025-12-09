using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Queries.GetClientById.ClientAccountManager
{
    public record GetClientAccountManagerByClientIdQuery(int ClientId) : IRequest<List<ClientAccountManagersDto>>;

    public class GetClientAccountManagerByClientIdQueryHandler : IRequestHandler<GetClientAccountManagerByClientIdQuery, List<ClientAccountManagersDto>>
    {
        private readonly IElixirUsersRepository _clientAccountRepository;

        public GetClientAccountManagerByClientIdQueryHandler(IElixirUsersRepository clientAccountRepository)
        {
            _clientAccountRepository = clientAccountRepository;
        }

        public async Task<List<ClientAccountManagersDto>> Handle(GetClientAccountManagerByClientIdQuery request, CancellationToken cancellationToken)
        {
            return await _clientAccountRepository.GetClientAccountManagersByClientIdAsync(request.ClientId);
        }
    }
}
