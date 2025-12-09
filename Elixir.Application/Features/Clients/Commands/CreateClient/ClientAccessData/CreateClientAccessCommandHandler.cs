using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record CreateClientAccessCommand(int userId, int clientId, ClientAccessDto ClientAccessDto) : IRequest<bool>;

public class CreateClientAccessCommandHandler : IRequestHandler<CreateClientAccessCommand, bool>
{
    private readonly IClientAccessRepository _clientAccessRepository;

    public CreateClientAccessCommandHandler(IClientAccessRepository clientAccessRepository)
    {
        _clientAccessRepository = clientAccessRepository;
    }

    public async Task<bool> Handle(CreateClientAccessCommand request, CancellationToken cancellationToken)
    {
        // Save data
        return await _clientAccessRepository.CreateClientAccessDataAsync(request.userId, request.clientId, request.ClientAccessDto);
    }
}
