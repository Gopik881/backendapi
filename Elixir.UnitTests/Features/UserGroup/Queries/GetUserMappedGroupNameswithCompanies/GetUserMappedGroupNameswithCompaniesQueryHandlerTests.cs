using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Features.UserGroup.Queries.GetUserMappedGroupNameswithCompanies;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetUserMappedGroupNameswithCompaniesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ValidUserId_ReturnsUserMappedGroupNamesWithCompaniesDto()
    {
        // Arrange
        var userId = 1;
        var expected = new UserMappedGroupNamesWithCompaniesDto
        {
            // Set properties as needed for assertion
        };
        var repoMock = new Mock<IUserGroupMappingsRepository>();
        repoMock.Setup(r => r.GetUserMappedGroupNamesWithCompaniesAsync(userId))
            .ReturnsAsync(expected);

        var handler = new GetUserMappedGroupNameswithCompaniesQueryHandler(repoMock.Object);
        var query = new GetUserMappedGroupNameswithCompaniesQuery(userId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_UserIdWithNoMappedGroups_ReturnsDefaultDto()
    {
        // Arrange
        var userId = 2;
        var expected = new UserMappedGroupNamesWithCompaniesDto(); // default/empty
        var repoMock = new Mock<IUserGroupMappingsRepository>();
        repoMock.Setup(r => r.GetUserMappedGroupNamesWithCompaniesAsync(userId))
            .ReturnsAsync(expected);

        var handler = new GetUserMappedGroupNameswithCompaniesQueryHandler(repoMock.Object);
        var query = new GetUserMappedGroupNameswithCompaniesQuery(userId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var userId = 3;
        var repoMock = new Mock<IUserGroupMappingsRepository>();
        repoMock.Setup(r => r.GetUserMappedGroupNamesWithCompaniesAsync(userId))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new GetUserMappedGroupNameswithCompaniesQueryHandler(repoMock.Object);
        var query = new GetUserMappedGroupNameswithCompaniesQuery(userId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
    }
}