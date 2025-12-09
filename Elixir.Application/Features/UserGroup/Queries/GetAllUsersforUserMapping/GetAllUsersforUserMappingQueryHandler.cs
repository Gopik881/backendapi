using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Queries.GetAllUsersforUserMapping;

public record GetAllUsersforUserMappingQuery() : IRequest<IEnumerable<UserListforUserMappingDto>>;

public class GetAllUsersforUserMappingQueryHandler : IRequestHandler<GetAllUsersforUserMappingQuery, IEnumerable<UserListforUserMappingDto>>
{
    private readonly IUserGroupMappingsRepository _userGroupMappingsRepository;

    public GetAllUsersforUserMappingQueryHandler(IUserGroupMappingsRepository userGroupMappingsRepository)
    {
        _userGroupMappingsRepository = userGroupMappingsRepository;
    }

    public async Task<IEnumerable<UserListforUserMappingDto>> Handle(GetAllUsersforUserMappingQuery request, CancellationToken cancellationToken)
    {
        return await _userGroupMappingsRepository.GetAllUsersforUserMappingAsync();
    }
}
