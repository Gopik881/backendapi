using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Commands.CreateClient.ClientContactData;

public record CreateClientContactCommand(int userId, int clientId, List<ClientContactInfoDto> ClientContactInfoDto) : IRequest<bool>;

public class CreateClientContactCommandHandler : IRequestHandler<CreateClientContactCommand, bool>
{
    private readonly IClientContactDetailsRepository _clientContactRepository;

    public CreateClientContactCommandHandler(IClientContactDetailsRepository clientContactRepository)
    {
        _clientContactRepository = clientContactRepository;
    }

    public async Task<bool> Handle(CreateClientContactCommand request, CancellationToken cancellationToken)
    {
        // Save contact data
        return await _clientContactRepository.CreateClientContactDataAsync(request.userId, request.clientId, request.ClientContactInfoDto);

    }
}
