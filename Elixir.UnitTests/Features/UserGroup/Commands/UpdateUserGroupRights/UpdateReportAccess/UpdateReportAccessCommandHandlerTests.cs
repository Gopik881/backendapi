using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.UpdateReportAccess;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class UpdateReportAccessCommandHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsTrue_ReturnsTrue()
    {
        // Arrange
        var userGroupId = 1;
        var reportAccessDto = new ReportingAccessDto { /* set properties as needed */ };
        var repoMock = new Mock<IReportAccessRepository>();
        repoMock.Setup(r => r.UpdateReportAccessAsync(userGroupId, reportAccessDto))
            .ReturnsAsync(true);

        var handler = new UpdateReportAccessCommandHandler(repoMock.Object);
        var command = new UpdateReportAccessCommand(userGroupId, reportAccessDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        repoMock.Verify(r => r.UpdateReportAccessAsync(userGroupId, reportAccessDto), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsFalse_ReturnsFalse()
    {
        // Arrange
        var userGroupId = 2;
        var reportAccessDto = new ReportingAccessDto { /* set properties as needed */ };
        var repoMock = new Mock<IReportAccessRepository>();
        repoMock.Setup(r => r.UpdateReportAccessAsync(userGroupId, reportAccessDto))
            .ReturnsAsync(false);

        var handler = new UpdateReportAccessCommandHandler(repoMock.Object);
        var command = new UpdateReportAccessCommand(userGroupId, reportAccessDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        repoMock.Verify(r => r.UpdateReportAccessAsync(userGroupId, reportAccessDto), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var userGroupId = 3;
        var reportAccessDto = new ReportingAccessDto { /* set properties as needed */ };
        var repoMock = new Mock<IReportAccessRepository>();
        repoMock.Setup(r => r.UpdateReportAccessAsync(userGroupId, reportAccessDto))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new UpdateReportAccessCommandHandler(repoMock.Object);
        var command = new UpdateReportAccessCommand(userGroupId, reportAccessDto);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        repoMock.Verify(r => r.UpdateReportAccessAsync(userGroupId, reportAccessDto), Times.Once);
    }
}