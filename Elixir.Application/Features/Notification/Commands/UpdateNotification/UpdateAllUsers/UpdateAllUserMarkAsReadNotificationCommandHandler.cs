using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Notification.Commands.UpdateNotification.UpdateAllUsers;

public record UpdateAllUserMarkAsReadNotificationCommand(int userId) : IRequest<bool>;

public class UpdateAllUserMarkAsReadNotificationCommandHandler : IRequestHandler<UpdateAllUserMarkAsReadNotificationCommand, bool>
{
    private readonly INotificationsRepository _userNotificationRepository;

    public UpdateAllUserMarkAsReadNotificationCommandHandler(INotificationsRepository userNotificationRepository)
    {
        _userNotificationRepository = userNotificationRepository;
    }

    public async Task<bool> Handle(UpdateAllUserMarkAsReadNotificationCommand request, CancellationToken cancellationToken)
    {
        return await _userNotificationRepository.UpdateAllUserMarkAsReadAsync(request.userId);
    }
}
