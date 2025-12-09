using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteUserRights;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class DeleteUserRightsCommandHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsTrue_ReturnsTrue()
    {
        // Arrange
        var userGroupId = 1;
        var repoMock = new Mock<IUserGroupMenuMappingRepository>();
        repoMock.Setup(r => r.DeleteUserRightsByUserGroupIdAsync(userGroupId))
            .ReturnsAsync(true);

        var handler = new DeleteUserRightsCommandHandler(repoMock.Object);
        var command = new DeleteUserRightsCommand(userGroupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        repoMock.Verify(r => r.DeleteUserRightsByUserGroupIdAsync(userGroupId), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsFalse_ReturnsFalse()
    {
        // Arrange
        var userGroupId = 2;
        var repoMock = new Mock<IUserGroupMenuMappingRepository>();
        repoMock.Setup(r => r.DeleteUserRightsByUserGroupIdAsync(userGroupId))
            .ReturnsAsync(false);

        var handler = new DeleteUserRightsCommandHandler(repoMock.Object);
        var command = new DeleteUserRightsCommand(userGroupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        repoMock.Verify(r => r.DeleteUserRightsByUserGroupIdAsync(userGroupId), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var userGroupId = 3;
        var repoMock = new Mock<IUserGroupMenuMappingRepository>();
        repoMock.Setup(r => r.DeleteUserRightsByUserGroupIdAsync(userGroupId))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new DeleteUserRightsCommandHandler(repoMock.Object);
        var command = new DeleteUserRightsCommand(userGroupId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        repoMock.Verify(r => r.DeleteUserRightsByUserGroupIdAsync(userGroupId), Times.Once);
    }
}