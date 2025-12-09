using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.UpdateUserRights;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class UpdateUserRightsCommandHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsTrue_ReturnsTrue()
    {
        // Arrange
        var userGroupId = 1;
        var userRightsDtos = new List<UserGroupMenuRights>
        {
            new UserGroupMenuRights { /* set properties as needed */ }
        };
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.UpdateUserRightsAsync(userRightsDtos, userGroupId))
            .ReturnsAsync(true);

        var handler = new UpdateUserRightsCommandHandler(repoMock.Object);
        var command = new UpdateUserRightsCommand(userGroupId, userRightsDtos);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        repoMock.Verify(r => r.UpdateUserRightsAsync(userRightsDtos, userGroupId), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsFalse_ReturnsFalse()
    {
        // Arrange
        var userGroupId = 2;
        var userRightsDtos = new List<UserGroupMenuRights>
        {
            new UserGroupMenuRights { /* set properties as needed */ }
        };
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.UpdateUserRightsAsync(userRightsDtos, userGroupId))
            .ReturnsAsync(false);

        var handler = new UpdateUserRightsCommandHandler(repoMock.Object);
        var command = new UpdateUserRightsCommand(userGroupId, userRightsDtos);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        repoMock.Verify(r => r.UpdateUserRightsAsync(userRightsDtos, userGroupId), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var userGroupId = 3;
        var userRightsDtos = new List<UserGroupMenuRights>
        {
            new UserGroupMenuRights { /* set properties as needed */ }
        };
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.UpdateUserRightsAsync(userRightsDtos, userGroupId))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new UpdateUserRightsCommandHandler(repoMock.Object);
        var command = new UpdateUserRightsCommand(userGroupId, userRightsDtos);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        repoMock.Verify(r => r.UpdateUserRightsAsync(userRightsDtos, userGroupId), Times.Once);
    }
}