using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetUserRightsByGroupId;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetUserGroupRightsByUserGroupIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ValidUserGroupId_ReturnsExpectedObject()
    {
        // Arrange
        var userGroupId = 1;
        var expected = new object(); // Replace with a more specific type if known
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.GetUserRightsByUserGroupId(userGroupId))
            .ReturnsAsync(expected);

        var handler = new GetUserGroupRightsByUserGroupIdQueryHandler(repoMock.Object);
        var query = new GetUserGroupRightsByUserGroupIdQuery(userGroupId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_UserGroupIdWithNoRights_ReturnsNull()
    {
        // Arrange
        var userGroupId = 2;
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.GetUserRightsByUserGroupId(userGroupId))
            .ReturnsAsync((object)null);

        var handler = new GetUserGroupRightsByUserGroupIdQueryHandler(repoMock.Object);
        var query = new GetUserGroupRightsByUserGroupIdQuery(userGroupId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var userGroupId = 3;
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.GetUserRightsByUserGroupId(userGroupId))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new GetUserGroupRightsByUserGroupIdQueryHandler(repoMock.Object);
        var query = new GetUserGroupRightsByUserGroupIdQuery(userGroupId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
    }
}