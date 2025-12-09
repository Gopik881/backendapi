using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetUserGroupDetailsByGroupId;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetUserGroupDetailsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ValidUserGroupId_ReturnsUserGroupDto()
    {
        // Arrange
        var userGroupId = 1;
        var expected = new UserGroupDto
        {
            // Set properties as needed for assertion
        };
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.GetUserGroupByIdAsync(userGroupId))
            .ReturnsAsync(expected);

        var handler = new GetUserGroupDetailsQueryHandler(repoMock.Object);
        var query = new GetUserGroupDetailsQuery(userGroupId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_UserGroupIdNotFound_ReturnsNull()
    {
        // Arrange
        var userGroupId = 2;
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.GetUserGroupByIdAsync(userGroupId))
            .ReturnsAsync((UserGroupDto)null);

        var handler = new GetUserGroupDetailsQueryHandler(repoMock.Object);
        var query = new GetUserGroupDetailsQuery(userGroupId);

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
        repoMock.Setup(r => r.GetUserGroupByIdAsync(userGroupId))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new GetUserGroupDetailsQueryHandler(repoMock.Object);
        var query = new GetUserGroupDetailsQuery(userGroupId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
    }
}