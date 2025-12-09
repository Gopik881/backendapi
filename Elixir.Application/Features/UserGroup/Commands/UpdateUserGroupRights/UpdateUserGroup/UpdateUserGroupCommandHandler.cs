using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.UpdateUserGroup;

public record UpdateUserGroupCommand(int UserGroupId, CreateUserGroupDto UserGroupDtos) : IRequest<bool>;

public class UpdateUserGroupCommandHandler : IRequestHandler<UpdateUserGroupCommand, bool>
{
    private readonly IUserGroupsRepository _userGroupRepository;

    public UpdateUserGroupCommandHandler(IUserGroupsRepository userGroupRepository)
    {
        _userGroupRepository = userGroupRepository;
    }

    public async Task<bool> Handle(UpdateUserGroupCommand request, CancellationToken cancellationToken)
    {
        // Save user group data
        return await _userGroupRepository.UpdateUserGroupAsync(request.UserGroupDtos);
    }
}
