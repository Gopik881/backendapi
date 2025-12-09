using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetHorizontalsByGroupId;

public record GetUserGroupHorizontalsQuery(int userGroupId) : IRequest<List<UserGroupHorizontals>>;

public class GetUserGroupHorizontalsQueryHandler : IRequestHandler<GetUserGroupHorizontalsQuery, List<UserGroupHorizontals>>
{
    private readonly IHorizontalsRepository _horizontalsRepository;

    public GetUserGroupHorizontalsQueryHandler(IHorizontalsRepository horizontalsRepository)
    {
        _horizontalsRepository = horizontalsRepository;
    }

    public async Task<List<UserGroupHorizontals>> Handle(GetUserGroupHorizontalsQuery request, CancellationToken cancellationToken)
    {
        return await _horizontalsRepository.GetHorizontalsForRoleAsync(request.userGroupId);
    }
}
