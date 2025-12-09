using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Enums;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Commands.UserGroupMapping.AddUserGroupMapping;

public record AddUserGroupMappingCommand(List<int> UserIds, int userId, int groupId) : IRequest<bool>;

public class AddUserGroupMappingCommandHandler : IRequestHandler<AddUserGroupMappingCommand, bool>
{
    private readonly IUserGroupMappingsRepository _userGroupMappingsRepository;
    private readonly INotificationsRepository _notificationsRepostory;

    public AddUserGroupMappingCommandHandler(IUserGroupMappingsRepository userGroupMappingsRepository, INotificationsRepository notificationsRepostory)
    {
        _userGroupMappingsRepository = userGroupMappingsRepository;
        _notificationsRepostory = notificationsRepostory;
    }

    public async Task<bool> Handle(AddUserGroupMappingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _userGroupMappingsRepository.AddUserGroupMappingAsync(request.groupId, request.UserIds);

            if (result && request.UserIds != null && request.UserIds.Count > 0)
            {
                // Fetch group name for notification message
                var userGroups = await _userGroupMappingsRepository.GetUserAssociatedGroupAsync(request.groupId);
                var group = userGroups?.FirstOrDefault(g => g.GroupId == request.groupId);
                var groupName = group?.GroupName ?? "User Group";

                var notifications = request.UserIds.Select(userId => new NotificationDto
                {
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = request.userId,
                    UpdatedBy = request.userId,
                    Title = "User Mapping",
                    Message = $"Your user rights have been updated.",
                    NotificationType = "Info",
                    IsRead = false,
                    IsDeleted = false,
                    UserId = userId,
                    IsActive = false
                }).ToList();

                foreach (var notification in notifications)
                {
                    await _notificationsRepostory.InsertNotificationAsync(notification);
                }
            }

            return result;
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);//AppConstants.ErrorCodes.USER_GROUP_USER_NOT_ADDED
        }
    }
}
