using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Commands.DeleteClient.ClientAdmin
{
    public record DeleteClientAdminCommand(int ClientId) : IRequest<bool>;

    public class DeleteClientAdminCommandHandler : IRequestHandler<DeleteClientAdminCommand, bool>
    {
        private readonly IClientAdminInfoRepository _clientAdminRepository;

        public DeleteClientAdminCommandHandler(IClientAdminInfoRepository clientAdminRepository)
        {
            _clientAdminRepository = clientAdminRepository;
        }

        public async Task<bool> Handle(DeleteClientAdminCommand request, CancellationToken cancellationToken)
        {
            return await _clientAdminRepository.DeleteClientAdminInfosAsync(request.ClientId);
        }
    }
}
