using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompanyElixirUsersList;

public record GetCompanyElixirUsersListQuery(string? ScreenName = "") : IRequest<ElixirUserListDto>;

public class GetCompanyElixirUsersListQueryHandler : IRequestHandler<GetCompanyElixirUsersListQuery, ElixirUserListDto>
{
    private readonly IElixirUsersRepository _elixirUserRepository;

    public GetCompanyElixirUsersListQueryHandler(IElixirUsersRepository elixirUserRepository)
    {
        _elixirUserRepository = elixirUserRepository;
    }

    public async Task<ElixirUserListDto> Handle(GetCompanyElixirUsersListQuery request, CancellationToken cancellationToken)
    {
        return await _elixirUserRepository.GetUserListsFromUserGroupMappingAsync(request.ScreenName);
    }
}
