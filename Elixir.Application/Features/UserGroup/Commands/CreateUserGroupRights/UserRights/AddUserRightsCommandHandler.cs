using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record AddUserRightsCommand(int UserGroupId, List<UserGroupMenuRights> UserRights) : IRequest<bool>;

public class AddUserRightsCommandHandler : IRequestHandler<AddUserRightsCommand, bool>
{
    private readonly IUserGroupsRepository _userGroupRepository;

    public AddUserRightsCommandHandler(IUserGroupsRepository userGroupRepository)
    {
        _userGroupRepository = userGroupRepository;
    }

    public async Task<bool> Handle(AddUserRightsCommand request, CancellationToken cancellationToken)
    {
        return await _userGroupRepository.AddUserRightsAsync(request.UserGroupId, request.UserRights);
    }
}