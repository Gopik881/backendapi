using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteUserRights
{
    public record DeleteUserRightsCommand(int UserGroupId) : IRequest<bool>;

    public class DeleteUserRightsCommandHandler : IRequestHandler<DeleteUserRightsCommand, bool>
    {
        private readonly IUserGroupMenuMappingRepository _userGroupsMenuMappingRepository;

        public DeleteUserRightsCommandHandler(IUserGroupMenuMappingRepository userGroupsMenuMappingRepository)
        {
            _userGroupsMenuMappingRepository = userGroupsMenuMappingRepository;
        }

        public async Task<bool> Handle(DeleteUserRightsCommand request, CancellationToken cancellationToken)
        {
            return await _userGroupsMenuMappingRepository.DeleteUserRightsByUserGroupIdAsync(request.UserGroupId);
        }
    }
}
