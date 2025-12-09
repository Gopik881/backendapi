using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.UpdateUserRights;

public record UpdateUserRightsCommand(int UserGroupId, List<UserGroupMenuRights> UserRightsDtos) : IRequest<bool>;

public class UpdateUserRightsCommandHandler : IRequestHandler<UpdateUserRightsCommand, bool>
{
    private readonly IUserGroupsRepository _userRightRepository;

    public UpdateUserRightsCommandHandler(IUserGroupsRepository userRightRepository)
    {
        _userRightRepository = userRightRepository;
    }

    public async Task<bool> Handle(UpdateUserRightsCommand request, CancellationToken cancellationToken)
    {
        // Save user rights data for the user group
        return await _userRightRepository.UpdateUserRightsAsync(request.UserRightsDtos, request.UserGroupId);
    }
}
