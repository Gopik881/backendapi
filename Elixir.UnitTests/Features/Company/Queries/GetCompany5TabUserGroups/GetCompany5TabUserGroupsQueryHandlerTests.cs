using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabUserGroups;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetCompany5TabUserGroupsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsUserGroups_WhenRepositoryReturnsData()
    {
        var mockRepo = new Mock<IUserGroupsRepository>();
        var expected = new List<Company5TabUserGroupDto> { new Company5TabUserGroupDto() };
        mockRepo.Setup(r => r.GetCompany5TabUserGroupsByCompanyIdAsync(1)).ReturnsAsync(expected);

        var handler = new GetCompany5TabUserGroupsQueryHandler(mockRepo.Object);
        var result = await handler.Handle(new GetCompany5TabUserGroupsQuery(1), CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenRepositoryReturnsEmpty()
    {
        var mockRepo = new Mock<IUserGroupsRepository>();
        mockRepo.Setup(r => r.GetCompany5TabUserGroupsByCompanyIdAsync(2)).ReturnsAsync(new List<Company5TabUserGroupDto>());

        var handler = new GetCompany5TabUserGroupsQueryHandler(mockRepo.Object);
        var result = await handler.Handle(new GetCompany5TabUserGroupsQuery(2), CancellationToken.None);

        Assert.Empty(result);
    }
}