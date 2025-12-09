using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record GetClientAdminByClientIdQuery(int clientId) : IRequest<ClientAdminInfoDto?>;

public class GetClientAdminByClientIdQueryHandler : IRequestHandler<GetClientAdminByClientIdQuery, ClientAdminInfoDto?>
{
    private readonly IClientAdminInfoRepository _clientAdminRepository;

    public GetClientAdminByClientIdQueryHandler(IClientAdminInfoRepository clientAdminRepository)
    {
        _clientAdminRepository = clientAdminRepository;
    }

    public async Task<ClientAdminInfoDto?> Handle(GetClientAdminByClientIdQuery request, CancellationToken cancellationToken)
    {
        // Retrieve admin data by clientId
        return await _clientAdminRepository.GetClientAdminInfoByClientIdAsync(request.clientId);
    }
}
