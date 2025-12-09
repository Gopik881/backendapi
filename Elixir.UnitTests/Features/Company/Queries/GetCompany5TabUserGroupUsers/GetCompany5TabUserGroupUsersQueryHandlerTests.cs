using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabUserGroupUsers;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetCompany5TabUserGroupUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsUsers_WhenRepositoryReturnsData()
    {
        var mockRepo = new Mock<IUserGroupMappingsRepository>();
        var expected = new List<CompanyUserDto> { new CompanyUserDto() };
        mockRepo.Setup(r => r.GetCompany5TabUserGroupUsersAsync(1, 1)).ReturnsAsync(expected);

        var handler = new GetCompany5TabUserGroupUsersQueryHandler(mockRepo.Object);
        var result = await handler.Handle(new GetCompany5TabUserGroupUsersQuery(1, 1), CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenRepositoryReturnsEmpty()
    {
        var mockRepo = new Mock<IUserGroupMappingsRepository>();
        mockRepo.Setup(r => r.GetCompany5TabUserGroupUsersAsync(2, 2)).ReturnsAsync(new List<CompanyUserDto>());

        var handler = new GetCompany5TabUserGroupUsersQueryHandler(mockRepo.Object);
        var result = await handler.Handle(new GetCompany5TabUserGroupUsersQuery(2, 2), CancellationToken.None);

        Assert.Empty(result);
    }
}