using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Commands.UpdateClient.UpdateClient.ClientContact;

public record UpdateClientContactCommand(int userId, int clientId, List<ClientContactInfoDto> ClientContactInfoDto) : IRequest<bool>;

public class UpdateClientContactCommandHandler : IRequestHandler<UpdateClientContactCommand, bool>
{
    private readonly IClientContactDetailsRepository _clientContactRepository;

    public UpdateClientContactCommandHandler(IClientContactDetailsRepository clientContactRepository)
    {
        _clientContactRepository = clientContactRepository;
    }

    public async Task<bool> Handle(UpdateClientContactCommand request, CancellationToken cancellationToken)
    {
        // Update contact data
        return await _clientContactRepository.UpdateClientContactDataAsync(request.userId, request.clientId, request.ClientContactInfoDto);
    }
}
