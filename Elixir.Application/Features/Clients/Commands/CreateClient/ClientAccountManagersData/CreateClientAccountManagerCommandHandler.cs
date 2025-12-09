using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Commands.CreateClient.ClientAccountManagersData;

public record CreateClientAccountManagerCommand(int userId, int clientId, List<ClientAccountManagersDto> ClientAccountManagers) : IRequest<bool>;

public class CreateClientAccountManagerCommandHandler : IRequestHandler<CreateClientAccountManagerCommand, bool>
{
    private readonly IElixirUsersHistoryRepository _clientAccountManagerRepository;

    public CreateClientAccountManagerCommandHandler(IElixirUsersHistoryRepository clientAccountManagerRepository)
    {
        _clientAccountManagerRepository = clientAccountManagerRepository;
    }

    public async Task<bool> Handle(CreateClientAccountManagerCommand request, CancellationToken cancellationToken)
    {
        // Save data
        return await _clientAccountManagerRepository.CreateClientAccountManagerDataAsync(request.userId, request.clientId, request.ClientAccountManagers);
    }
}
