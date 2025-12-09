using Elixir.Application.Common.Models;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Notification.Queries.GetPagedNotifications;

public record GetPagedNotificationsQuery(int UserId, bool IsSuperAdmin, int PageNumber, int PageSize, string? SearchTerm) : IRequest<PaginatedResponse<NotificationDto>>;

public class GetPagedNotificationsQueryHandler : IRequestHandler<GetPagedNotificationsQuery, PaginatedResponse<NotificationDto>>
{
    private readonly INotificationsRepository _notificationsRepository;

    public GetPagedNotificationsQueryHandler(INotificationsRepository notificationsRepository)
    {
        _notificationsRepository = notificationsRepository;
    }

    public async Task<PaginatedResponse<NotificationDto>> Handle(GetPagedNotificationsQuery request, CancellationToken cancellationToken)
    {
        var result = await _notificationsRepository.GetFilteredNotificationsAsync(request.UserId, request.IsSuperAdmin, request.PageNumber, request.PageSize, request.SearchTerm);
        return new PaginatedResponse<NotificationDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
    }
}
