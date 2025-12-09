using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Commands.UpdateClient.UpdateClient.ClientAccountManagers;

public record UpdateClientAccountManagerCommand(int userId, int clientId, List<ClientAccountManagersDto> ClientAccountManagers) : IRequest<bool>;

public class UpdateClientAccountManagerCommandHandler : IRequestHandler<UpdateClientAccountManagerCommand, bool>
{
    private readonly IElixirUsersHistoryRepository _clientAccountManagerRepository;

    public UpdateClientAccountManagerCommandHandler(IElixirUsersHistoryRepository clientAccountManagerRepository)
    {
        _clientAccountManagerRepository = clientAccountManagerRepository;
    }

    public async Task<bool> Handle(UpdateClientAccountManagerCommand request, CancellationToken cancellationToken)
    {
        // Update data
        // Assuming the repository has an Update method, otherwise implement accordingly
        return await _clientAccountManagerRepository.UpdateClientAccountManagerDataAsync(request.userId, request.clientId, request.ClientAccountManagers);
    }
}
