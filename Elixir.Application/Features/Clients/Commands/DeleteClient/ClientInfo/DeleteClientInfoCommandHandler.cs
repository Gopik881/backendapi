using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Commands.DeleteClient.ClientInfo
{
    public record DeleteClientInfoCommand(int ClientId) : IRequest<bool>;

    public class DeleteClientInfoCommandHandler : IRequestHandler<DeleteClientInfoCommand, bool>
    {
        private readonly IClientsRepository _clientsRepository;

        public DeleteClientInfoCommandHandler(IClientsRepository clientsRepository)
        {
            _clientsRepository = clientsRepository;
        }

        public async Task<bool> Handle(DeleteClientInfoCommand request, CancellationToken cancellationToken)
        {
            return await _clientsRepository.DeleteClientInfoAsync(request.ClientId);
        }
    }
}
