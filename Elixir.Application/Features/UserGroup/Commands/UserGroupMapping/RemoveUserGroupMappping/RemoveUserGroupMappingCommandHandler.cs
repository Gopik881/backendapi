using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Enums;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Features.UserGroup.Queries.GetCreatedUserGroupUsersByGroupId;
using Elixir.Application.Interfaces.Persistance;

using MediatR;

namespace Elixir.Application.Features.UserGroup.Commands.UserGroupMapping.RemoveUserGroupMappping;

public record RemoveUserGroupMappingCommand(List<int> UserIds, int userId, int GroupId) : IRequest<bool>;

public class RemoveUserGroupMappingCommandHandler : IRequestHandler<RemoveUserGroupMappingCommand, bool>
{
    private readonly IUserGroupMappingsRepository _userGroupMappingsRepository;
    private readonly INotificationsRepository _notificationsRepository;

    public RemoveUserGroupMappingCommandHandler(IUserGroupMappingsRepository userGroupMappingsRepository, INotificationsRepository notificationsRepository)
    {
        _userGroupMappingsRepository = userGroupMappingsRepository;
        _notificationsRepository = notificationsRepository;
    }

    public async Task<bool> Handle(RemoveUserGroupMappingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _userGroupMappingsRepository.RemoveUserGroupMappingAsync(request.GroupId, request.UserIds);

            if (result && request.UserIds != null && request.UserIds.Count > 0)
            {
                // Fetch group name for notification message
                var userGroups = await _userGroupMappingsRepository.GetUserAssociatedGroupAsync(request.GroupId);
                var group = userGroups?.FirstOrDefault(g => g.GroupId == request.GroupId);
                var resolvedGroupName = group?.GroupName ?? "User Group";

                var notifications = request.UserIds.Select(userId => new NotificationDto
                {
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = request.userId,
                    UpdatedBy = request.userId,
                    Title = "User Unmapping",
                    Message = $"You have been removed from the group '{resolvedGroupName}'", //on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC.",
                    NotificationType = "Info",
                    IsRead = false,
                    IsDeleted = false,
                    UserId = userId,
                    IsActive = false
                }).ToList();

                foreach (var notification in notifications)
                {
                    await _notificationsRepository.InsertNotificationAsync(notification);
                }
            }

            return result;
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
