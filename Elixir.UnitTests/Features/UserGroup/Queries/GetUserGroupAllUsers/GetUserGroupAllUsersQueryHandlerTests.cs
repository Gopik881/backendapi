using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupAllUsers;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetUserGroupAllUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsUsers_ReturnsCompanyUserDtos()
    {
        // Arrange
        var expectedUsers = new List<CompanyUserDto>
        {
            new CompanyUserDto { /* set properties as needed */ },
            new CompanyUserDto { /* set properties as needed */ }
        };
        var repoMock = new Mock<IUserGroupMappingsRepository>();
        repoMock.Setup(r => r.GetAllUserGroupUsersAsync())
            .ReturnsAsync(expectedUsers);

        var handler = new GetUserGroupAllUsersQueryHandler(repoMock.Object);
        var query = new GetUserGroupAllUsersQuery();

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
        repoMock.Setup(r => r.GetAllUserGroupUsersAsync())
            .ReturnsAsync(new List<CompanyUserDto>());

        var handler = new GetUserGroupAllUsersQueryHandler(repoMock.Object);
        var query = new GetUserGroupAllUsersQuery();

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
        repoMock.Setup(r => r.GetAllUserGroupUsersAsync())
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new GetUserGroupAllUsersQueryHandler(repoMock.Object);
        var query = new GetUserGroupAllUsersQuery();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
    }
}