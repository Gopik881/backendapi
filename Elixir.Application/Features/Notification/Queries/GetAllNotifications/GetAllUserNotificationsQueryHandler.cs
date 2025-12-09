using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Notification.Queries.GetAllNotifications;

public record GetAllUserNotificationsQuery(int UserId) : IRequest<IEnumerable<NotificationDto>>;

public class GetAllUserNotificationsQueryHandler : IRequestHandler<GetAllUserNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly INotificationsRepository _notificationRepository;

    public GetAllUserNotificationsQueryHandler(INotificationsRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetAllUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        return await _notificationRepository.GetAllUserNotificationsAsync(request.UserId);
    }
}
