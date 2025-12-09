using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetRoleNameByGroupId;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetUserGroupRoleNameQueryHandlerTests
{
    [Fact]
    public async Task Handle_ValidUserGroupId_ReturnsRoleName()
    {
        // Arrange
        var userGroupId = 1;
        var expectedRoleName = "Admin";
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.GetRoleNameByGroupId(userGroupId))
            .ReturnsAsync(expectedRoleName);

        var handler = new GetUserGroupRoleNameQueryHandler(repoMock.Object);
        var query = new GetUserGroupRoleNameQuery(userGroupId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedRoleName, result);
    }

    [Fact]
    public async Task Handle_UserGroupIdNotFound_ReturnsNull()
    {
        // Arrange
        var userGroupId = 2;
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.GetRoleNameByGroupId(userGroupId))
            .ReturnsAsync((string)null);

        var handler = new GetUserGroupRoleNameQueryHandler(repoMock.Object);
        var query = new GetUserGroupRoleNameQuery(userGroupId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_UserGroupIdNotFound_ReturnsEmptyString()
    {
        // Arrange
        var userGroupId = 3;
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.GetRoleNameByGroupId(userGroupId))
            .ReturnsAsync(string.Empty);

        var handler = new GetUserGroupRoleNameQueryHandler(repoMock.Object);
        var query = new GetUserGroupRoleNameQuery(userGroupId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var userGroupId = 4;
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.GetRoleNameByGroupId(userGroupId))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new GetUserGroupRoleNameQueryHandler(repoMock.Object);
        var query = new GetUserGroupRoleNameQuery(userGroupId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
    }
}