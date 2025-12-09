using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetHorizontalsByGroupId;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetUserGroupHorizontalsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ValidUserGroupId_ReturnsUserGroupHorizontals()
    {
        // Arrange
        var userGroupId = 1;
        var expected = new List<UserGroupHorizontals>
        {
            new UserGroupHorizontals { /* set properties as needed */ },
            new UserGroupHorizontals { /* set properties as needed */ }
        };
        var repoMock = new Mock<IHorizontalsRepository>();
        repoMock.Setup(r => r.GetHorizontalsForRoleAsync(userGroupId))
            .ReturnsAsync(expected);

        var handler = new GetUserGroupHorizontalsQueryHandler(repoMock.Object);
        var query = new GetUserGroupHorizontalsQuery(userGroupId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_UserGroupIdWithNoHorizontals_ReturnsEmptyList()
    {
        // Arrange
        var userGroupId = 2;
        var repoMock = new Mock<IHorizontalsRepository>();
        repoMock.Setup(r => r.GetHorizontalsForRoleAsync(userGroupId))
            .ReturnsAsync(new List<UserGroupHorizontals>());

        var handler = new GetUserGroupHorizontalsQueryHandler(repoMock.Object);
        var query = new GetUserGroupHorizontalsQuery(userGroupId);

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
        var repoMock = new Mock<IHorizontalsRepository>();
        repoMock.Setup(r => r.GetHorizontalsForRoleAsync(userGroupId))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new GetUserGroupHorizontalsQueryHandler(repoMock.Object);
        var query = new GetUserGroupHorizontalsQuery(userGroupId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
    }
}