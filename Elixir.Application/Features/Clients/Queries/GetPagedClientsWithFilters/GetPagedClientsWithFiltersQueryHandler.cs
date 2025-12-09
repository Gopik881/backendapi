using Elixir.Application.Common.Models;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Queries.GetPagedClientsWithFilters;

public record GetPagedClientsWithFiltersQuery(int PageNumber, int PageSize, string? SearchTerm) : IRequest<PaginatedResponse<ClientDto>>;

public class GetPagedClientsWithFiltersQueryHandler : IRequestHandler<GetPagedClientsWithFiltersQuery, PaginatedResponse<ClientDto>>
{
    private readonly IClientsRepository _clientRepository;

    public GetPagedClientsWithFiltersQueryHandler(IClientsRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<PaginatedResponse<ClientDto>> Handle(GetPagedClientsWithFiltersQuery request, CancellationToken cancellationToken)
    {
        var result = await _clientRepository.GetFilteredClientsAsync(request.PageNumber, request.PageSize, request.SearchTerm);
        return new PaginatedResponse<ClientDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
    }
}
