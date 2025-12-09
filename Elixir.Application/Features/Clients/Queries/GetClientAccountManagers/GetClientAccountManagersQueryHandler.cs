using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Elixir.Application.Features.Clients.Queries.GetClientAccountManagers
{
    public record GetClientAccountManagersQuery() : IRequest<IEnumerable<ClientGroupswithAccountManagersDto>>;

    public class GetClientAccountManagersQueryHandler : IRequestHandler<GetClientAccountManagersQuery, IEnumerable<ClientGroupswithAccountManagersDto>>
    {
        private readonly IClientsRepository _clientAccountManagersRepository;

        public GetClientAccountManagersQueryHandler(IClientsRepository clientAccountManagersRepository)
        {
            _clientAccountManagersRepository = clientAccountManagersRepository;
        }

        public async Task<IEnumerable<ClientGroupswithAccountManagersDto>> Handle(GetClientAccountManagersQuery request, CancellationToken cancellationToken)
        {
            return await _clientAccountManagersRepository.GetClientGroupswithAccountManagersAsync();
        }
    }
}
