using Elixir.Application.Common.Models;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Queries.GetPagedClientAccountManagers;

public record GetPagedClientsByUsersQuery(int UserId, int GroupId, string GroupName, string SearchTerm, int PageNumber, int PageSize) : IRequest<PaginatedResponse<ClientBasicInfoDto>>;

public class GetPagedClientsByUsersQueryHandler : IRequestHandler<GetPagedClientsByUsersQuery, PaginatedResponse<ClientBasicInfoDto>>
{
    private readonly IClientsRepository _clientsRepository;

    public GetPagedClientsByUsersQueryHandler(IClientsRepository clientsRepository)
    {
        _clientsRepository = clientsRepository;
    }

    public async Task<PaginatedResponse<ClientBasicInfoDto>> Handle(GetPagedClientsByUsersQuery request, CancellationToken cancellationToken)
    {
        var result = await _clientsRepository.GetFilteredClientsByUsersAsync(
            request.UserId,
            request.GroupId,
            request.GroupName,
            request.SearchTerm,
            request.PageNumber,
            request.PageSize
        );

        return new PaginatedResponse<ClientBasicInfoDto>(
            result.Item1,
            new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber)
        );
    }
}
