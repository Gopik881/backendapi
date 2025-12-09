using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabCustomUsers;
using Elixir.Application.Interfaces.Persistance;
using Moq;

namespace Elixir.UnitTests.Features.Company.Queries;

public class GetCompany5TabCustomUserGroupsQueryHandlerTests
{
    private readonly Mock<ICompaniesRepository> _repoMock;
    private readonly GetCompany5TabCustomUserGroupsQueryHandler _handler;

    public GetCompany5TabCustomUserGroupsQueryHandlerTests()
    {
        _repoMock = new Mock<ICompaniesRepository>();
        _handler = new GetCompany5TabCustomUserGroupsQueryHandler(_repoMock.Object);
    }

    //[Fact]
    //public async Task Handle_ReturnsUserGroups_WhenDataExists()
    //{
    //    var query = new GetCompany5TabCustomUserGroupsQuery(1);
    //    //var userGroups = new List<UserGroupDto> { new UserGroupDto() };
    //    //_repoMock.Setup(r => r.GetCompany5TabCustomUserGroups(query.CompanyId)).ReturnsAsync(userGroups);

    //    var result = await _handler.Handle(query, CancellationToken.None);

    //    Assert.NotNull(result);
    //    Assert.Single(result);
    //}

    //[Fact]
    //public async Task Handle_ReturnsEmptyList_WhenNoData()
    //{
    //    var query = new GetCompany5TabCustomUserGroupsQuery(1);
    //    //_repoMock.Setup(r => r.GetCompany5TabCustomUserGroups(query.CompanyId)).ReturnsAsync(new List<UserGroupDto>());

    //    var result = await _handler.Handle(query, CancellationToken.None);

    //    Assert.NotNull(result);
    //    Assert.Empty(result);
    //}

    [Fact]
    public async Task Handle_ThrowsException_WhenRepositoryThrows()
    {
        var query = new GetCompany5TabCustomUserGroupsQuery(1);
        // Fix for CS0854: Remove optional argument from method call in Setup
        _repoMock.Setup(r => r.GetCompany5TabCustomUserGroups(It.IsAny<int>(), It.IsAny<string>())).ThrowsAsync(new Exception("DB error"));

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }
}