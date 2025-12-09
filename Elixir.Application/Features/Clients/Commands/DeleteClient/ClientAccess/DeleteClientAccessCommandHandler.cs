using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Commands.DeleteClient.ClientAccess
{
    public record DeleteClientAccessCommand(int ClientId) : IRequest<bool>;

    public class DeleteClientAccessCommandHandler : IRequestHandler<DeleteClientAccessCommand, bool>
    {
        private readonly IClientAccessRepository _clientAccessRepository;
        private readonly ICompanyOnboardingStatusRepository _companyOnboardingStatusRepository;

        public DeleteClientAccessCommandHandler(IClientAccessRepository clientAccessRepository, ICompanyOnboardingStatusRepository companyOnboardingStatusRepository)
        {
            _clientAccessRepository = clientAccessRepository;
            _companyOnboardingStatusRepository = companyOnboardingStatusRepository;
        }

        public async Task<bool> Handle(DeleteClientAccessCommand request, CancellationToken cancellationToken)
        {
            int companyId = await _companyOnboardingStatusRepository.GetCompanyIdByClientIdAsync(request.ClientId);
            bool clientExists = await _clientAccessRepository.GetClientByCompanyIdAsync(companyId);
            return await _clientAccessRepository.DeleteClientAccessAsync(request.ClientId);
        }
    }
}
