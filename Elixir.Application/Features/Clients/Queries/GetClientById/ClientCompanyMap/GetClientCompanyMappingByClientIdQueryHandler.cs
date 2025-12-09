using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record GetClientCompanyMappingByClientIdQuery(int clientId) : IRequest<List<ClientCompanyMappingDto>>;

public class GetClientCompanyMappingByClientIdQueryHandler : IRequestHandler<GetClientCompanyMappingByClientIdQuery, List<ClientCompanyMappingDto>>
{
    private readonly IClientCompaniesMappingRepository _clientCompanyMapRepository;

    public GetClientCompanyMappingByClientIdQueryHandler(IClientCompaniesMappingRepository clientCompanyMapRepository)
    {
        _clientCompanyMapRepository = clientCompanyMapRepository;
    }

    public async Task<List<ClientCompanyMappingDto>> Handle(GetClientCompanyMappingByClientIdQuery request, CancellationToken cancellationToken)
    {
        return await _clientCompanyMapRepository.GetClientCompanyMappingByClientIdAsync(request.clientId);
    }
}
