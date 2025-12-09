using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Notification.Commands.UpdateNotification.UpdateUser;

public record UpdateUserMarkAsReadNotificationCommand(int NotificationId) : IRequest<bool>;

public class UpdateUserMarkAsReadNotificationCommandHandler : IRequestHandler<UpdateUserMarkAsReadNotificationCommand, bool>
{
    private readonly INotificationsRepository _userNotificationRepository;

    public UpdateUserMarkAsReadNotificationCommandHandler(INotificationsRepository userNotificationRepository)
    {
        _userNotificationRepository = userNotificationRepository;
    }

    public async Task<bool> Handle(UpdateUserMarkAsReadNotificationCommand request, CancellationToken cancellationToken)
    {
        return await _userNotificationRepository.UpdateUserMarkAsReadAsync(request.NotificationId);
    }
}
