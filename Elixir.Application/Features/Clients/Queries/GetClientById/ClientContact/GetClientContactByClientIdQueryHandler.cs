using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Queries.GetClientById.ClientContact;

public record GetClientContactByClientIdQuery(int clientId) : IRequest<List<ClientContactInfoDto>>;

public class GetClientContactByClientIdQueryHandler : IRequestHandler<GetClientContactByClientIdQuery, List<ClientContactInfoDto>>
{
    private readonly IClientContactDetailsRepository _clientContactRepository;

    public GetClientContactByClientIdQueryHandler(IClientContactDetailsRepository clientContactRepository)
    {
        _clientContactRepository = clientContactRepository;
    }

    public async Task<List<ClientContactInfoDto>> Handle(GetClientContactByClientIdQuery request, CancellationToken cancellationToken)
    {
        return await _clientContactRepository.GetClientContactDataByClientIdAsync(request.clientId);
    }
}
