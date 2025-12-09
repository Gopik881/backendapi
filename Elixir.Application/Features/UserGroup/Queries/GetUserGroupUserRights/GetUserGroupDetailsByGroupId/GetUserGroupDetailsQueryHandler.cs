using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetUserGroupDetailsByGroupId;

public record GetUserGroupDetailsQuery(int userGroupId) : IRequest<UserGroupDto>;

public class GetUserGroupDetailsQueryHandler : IRequestHandler<GetUserGroupDetailsQuery, UserGroupDto>
{
    private readonly IUserGroupsRepository _userGroupRepository;

    public GetUserGroupDetailsQueryHandler(IUserGroupsRepository userGroupRepository)
    {
        _userGroupRepository = userGroupRepository;
    }

    public async Task<UserGroupDto> Handle(GetUserGroupDetailsQuery request, CancellationToken cancellationToken)
    {
        return await _userGroupRepository.GetUserGroupByIdAsync(request.userGroupId);
    }
}
