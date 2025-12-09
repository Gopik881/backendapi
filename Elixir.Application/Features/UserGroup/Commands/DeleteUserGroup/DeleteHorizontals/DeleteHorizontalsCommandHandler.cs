using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteHorizontals
{
    public record DeleteHorizontalsCommand(int UserGroupId) : IRequest<bool>;

    public class DeleteHorizontalsCommandHandler : IRequestHandler<DeleteHorizontalsCommand, bool>
    {
        private readonly IHorizontalsRepository _horizontalRepository;

        public DeleteHorizontalsCommandHandler(IHorizontalsRepository horizontalRepository)
        {
            _horizontalRepository = horizontalRepository;
        }

        public async Task<bool> Handle(DeleteHorizontalsCommand request, CancellationToken cancellationToken)
        {
            return await _horizontalRepository.DeleteHorizontalsByUserGroupIdAsync(request.UserGroupId);
        }
    }
}
