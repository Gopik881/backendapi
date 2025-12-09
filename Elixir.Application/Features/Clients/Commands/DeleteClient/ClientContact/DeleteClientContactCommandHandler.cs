using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Commands.DeleteClient.ClientContact
{
    public record DeleteClientContactCommand(int ClientContactId) : IRequest<bool>;

    public class DeleteClientContactCommandHandler : IRequestHandler<DeleteClientContactCommand, bool>
    {
        private readonly IClientContactDetailsRepository _clientContactRepository;

        public DeleteClientContactCommandHandler(IClientContactDetailsRepository clientContactRepository)
        {
            _clientContactRepository = clientContactRepository;
        }

        public async Task<bool> Handle(DeleteClientContactCommand request, CancellationToken cancellationToken)
        {
            return await _clientContactRepository.DeleteClientContactDetailsAsync(request.ClientContactId);
        }
    }
}
