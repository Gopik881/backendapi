using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record UpdateClientCompanyMappingCommand(int userId, int clientId, List<ClientCompanyMappingDto> ClientCompanyMappingDtos, string clientName, bool IsSuperUser) : IRequest<bool>;

public class UpdateClientCompanyMappingCommandHandler : IRequestHandler<UpdateClientCompanyMappingCommand, bool>
{
    private readonly IClientCompaniesMappingRepository _clientCompanyMapRepository;

    public UpdateClientCompanyMappingCommandHandler(IClientCompaniesMappingRepository clientCompanyMapRepository)
    {
        _clientCompanyMapRepository = clientCompanyMapRepository;
    }

    public async Task<bool> Handle(UpdateClientCompanyMappingCommand request, CancellationToken cancellationToken)
    {
        // Update data
        return await _clientCompanyMapRepository.UpdateClientCompanyMapDataAsync(request.userId, request.clientId, request.ClientCompanyMappingDtos, request.clientName, request.IsSuperUser);
    }
}
