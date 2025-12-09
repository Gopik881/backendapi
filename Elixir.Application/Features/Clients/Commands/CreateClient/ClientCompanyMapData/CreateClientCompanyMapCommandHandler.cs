using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record CreateClientCompanyMapCommand(int userId, int clientId, List<ClientCompanyMappingDto> ClientCompanyMappingDtos, string clientName) : IRequest<bool>;

public class CreateClientCompanyMapCommandHandler : IRequestHandler<CreateClientCompanyMapCommand, bool>
{
    private readonly IClientCompaniesMappingRepository _clientCompanyMapRepository;

    public CreateClientCompanyMapCommandHandler(IClientCompaniesMappingRepository clientCompanyMapRepository)
    {
        _clientCompanyMapRepository = clientCompanyMapRepository;
    }

    public async Task<bool> Handle(CreateClientCompanyMapCommand request, CancellationToken cancellationToken)
    {
        // Save data
        return await _clientCompanyMapRepository.CreateClientCompanyMapDataAsync(request.userId, request.clientId, request.ClientCompanyMappingDtos, request.clientName);
    }
}
