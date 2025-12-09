using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record UpdateClientCommand(int userId, int clientId, ClientInfoDto ClientInfoDto) : IRequest<bool>;

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, bool>
{
    private readonly IClientsRepository _clientRepository;

    public UpdateClientCommandHandler(IClientsRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<bool> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        // Save client basic data
        return await _clientRepository.UpdateClientInformationAsync(request.ClientInfoDto,request.clientId, request.userId);
    }
}
