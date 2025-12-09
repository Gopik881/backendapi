using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteUserGroupDetails
{
    public record DeleteUserGroupDetailsCommand(int UserGroupId) : IRequest<bool>;

    public class DeleteUserGroupDetailsCommandHandler : IRequestHandler<DeleteUserGroupDetailsCommand, bool>
    {
        private readonly IUserGroupsRepository _userGroupDetailsRepository;

        public DeleteUserGroupDetailsCommandHandler(IUserGroupsRepository userGroupDetailsRepository)
        {
            _userGroupDetailsRepository = userGroupDetailsRepository;
        }

        public async Task<bool> Handle(DeleteUserGroupDetailsCommand request, CancellationToken cancellationToken)
        {
            return await _userGroupDetailsRepository.DeleteUserGroupDetailsByUserGroupIdAsync(request.UserGroupId);
        }
    }
}
