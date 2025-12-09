using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Queries.GetUserMappingsGroupNames;

public record GetUserMappingGroupNamesByGroupTypeQuery() : IRequest<IEnumerable<UserMappingGroupsByGroupTypeDto>>;

public class GetUserMappingGroupNamesByGroupTypeQueryHandler : IRequestHandler<GetUserMappingGroupNamesByGroupTypeQuery, IEnumerable<UserMappingGroupsByGroupTypeDto>>
{
    private readonly IUserGroupMappingsRepository _userGroupMappingsRepository;

    public GetUserMappingGroupNamesByGroupTypeQueryHandler(IUserGroupMappingsRepository userGroupMappingsRepository)
    {
        _userGroupMappingsRepository = userGroupMappingsRepository;
    }

    public async Task<IEnumerable<UserMappingGroupsByGroupTypeDto>> Handle(GetUserMappingGroupNamesByGroupTypeQuery request, CancellationToken cancellationToken)
    {
        return await _userGroupMappingsRepository.GetUserMappingGroupNamesByGroupTypeAsync();
    }
}
