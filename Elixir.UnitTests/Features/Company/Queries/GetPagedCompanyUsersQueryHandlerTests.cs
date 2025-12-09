using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Queries.GetPagedCompanyUsers;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using System.Collections.Generic;

public class GetPagedCompanyUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPagedCompanyUsers()
    {
        var mockRepo = new Mock<ICompaniesRepository>();
        var expectedData = new List<CompanyUserDto> { new CompanyUserDto() };
        var expectedResult = new Tuple<List<CompanyUserDto>, int>(expectedData, 1);

        // Fix: Ensure the correct type is passed to ReturnsAsync
        mockRepo.Setup(r => r.GetFilteredCompanyUsersAsync(1, "search", 1, 10))
            .ReturnsAsync(expectedResult);

        var handler = new GetPagedCompanyUsersQueryHandler(mockRepo.Object);
        var query = new GetPagedCompanyUsersQuery(1, "search", 1, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
    }
}