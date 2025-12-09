using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Commands.DeleteClient.ClientCompanyMapping
{
    public record DeleteClientCompanyMappingCommand(int ClientId) : IRequest<bool>;

    public class DeleteClientCompanyMappingCommandHandler : IRequestHandler<DeleteClientCompanyMappingCommand, bool>
    {
        private readonly IClientCompaniesMappingRepository _clientCompanyMappingRepository;

        public DeleteClientCompanyMappingCommandHandler(IClientCompaniesMappingRepository clientCompanyMappingRepository)
        {
            _clientCompanyMappingRepository = clientCompanyMappingRepository;
        }

        public async Task<bool> Handle(DeleteClientCompanyMappingCommand request, CancellationToken cancellationToken)
        {
            return await _clientCompanyMappingRepository.DeleteClientCompaniesMappingsAsync(request.ClientId);
        }
    }
}
