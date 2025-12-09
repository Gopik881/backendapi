using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Queries.GetPagedCompaniesSummaryForAdminUsers;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using System.Collections.Generic;

public class GetPagedCompaniesSummaryForAdminUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_SuperAdmin_ReturnsAllCompanies()
    {
        var mockRepo = new Mock<ICompaniesRepository>();
        var expectedData = new List<CompanySummaryDto> { new CompanySummaryDto() };
        mockRepo.Setup(r => r.GetPagedSuperAdminCompaniesSummaryAsync(1, true, true, "search", 1, 10))
            .ReturnsAsync(new Tuple<List<CompanySummaryDto>, int>(expectedData, 1));

        var handler = new GetPagedCompaniesSummaryForAdminUsersQueryHandler(mockRepo.Object);
        var query = new GetPagedCompaniesSummaryForAdminUsersQuery(1, true, true, "search", 1, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task Handle_DelegateAdmin_ReturnsDelegateCompanies()
    {
        var mockRepo = new Mock<ICompaniesRepository>();
        var expectedData = new List<CompanySummaryDto> { new CompanySummaryDto() };
        mockRepo.Setup(r => r.GetPagedDelegateAdminCompaniesSummaryAsync(1, true, false, "search", 1, 10))
            .ReturnsAsync(new Tuple<List<CompanySummaryDto>, int>(expectedData, 1));

        var handler = new GetPagedCompaniesSummaryForAdminUsersQueryHandler(mockRepo.Object);
        var query = new GetPagedCompaniesSummaryForAdminUsersQuery(1, true, false, "search", 1, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
    }
}