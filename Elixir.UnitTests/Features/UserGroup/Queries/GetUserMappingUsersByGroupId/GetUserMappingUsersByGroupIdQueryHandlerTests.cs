using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.UserGroup.Queries.GetUserMappingUsersByGroupId;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetUserMappingUsersByGroupIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ValidGroupId_ReturnsCompanyUserDtos()
    {
        // Arrange
        var groupId = 1;
        var expectedUsers = new List<CompanyUserDto>
        {
            new CompanyUserDto { /* set properties as needed */ },
            new CompanyUserDto { /* set properties as needed */ }
        };
        var repoMock = new Mock<IUserGroupMappingsRepository>();
        repoMock.Setup(r => r.GetUserMappingUsersByGroupIdAsync(groupId))
            .ReturnsAsync(expectedUsers);

        var handler = new GetUserMappingUsersByGroupIdQueryHandler(repoMock.Object);
        var query = new GetUserMappingUsersByGroupIdQuery(groupId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUsers, result);
    }

    [Fact]
    public async Task Handle_GroupIdWithNoUsers_ReturnsEmptyList()
    {
        // Arrange
        var groupId = 2;
        var repoMock = new Mock<IUserGroupMappingsRepository>();
        repoMock.Setup(r => r.GetUserMappingUsersByGroupIdAsync(groupId))
            .ReturnsAsync(new List<CompanyUserDto>());

        var handler = new GetUserMappingUsersByGroupIdQueryHandler(repoMock.Object);
        var query = new GetUserMappingUsersByGroupIdQuery(groupId);

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
        var groupId = 3;
        var repoMock = new Mock<IUserGroupMappingsRepository>();
        repoMock.Setup(r => r.GetUserMappingUsersByGroupIdAsync(groupId))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new GetUserMappingUsersByGroupIdQueryHandler(repoMock.Object);
        var query = new GetUserMappingUsersByGroupIdQuery(groupId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
    }
}