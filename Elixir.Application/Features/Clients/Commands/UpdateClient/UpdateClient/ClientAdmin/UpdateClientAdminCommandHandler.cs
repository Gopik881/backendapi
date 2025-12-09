using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record UpdateClientAdminCommand(int userId, int clientId, ClientAdminInfoDto ClientAdminInfoDto) : IRequest<bool>;

public class UpdateClientAdminCommandHandler : IRequestHandler<UpdateClientAdminCommand, bool>
{
    private readonly IClientAdminInfoRepository _clientAdminRepository;

    public UpdateClientAdminCommandHandler(IClientAdminInfoRepository clientAdminRepository)
    {
        _clientAdminRepository = clientAdminRepository;
    }

    public async Task<bool> Handle(UpdateClientAdminCommand request, CancellationToken cancellationToken)
    {
        // Update admin data
        return await _clientAdminRepository.UpdateClientAdminInfoDataAsync(request.userId, request.clientId, request.ClientAdminInfoDto);
    }
}
