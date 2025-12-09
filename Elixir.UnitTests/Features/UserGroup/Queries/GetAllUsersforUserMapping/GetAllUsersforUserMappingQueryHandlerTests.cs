using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Features.UserGroup.Queries.GetAllUsersforUserMapping;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetAllUsersforUserMappingQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsUsers_ReturnsUserListforUserMappingDtos()
    {
        // Arrange
        var expectedUsers = new List<UserListforUserMappingDto>
        {
            new UserListforUserMappingDto { /* set properties as needed */ },
            new UserListforUserMappingDto { /* set properties as needed */ }
        };
        var repoMock = new Mock<IUserGroupMappingsRepository>();
        repoMock.Setup(r => r.GetAllUsersforUserMappingAsync())
            .ReturnsAsync(expectedUsers);

        var handler = new GetAllUsersforUserMappingQueryHandler(repoMock.Object);
        var query = new GetAllUsersforUserMappingQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUsers, result);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var repoMock = new Mock<IUserGroupMappingsRepository>();
        repoMock.Setup(r => r.GetAllUsersforUserMappingAsync())
            .ReturnsAsync(new List<UserListforUserMappingDto>());

        var handler = new GetAllUsersforUserMappingQueryHandler(repoMock.Object);
        var query = new GetAllUsersforUserMappingQuery();

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
        var repoMock = new Mock<IUserGroupMappingsRepository>();
        repoMock.Setup(r => r.GetAllUsersforUserMappingAsync())
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new GetAllUsersforUserMappingQueryHandler(repoMock.Object);
        var query = new GetAllUsersforUserMappingQuery();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
    }
}