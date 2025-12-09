using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetAllCategoriesQuery;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetAllCategoriesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ValidUserGroupId_ReturnsSelectionItemDtos()
    {
        // Arrange
        var userGroupId = 1;
        var expected = new List<SelectionItemDto>
        {
            new SelectionItemDto { /* set properties as needed */ },
            new SelectionItemDto { /* set properties as needed */ }
        };
        var repoMock = new Mock<IReportAccessRepository>();
        repoMock.Setup(r => r.GetCategoriesForReportsAsync(userGroupId))
            .ReturnsAsync(expected);

        var handler = new GetAllCategoriesQueryHandler(repoMock.Object);
        var query = new GetAllCategoriesQuery(userGroupId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_UserGroupIdWithNoCategories_ReturnsEmptyList()
    {
        // Arrange
        var userGroupId = 2;
        var repoMock = new Mock<IReportAccessRepository>();
        repoMock.Setup(r => r.GetCategoriesForReportsAsync(userGroupId))
            .ReturnsAsync(new List<SelectionItemDto>());

        var handler = new GetAllCategoriesQueryHandler(repoMock.Object);
        var query = new GetAllCategoriesQuery(userGroupId);

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
        var userGroupId = 3;
        var repoMock = new Mock<IReportAccessRepository>();
        repoMock.Setup(r => r.GetCategoriesForReportsAsync(userGroupId))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new GetAllCategoriesQueryHandler(repoMock.Object);
        var query = new GetAllCategoriesQuery(userGroupId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
    }
}