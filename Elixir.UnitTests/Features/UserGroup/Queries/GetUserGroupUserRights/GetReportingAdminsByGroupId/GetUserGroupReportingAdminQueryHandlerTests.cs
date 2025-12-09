using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetReportingAdminsByGroupId;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetUserGroupReportingAdminQueryHandlerTests
{
    [Fact]
    public async Task Handle_ValidUserGroupId_ReturnsReportingAdmins()
    {
        // Arrange
        var userGroupId = 1;
        var expected = new List<UserGroupReportingAdmin>
        {
            new UserGroupReportingAdmin { /* set properties as needed */ },
            new UserGroupReportingAdmin { /* set properties as needed */ }
        };
        var repoMock = new Mock<IReportingAdminRepository>();
        repoMock.Setup(r => r.GetReportingAdminsForRoleAsync(userGroupId))
            .ReturnsAsync(expected);

        var handler = new GetUserGroupReportingAdminQueryHandler(repoMock.Object);
        var query = new GetUserGroupReportingAdminQuery(userGroupId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_UserGroupIdWithNoReportingAdmins_ReturnsEmptyList()
    {
        // Arrange
        var userGroupId = 2;
        var repoMock = new Mock<IReportingAdminRepository>();
        repoMock.Setup(r => r.GetReportingAdminsForRoleAsync(userGroupId))
            .ReturnsAsync(new List<UserGroupReportingAdmin>());

        var handler = new GetUserGroupReportingAdminQueryHandler(repoMock.Object);
        var query = new GetUserGroupReportingAdminQuery(userGroupId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var userGroupId = 3;
        var repoMock = new Mock<IReportingAdminRepository>();
        repoMock.Setup(r => r.GetReportingAdminsForRoleAsync(userGroupId))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new GetUserGroupReportingAdminQueryHandler(repoMock.Object);
        var query = new GetUserGroupReportingAdminQuery(userGroupId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
    }
}