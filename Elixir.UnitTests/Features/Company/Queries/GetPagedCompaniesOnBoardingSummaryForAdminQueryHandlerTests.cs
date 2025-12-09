using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Queries.GetPagedCompaniesOnBoardingSummaryForAdminUsers;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using System.Collections.Generic;

public class GetPagedCompaniesOnBoardingSummaryForAdminUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_SuperAdmin_ReturnsAllCompanies()
    {
        var mockRepo = new Mock<ICompanyOnboardingStatusRepository>();
        var expectedData = new List<CompanyOnBoardingSummaryDto> { new CompanyOnBoardingSummaryDto() };
        mockRepo.Setup(r => r.GetPagedSuperAdminCompaniesOnBoardingSummaryAsync(1, true, "search", 1, 10))
            .ReturnsAsync(new Tuple<List<CompanyOnBoardingSummaryDto>, int>(expectedData, 1));

        var handler = new GetPagedCompaniesOnBoardingSummaryForAdminUsersQueryHandler(mockRepo.Object);
        var query = new GetPagedCompaniesOnBoardingSummaryForAdminUsersQuery(1, true, "search", 1, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task Handle_DelegateAdmin_ReturnsDelegateCompanies()
    {
        var mockRepo = new Mock<ICompanyOnboardingStatusRepository>();
        var expectedData = new List<CompanyOnBoardingSummaryDto> { new CompanyOnBoardingSummaryDto() };
        mockRepo.Setup(r => r.GetPagedDelegateAdminCompaniesOnBoardingSummaryAsync(1, "search", 1, 10))
            .ReturnsAsync(new Tuple<List<CompanyOnBoardingSummaryDto>, int>(expectedData, 1));

        var handler = new GetPagedCompaniesOnBoardingSummaryForAdminUsersQueryHandler(mockRepo.Object);
        var query = new GetPagedCompaniesOnBoardingSummaryForAdminUsersQuery(1, false, "search", 1, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
    }
}