using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetReportAccess;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetReportAccessQueryHandlerTests
{
    [Fact]
    public async Task Handle_ValidUserGroupId_ReturnsReportingAccessDtos()
    {
        // Arrange
        var userGroupId = 1;
        var expected = new List<ReportingAccessDto>
        {
            new ReportingAccessDto { /* set properties as needed */ },
            new ReportingAccessDto { /* set properties as needed */ }
        };
        var repoMock = new Mock<IReportAccessRepository>();
        repoMock.Setup(r => r.GetReportAccessData(userGroupId))
            .ReturnsAsync(expected);

        var handler = new GetReportAccessQueryHandler(repoMock.Object);
        var query = new GetReportAccessQuery(userGroupId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_UserGroupIdWithNoReportAccess_ReturnsEmptyList()
    {
        // Arrange
        var userGroupId = 2;
        var repoMock = new Mock<IReportAccessRepository>();
        repoMock.Setup(r => r.GetReportAccessData(userGroupId))
            .ReturnsAsync(new List<ReportingAccessDto>());

        var handler = new GetReportAccessQueryHandler(repoMock.Object);
        var query = new GetReportAccessQuery(userGroupId);

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
        var repoMock = new Mock<IReportAccessRepository>();
        repoMock.Setup(r => r.GetReportAccessData(userGroupId))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new GetReportAccessQueryHandler(repoMock.Object);
        var query = new GetReportAccessQuery(userGroupId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
    }
}