using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.User.Commands.DeleteUser;

public record DeleteUserCommand(int UserId) : IRequest<bool>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUsersRepository _userRepository;

    public DeleteUserCommandHandler(IUsersRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
            return await _userRepository.DeleteUserAsync(request.UserId);
    }
}
