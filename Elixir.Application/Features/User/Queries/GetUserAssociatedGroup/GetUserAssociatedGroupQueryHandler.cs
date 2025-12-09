using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.User.Queries.GetUserAssociatedGroup;

public record GetUserAssociatedGroupQuery(int UserId) : IRequest<IEnumerable<UserGroupDto>>;
public class GetUserAssociatedGroupQueryHandler : IRequestHandler<GetUserAssociatedGroupQuery, IEnumerable<UserGroupDto>>
{
    private readonly IUserGroupMappingsRepository _userGroupMappingsRepository;
    public GetUserAssociatedGroupQueryHandler(IUserGroupMappingsRepository userGroupMappingsRepository)
    {
        _userGroupMappingsRepository = userGroupMappingsRepository;
    }
    public async Task<IEnumerable<UserGroupDto>> Handle(GetUserAssociatedGroupQuery request, CancellationToken cancellationToken)
    {
        return await _userGroupMappingsRepository.GetUserAssociatedGroupAsync(request.UserId);
    }
}
