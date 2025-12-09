using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Queries.GetPagedCompanyByUsers;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using System.Collections.Generic;

public class GetPagedCompanyByUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPagedCompanies()
    {
        var mockRepo = new Mock<ICompaniesRepository>();
        var expectedData = new List<CompanyBasicInfoDto> { new CompanyBasicInfoDto() };
        var expectedResult = new Tuple<List<CompanyBasicInfoDto>, int>(expectedData, 1);

        // Fix: Ensure the correct type is passed to ReturnsAsync
        mockRepo.Setup(r => r.GetFilteredCompanyByUsersAsync(1, 2, "group", "search", 1, 10))
            .ReturnsAsync(expectedResult);

        var handler = new GetPagedCompanyByUsersQueryHandler(mockRepo.Object);
        var query = new GetPagedCompanyByUsersQuery(1, 2, "group", "search", 1, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
    }
}