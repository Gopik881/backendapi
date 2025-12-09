using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Queries.GetClientUnmappedCompanies;

public record GetClientUnmappedCompaniesQuery() : IRequest<IEnumerable<ClientUnmappedCompaniesDto>>;

public class GetClientUnmappedComapniesQueryHandler : IRequestHandler<GetClientUnmappedCompaniesQuery, IEnumerable<ClientUnmappedCompaniesDto>>
{
    private readonly IClientsRepository _clientsRepository;

    public GetClientUnmappedComapniesQueryHandler(IClientsRepository clientsRepository)
    {
        _clientsRepository = clientsRepository;
    }

    public async Task<IEnumerable<ClientUnmappedCompaniesDto>> Handle(GetClientUnmappedCompaniesQuery request, CancellationToken cancellationToken)
    {
        return await _clientsRepository.GetClientUnmappedCompaniesAsync();
    }
}
