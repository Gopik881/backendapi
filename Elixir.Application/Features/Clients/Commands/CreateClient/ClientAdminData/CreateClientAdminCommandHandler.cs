using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record CreateClientAdminCommand(int userId, int clientId, ClientAdminInfoDto ClientAdminInfoDto) : IRequest<bool>;

public class CreateClientAdminCommandHandler : IRequestHandler<CreateClientAdminCommand, bool>
{
    private readonly IClientAdminInfoRepository _clientAdminRepository;

    public CreateClientAdminCommandHandler(IClientAdminInfoRepository clientAdminRepository)
    {
        _clientAdminRepository = clientAdminRepository;
    }

    public async Task<bool> Handle(CreateClientAdminCommand request, CancellationToken cancellationToken)
    {
        // Save admin data
        return await _clientAdminRepository.CreateClientAdminInfoDataAsync(request.userId, request.clientId, request.ClientAdminInfoDto);
    }
}
