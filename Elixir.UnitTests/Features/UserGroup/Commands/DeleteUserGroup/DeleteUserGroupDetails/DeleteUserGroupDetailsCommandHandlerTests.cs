using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteUserGroupDetails;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class DeleteUserGroupDetailsCommandHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsTrue_ReturnsTrue()
    {
        // Arrange
        var userGroupId = 1;
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.DeleteUserGroupDetailsByUserGroupIdAsync(userGroupId))
            .ReturnsAsync(true);

        var handler = new DeleteUserGroupDetailsCommandHandler(repoMock.Object);
        var command = new DeleteUserGroupDetailsCommand(userGroupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        repoMock.Verify(r => r.DeleteUserGroupDetailsByUserGroupIdAsync(userGroupId), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsFalse_ReturnsFalse()
    {
        // Arrange
        var userGroupId = 2;
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.DeleteUserGroupDetailsByUserGroupIdAsync(userGroupId))
            .ReturnsAsync(false);

        var handler = new DeleteUserGroupDetailsCommandHandler(repoMock.Object);
        var command = new DeleteUserGroupDetailsCommand(userGroupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        repoMock.Verify(r => r.DeleteUserGroupDetailsByUserGroupIdAsync(userGroupId), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var userGroupId = 3;
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.DeleteUserGroupDetailsByUserGroupIdAsync(userGroupId))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new DeleteUserGroupDetailsCommandHandler(repoMock.Object);
        var command = new DeleteUserGroupDetailsCommand(userGroupId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        repoMock.Verify(r => r.DeleteUserGroupDetailsByUserGroupIdAsync(userGroupId), Times.Once);
    }
}