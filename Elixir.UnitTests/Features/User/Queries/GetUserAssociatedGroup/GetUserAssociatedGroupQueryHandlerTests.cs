using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Features.User.Queries.GetUserAssociatedGroup;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetUserAssociatedGroupQueryHandlerTests
{
    [Fact]
    public async Task Handle_ValidUserId_ReturnsUserGroupDtos()
    {
        // Arrange
        var userId = 1;
        var expectedGroups = new List<UserGroupDto>
        {
            new UserGroupDto { /* set properties as needed */ },
            new UserGroupDto { /* set properties as needed */ }
        };
        var repoMock = new Mock<IUserGroupMappingsRepository>();
        repoMock.Setup(r => r.GetUserAssociatedGroupAsync(userId))
            .ReturnsAsync(expectedGroups);

        var handler = new GetUserAssociatedGroupQueryHandler(repoMock.Object);
        var query = new GetUserAssociatedGroupQuery(userId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedGroups, result);
    }

    [Fact]
    public async Task Handle_UserIdWithNoAssociatedGroups_ReturnsEmptyList()
    {
        // Arrange
        var userId = 2;
        var repoMock = new Mock<IUserGroupMappingsRepository>();
        repoMock.Setup(r => r.GetUserAssociatedGroupAsync(userId))
            .ReturnsAsync(new List<UserGroupDto>());

        var handler = new GetUserAssociatedGroupQueryHandler(repoMock.Object);
        var query = new GetUserAssociatedGroupQuery(userId);

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
        var repoMock = new Mock<IUserGroupMappingsRepository>();
        repoMock.Setup(r => r.GetUserAssociatedGroupAsync(userId))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new GetUserAssociatedGroupQueryHandler(repoMock.Object);
        var query = new GetUserAssociatedGroupQuery(userId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
    }
}