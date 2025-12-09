using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteReportAccess;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class DeleteReportAccessCommandHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsTrue_ReturnsTrue()
    {
        // Arrange
        var userGroupId = 1;
        var repoMock = new Mock<IReportAccessRepository>();
        repoMock.Setup(r => r.DeleteReportAccessByUserGroupIdAsync(userGroupId))
            .ReturnsAsync(true);

        var handler = new DeleteReportAccessCommandHandler(repoMock.Object);
        var command = new DeleteReportAccessCommand(userGroupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        repoMock.Verify(r => r.DeleteReportAccessByUserGroupIdAsync(userGroupId), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsFalse_ReturnsFalse()
    {
        // Arrange
        var userGroupId = 2;
        var repoMock = new Mock<IReportAccessRepository>();
        repoMock.Setup(r => r.DeleteReportAccessByUserGroupIdAsync(userGroupId))
            .ReturnsAsync(false);

        var handler = new DeleteReportAccessCommandHandler(repoMock.Object);
        var command = new DeleteReportAccessCommand(userGroupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        repoMock.Verify(r => r.DeleteReportAccessByUserGroupIdAsync(userGroupId), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var userGroupId = 3;
        var repoMock = new Mock<IReportAccessRepository>();
        repoMock.Setup(r => r.DeleteReportAccessByUserGroupIdAsync(userGroupId))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new DeleteReportAccessCommandHandler(repoMock.Object);
        var command = new DeleteReportAccessCommand(userGroupId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        repoMock.Verify(r => r.DeleteReportAccessByUserGroupIdAsync(userGroupId), Times.Once);
    }
}