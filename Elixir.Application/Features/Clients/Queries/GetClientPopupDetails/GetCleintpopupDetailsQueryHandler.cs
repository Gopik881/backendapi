using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record GetCleintpopupDetailsQuery(int ClientId) : IRequest<ClientPopupDetailsDto>;

public class GetCleintpopupDetailsQueryHandler : IRequestHandler<GetCleintpopupDetailsQuery, ClientPopupDetailsDto>
{
    private readonly IClientsRepository _clientsRepository;

    public GetCleintpopupDetailsQueryHandler(IClientsRepository clientsRepository)
    {
        _clientsRepository = clientsRepository;
    }

    public async Task<ClientPopupDetailsDto> Handle(GetCleintpopupDetailsQuery request, CancellationToken cancellationToken)
    {
        return await _clientsRepository.GetClientDetailsAsync(request.ClientId);
    }
}
