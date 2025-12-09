using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Commands.UpdateClient.UpdateClient.ClientAccess;

public record UpdateClientAccessCommand(int UserId, int clientId, ClientAccessDto ClientAccessDto) : IRequest<bool>;

public class UpdateClientAccessCommandHandler : IRequestHandler<UpdateClientAccessCommand, bool>
{
    private readonly IClientAccessRepository _clientAccessRepository;

    public UpdateClientAccessCommandHandler(IClientAccessRepository clientAccessRepository)
    {
        _clientAccessRepository = clientAccessRepository;
    }

    public async Task<bool> Handle(UpdateClientAccessCommand request, CancellationToken cancellationToken)
    {
        // Save client access data
        return await _clientAccessRepository.UpdateClientAccessAsync(request.ClientAccessDto, request.clientId, request.UserId);
    }
}
