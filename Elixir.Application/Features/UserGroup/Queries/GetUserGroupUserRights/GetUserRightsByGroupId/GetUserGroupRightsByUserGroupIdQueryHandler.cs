using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetUserRightsByGroupId;

public record GetUserGroupRightsByUserGroupIdQuery(int userGroupId) : IRequest<object>;

public class GetUserGroupRightsByUserGroupIdQueryHandler : IRequestHandler<GetUserGroupRightsByUserGroupIdQuery, object>
{
    private readonly IUserGroupsRepository _userGroupRepository;

    public GetUserGroupRightsByUserGroupIdQueryHandler(IUserGroupsRepository userGroupRepository)
    {
        _userGroupRepository = userGroupRepository;
    }

    public async Task<object> Handle(GetUserGroupRightsByUserGroupIdQuery request, CancellationToken cancellationToken)
    {
        return await _userGroupRepository.GetUserRightsByUserGroupId(request.userGroupId);
    }
}