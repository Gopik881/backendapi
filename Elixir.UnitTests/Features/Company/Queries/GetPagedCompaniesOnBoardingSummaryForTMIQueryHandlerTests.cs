using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Queries.GetPagedCompaniesOnBoardingSummaryForTMIUsers;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using System.Collections.Generic;

public class GetPagedCompaniesOnBoardingSummaryForTMIUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPagedTMIUsersCompanies()
    {
        var mockRepo = new Mock<ICompanyOnboardingStatusRepository>();
        var expectedData = new List<CompanyTMIOnBoardingSummaryDto> { new CompanyTMIOnBoardingSummaryDto() };
        var expectedResult = new Tuple<List<CompanyTMIOnBoardingSummaryDto>, int>(expectedData, 1);

        // Specify all arguments explicitly, including the optional 'isDashboard' argument.
        mockRepo.Setup(r => r.GetPagedTMIUsersCompaniesOnBoardingSummaryAsync(1, "search", 1, 10, false))
            .ReturnsAsync(expectedResult);

        var handler = new GetPagedCompaniesOnBoardingSummaryForTMIUsersQueryHandler(mockRepo.Object);
        var query = new GetPagedCompaniesOnBoardingSummaryForTMIUsersQuery(1, "search", 1, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
    }
}