using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.User.Queries.GetUserCriticalGroups;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetUserCriticalGroupsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ValidUserId_ReturnsCriticalGroups()
    {
        // Arrange
        var userId = 1;
        var expectedGroups = new List<string> { "GroupA", "GroupB" };
        var repoMock = new Mock<IUsersRepository>();
        repoMock.Setup(r => r.GetUsersCriticalGroupAsync(userId))
            .ReturnsAsync(expectedGroups);

        var handler = new GetUserCriticalGroupsQueryHandler(repoMock.Object);
        var query = new GetUserCriticalGroupsQuery(userId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedGroups, result);
    }

    [Fact]
    public async Task Handle_UserIdWithNoCriticalGroups_ReturnsEmptyList()
    {
        // Arrange
        var userId = 2;
        var repoMock = new Mock<IUsersRepository>();
        repoMock.Setup(r => r.GetUsersCriticalGroupAsync(userId))
            .ReturnsAsync(new List<string>());

        var handler = new GetUserCriticalGroupsQueryHandler(repoMock.Object);
        var query = new GetUserCriticalGroupsQuery(userId);

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
        var userId = 3;
        var repoMock = new Mock<IUsersRepository>();
        repoMock.Setup(r => r.GetUsersCriticalGroupAsync(userId))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new GetUserCriticalGroupsQueryHandler(repoMock.Object);
        var query = new GetUserCriticalGroupsQuery(userId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
    }
}