using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.User.Commands.DeleteUser;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class DeleteUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsTrue_ReturnsTrue()
    {
        // Arrange
        var userId = 1;
        var repoMock = new Mock<IUsersRepository>();
        repoMock.Setup(r => r.DeleteUserAsync(userId)).ReturnsAsync(true);

        var handler = new DeleteUserCommandHandler(repoMock.Object);
        var command = new DeleteUserCommand(userId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        repoMock.Verify(r => r.DeleteUserAsync(userId), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsFalse_ReturnsFalse()
    {
        // Arrange
        var userId = 2;
        var repoMock = new Mock<IUsersRepository>();
        repoMock.Setup(r => r.DeleteUserAsync(userId)).ReturnsAsync(false);

        var handler = new DeleteUserCommandHandler(repoMock.Object);
        var command = new DeleteUserCommand(userId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        repoMock.Verify(r => r.DeleteUserAsync(userId), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var userId = 3;
        var repoMock = new Mock<IUsersRepository>();
        repoMock.Setup(r => r.DeleteUserAsync(userId)).ThrowsAsync(new Exception("Repository error"));

        var handler = new DeleteUserCommandHandler(repoMock.Object);
        var command = new DeleteUserCommand(userId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        repoMock.Verify(r => r.DeleteUserAsync(userId), Times.Once);
    }
}