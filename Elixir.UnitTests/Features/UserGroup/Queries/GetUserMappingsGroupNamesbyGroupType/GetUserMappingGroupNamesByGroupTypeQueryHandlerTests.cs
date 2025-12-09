using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Features.UserGroup.Queries.GetUserMappingsGroupNames;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetUserMappingGroupNamesByGroupTypeQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsListOfUserMappingGroupsByGroupTypeDto()
    {
        // Arrange
        var expected = new List<UserMappingGroupsByGroupTypeDto>
        {
            new UserMappingGroupsByGroupTypeDto { /* set properties as needed */ },
            new UserMappingGroupsByGroupTypeDto { /* set properties as needed */ }
        };
        var repoMock = new Mock<IUserGroupMappingsRepository>();
        repoMock.Setup(r => r.GetUserMappingGroupNamesByGroupTypeAsync())
            .ReturnsAsync(expected);

        var handler = new GetUserMappingGroupNamesByGroupTypeQueryHandler(repoMock.Object);
        var query = new GetUserMappingGroupNamesByGroupTypeQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var repoMock = new Mock<IUserGroupMappingsRepository>();
        repoMock.Setup(r => r.GetUserMappingGroupNamesByGroupTypeAsync())
            .ReturnsAsync(new List<UserMappingGroupsByGroupTypeDto>());

        var handler = new GetUserMappingGroupNamesByGroupTypeQueryHandler(repoMock.Object);
        var query = new GetUserMappingGroupNamesByGroupTypeQuery();

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
        repoMock.Setup(r => r.GetUserMappingGroupNamesByGroupTypeAsync())
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new GetUserMappingGroupNamesByGroupTypeQueryHandler(repoMock.Object);
        var query = new GetUserMappingGroupNamesByGroupTypeQuery();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
    }
}