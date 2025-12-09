using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.UpdateReportingAdmins;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class UpdateReportingAdminsCommandHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsTrue_ReturnsTrue()
    {
        // Arrange
        var userGroupId = 1;
        var reportingAdminsDtos = new List<UserGroupReportingAdmin>
        {
            new UserGroupReportingAdmin { /* set properties as needed */ }
        };
        var repoMock = new Mock<IReportingAdminRepository>();
        repoMock.Setup(r => r.UpdateReportingAdminsAsync(userGroupId, reportingAdminsDtos))
            .ReturnsAsync(true);

        var handler = new UpdateReportingAdminsCommandHandler(repoMock.Object);
        var command = new UpdateReportingAdminsCommand(userGroupId, reportingAdminsDtos);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        repoMock.Verify(r => r.UpdateReportingAdminsAsync(userGroupId, reportingAdminsDtos), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsFalse_ReturnsFalse()
    {
        // Arrange
        var userGroupId = 2;
        var reportingAdminsDtos = new List<UserGroupReportingAdmin>
        {
            new UserGroupReportingAdmin { /* set properties as needed */ }
        };
        var repoMock = new Mock<IReportingAdminRepository>();
        repoMock.Setup(r => r.UpdateReportingAdminsAsync(userGroupId, reportingAdminsDtos))
            .ReturnsAsync(false);

        var handler = new UpdateReportingAdminsCommandHandler(repoMock.Object);
        var command = new UpdateReportingAdminsCommand(userGroupId, reportingAdminsDtos);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        repoMock.Verify(r => r.UpdateReportingAdminsAsync(userGroupId, reportingAdminsDtos), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var userGroupId = 3;
        var reportingAdminsDtos = new List<UserGroupReportingAdmin>
        {
            new UserGroupReportingAdmin { /* set properties as needed */ }
        };
        var repoMock = new Mock<IReportingAdminRepository>();
        repoMock.Setup(r => r.UpdateReportingAdminsAsync(userGroupId, reportingAdminsDtos))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new UpdateReportingAdminsCommandHandler(repoMock.Object);
        var command = new UpdateReportingAdminsCommand(userGroupId, reportingAdminsDtos);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        repoMock.Verify(r => r.UpdateReportingAdminsAsync(userGroupId, reportingAdminsDtos), Times.Once);
    }
}