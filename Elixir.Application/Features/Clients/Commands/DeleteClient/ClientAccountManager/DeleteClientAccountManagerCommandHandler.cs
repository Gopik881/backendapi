using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Commands.DeleteClient.ClientAccountManager
{
    public record DeleteClientAccountManagerCommand(int ClientId) : IRequest<bool>;

    public class DeleteClientAccountManagerCommandHandler : IRequestHandler<DeleteClientAccountManagerCommand, bool>
    {
        private readonly IClientsRepository _clientAccountManagerRepository;

        public DeleteClientAccountManagerCommandHandler(IClientsRepository clientAccountManagerRepository)
        {
            _clientAccountManagerRepository = clientAccountManagerRepository;
        }

        public async Task<bool> Handle(DeleteClientAccountManagerCommand request, CancellationToken cancellationToken)
        {
            return await _clientAccountManagerRepository.DeleteClientAccountManagersAsync(request.ClientId);
        }
    }
}
