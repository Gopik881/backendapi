using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Features.UserGroup.Commands.UserGroupMapping.RemoveUserGroupMappping;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class RemoveUserGroupMappingCommandHandlerTests
{
    [Fact]
    public async Task Handle_RemoveSuccessWithUserIds_CreatesNotificationsAndReturnsTrue()
    {
        // Arrange
        var groupId = 1;
        var userId = 10;
        var userIds = new List<int> { 2, 3 };
        var groupName = "Test Group";
        var userGroups = new List<UserGroupDto> { new UserGroupDto { GroupId = groupId, GroupName = groupName } };

        var userGroupMappingsRepo = new Mock<IUserGroupMappingsRepository>();
        var notificationsRepo = new Mock<INotificationsRepository>();

        userGroupMappingsRepo.Setup(r => r.RemoveUserGroupMappingAsync(groupId, userIds))
            .ReturnsAsync(true);
        userGroupMappingsRepo.Setup(r => r.GetUserAssociatedGroupAsync(groupId))
            .ReturnsAsync(userGroups.AsEnumerable());

        notificationsRepo.Setup(r => r.InsertNotificationAsync(It.IsAny<NotificationDto>()))
            .ReturnsAsync(true);

        var handler = new RemoveUserGroupMappingCommandHandler(userGroupMappingsRepo.Object, notificationsRepo.Object);
        var command = new RemoveUserGroupMappingCommand(userIds, userId, groupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        userGroupMappingsRepo.Verify(r => r.RemoveUserGroupMappingAsync(groupId, userIds), Times.Once);
        userGroupMappingsRepo.Verify(r => r.GetUserAssociatedGroupAsync(groupId), Times.Once);
        notificationsRepo.Verify(r => r.InsertNotificationAsync(It.Is<NotificationDto>(n => userIds.Contains(n.UserId))), Times.Exactly(userIds.Count));
    }

    [Fact]
    public async Task Handle_RemoveSuccessWithNullUserIds_DoesNotCreateNotifications()
    {
        // Arrange
        var groupId = 1;
        var userId = 10;
        List<int>? userIds = null;

        var userGroupMappingsRepo = new Mock<IUserGroupMappingsRepository>();
        var notificationsRepo = new Mock<INotificationsRepository>();

        userGroupMappingsRepo.Setup(r => r.RemoveUserGroupMappingAsync(groupId, userIds))
            .ReturnsAsync(true);

        var handler = new RemoveUserGroupMappingCommandHandler(userGroupMappingsRepo.Object, notificationsRepo.Object);
        var command = new RemoveUserGroupMappingCommand(userIds, userId, groupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        notificationsRepo.Verify(r => r.InsertNotificationAsync(It.IsAny<NotificationDto>()), Times.Never);
        userGroupMappingsRepo.Verify(r => r.GetUserAssociatedGroupAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RemoveSuccessWithEmptyUserIds_DoesNotCreateNotifications()
    {
        // Arrange
        var groupId = 1;
        var userId = 10;
        var userIds = new List<int>();

        var userGroupMappingsRepo = new Mock<IUserGroupMappingsRepository>();
        var notificationsRepo = new Mock<INotificationsRepository>();

        userGroupMappingsRepo.Setup(r => r.RemoveUserGroupMappingAsync(groupId, userIds))
            .ReturnsAsync(true);

        var handler = new RemoveUserGroupMappingCommandHandler(userGroupMappingsRepo.Object, notificationsRepo.Object);
        var command = new RemoveUserGroupMappingCommand(userIds, userId, groupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        notificationsRepo.Verify(r => r.InsertNotificationAsync(It.IsAny<NotificationDto>()), Times.Never);
        userGroupMappingsRepo.Verify(r => r.GetUserAssociatedGroupAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RemoveFails_ReturnsFalseAndDoesNotCreateNotifications()
    {
        // Arrange
        var groupId = 1;
        var userId = 10;
        var userIds = new List<int> { 2, 3 };

        var userGroupMappingsRepo = new Mock<IUserGroupMappingsRepository>();
        var notificationsRepo = new Mock<INotificationsRepository>();

        userGroupMappingsRepo.Setup(r => r.RemoveUserGroupMappingAsync(groupId, userIds))
            .ReturnsAsync(false);

        var handler = new RemoveUserGroupMappingCommandHandler(userGroupMappingsRepo.Object, notificationsRepo.Object);
        var command = new RemoveUserGroupMappingCommand(userIds, userId, groupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        notificationsRepo.Verify(r => r.InsertNotificationAsync(It.IsAny<NotificationDto>()), Times.Never);
        userGroupMappingsRepo.Verify(r => r.GetUserAssociatedGroupAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_GroupNotFound_UsesDefaultGroupNameInNotification()
    {
        // Arrange
        var groupId = 1;
        var userId = 10;
        var userIds = new List<int> { 2 };
        IEnumerable<UserGroupDto>? userGroups = null;

        var userGroupMappingsRepo = new Mock<IUserGroupMappingsRepository>();
        var notificationsRepo = new Mock<INotificationsRepository>();

        userGroupMappingsRepo.Setup(r => r.RemoveUserGroupMappingAsync(groupId, userIds))
            .ReturnsAsync(true);
        userGroupMappingsRepo.Setup(r => r.GetUserAssociatedGroupAsync(groupId))
            .ReturnsAsync(userGroups);

        NotificationDto? capturedNotification = null;
        notificationsRepo.Setup(r => r.InsertNotificationAsync(It.IsAny<NotificationDto>()))
            .Callback<NotificationDto>(n => capturedNotification = n)
            .ReturnsAsync(true);

        var handler = new RemoveUserGroupMappingCommandHandler(userGroupMappingsRepo.Object, notificationsRepo.Object);
        var command = new RemoveUserGroupMappingCommand(userIds, userId, groupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.NotNull(capturedNotification);
        Assert.Contains("User Group", capturedNotification!.Message);
    }

    [Fact]
    public async Task Handle_NotificationsRepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var groupId = 1;
        var userId = 10;
        var userIds = new List<int> { 2 };
        var userGroups = new List<UserGroupDto> { new UserGroupDto { GroupId = groupId, GroupName = "Test Group" } };

        var userGroupMappingsRepo = new Mock<IUserGroupMappingsRepository>();
        var notificationsRepo = new Mock<INotificationsRepository>();

        userGroupMappingsRepo.Setup(r => r.RemoveUserGroupMappingAsync(groupId, userIds))
            .ReturnsAsync(true);
        userGroupMappingsRepo.Setup(r => r.GetUserAssociatedGroupAsync(groupId))
            .ReturnsAsync(userGroups.AsEnumerable());

        notificationsRepo.Setup(r => r.InsertNotificationAsync(It.IsAny<NotificationDto>()))
            .ThrowsAsync(new Exception("Notification error"));

        var handler = new RemoveUserGroupMappingCommandHandler(userGroupMappingsRepo.Object, notificationsRepo.Object);
        var command = new RemoveUserGroupMappingCommand(userIds, userId, groupId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_RemoveUserGroupMappingAsyncThrows_ExceptionPropagates()
    {
        // Arrange
        var groupId = 1;
        var userId = 10;
        var userIds = new List<int> { 2 };

        var userGroupMappingsRepo = new Mock<IUserGroupMappingsRepository>();
        var notificationsRepo = new Mock<INotificationsRepository>();

        userGroupMappingsRepo.Setup(r => r.RemoveUserGroupMappingAsync(groupId, userIds))
            .ThrowsAsync(new Exception("Remove error"));

        var handler = new RemoveUserGroupMappingCommandHandler(userGroupMappingsRepo.Object, notificationsRepo.Object);
        var command = new RemoveUserGroupMappingCommand(userIds, userId, groupId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }
}