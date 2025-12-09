using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetRoleNameByGroupId;

public record GetUserGroupRoleNameQuery(int userGroupId) : IRequest<string>;

public class GetUserGroupRoleNameQueryHandler : IRequestHandler<GetUserGroupRoleNameQuery, string>
{
    private readonly IUserGroupsRepository _userGroupRepository;

    public GetUserGroupRoleNameQueryHandler(IUserGroupsRepository userGroupRepository)
    {
        _userGroupRepository = userGroupRepository;
    }

    public async Task<string> Handle(GetUserGroupRoleNameQuery request, CancellationToken cancellationToken)
    {
        return await _userGroupRepository.GetRoleNameByGroupId(request.userGroupId);
    }
}
