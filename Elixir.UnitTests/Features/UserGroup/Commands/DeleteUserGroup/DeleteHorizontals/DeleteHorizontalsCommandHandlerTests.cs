using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteHorizontals;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class DeleteHorizontalsCommandHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsTrue_ReturnsTrue()
    {
        // Arrange
        var userGroupId = 1;
        var repoMock = new Mock<IHorizontalsRepository>();
        repoMock.Setup(r => r.DeleteHorizontalsByUserGroupIdAsync(userGroupId))
            .ReturnsAsync(true);

        var handler = new DeleteHorizontalsCommandHandler(repoMock.Object);
        var command = new DeleteHorizontalsCommand(userGroupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        repoMock.Verify(r => r.DeleteHorizontalsByUserGroupIdAsync(userGroupId), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsFalse_ReturnsFalse()
    {
        // Arrange
        var userGroupId = 2;
        var repoMock = new Mock<IHorizontalsRepository>();
        repoMock.Setup(r => r.DeleteHorizontalsByUserGroupIdAsync(userGroupId))
            .ReturnsAsync(false);

        var handler = new DeleteHorizontalsCommandHandler(repoMock.Object);
        var command = new DeleteHorizontalsCommand(userGroupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        repoMock.Verify(r => r.DeleteHorizontalsByUserGroupIdAsync(userGroupId), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var userGroupId = 3;
        var repoMock = new Mock<IHorizontalsRepository>();
        repoMock.Setup(r => r.DeleteHorizontalsByUserGroupIdAsync(userGroupId))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new DeleteHorizontalsCommandHandler(repoMock.Object);
        var command = new DeleteHorizontalsCommand(userGroupId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        repoMock.Verify(r => r.DeleteHorizontalsByUserGroupIdAsync(userGroupId), Times.Once);
    }
}