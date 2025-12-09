using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Queries.GetPagedCompaniesSummaryForTMIUsers;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using System.Collections.Generic;

public class GetPagedCompaniesSummaryForTMIUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPagedTMIUsersCompanies()
    {
        var mockRepo = new Mock<ICompaniesRepository>();
        var expectedData = new List<CompanyTMISummaryDto> { new CompanyTMISummaryDto() };
        var expectedResult = new Tuple<List<CompanyTMISummaryDto>, int>(expectedData, 1);

        // Fix: Ensure the correct type is passed to ReturnsAsync
        mockRepo.Setup(r => r.GetPagedTMIUsersCompaniesSummaryAsync(1, true, "search", 1, 10))
            .ReturnsAsync(expectedResult);

        var handler = new GetPagedCompaniesSummaryForTMIUsersQueryHandler(mockRepo.Object);
        var query = new GetPagedCompaniesSummaryForTMIUsersQuery(1, true, "search", 1, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
    }
}