using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetReportingToolLimit;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Company.DTOs;

public class GetCompany5TabReportingToolLimitQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsDto_WhenRepositoryReturnsDto()
    {
        var repoMock = new Mock<IReportingToolLimitsHistoryRepository>();
        var expected = new Company5TabReportingToolLimitsDto();
        repoMock.Setup(r => r.GetCompany5TabLatestReportingToolLimitsHistoryAsync(
                It.Is<int>(companyId => companyId == 1),
                It.Is<bool>(isPrevious => isPrevious == false),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

        var handler = new GetCompany5TabReportingToolLimitQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetCompany5TabReportingToolLimitQuery(1, false), CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_CallsRepositoryWithCorrectParameters()
    {
        var repoMock = new Mock<IReportingToolLimitsHistoryRepository>();
        var handler = new GetCompany5TabReportingToolLimitQueryHandler(repoMock.Object);
        var query = new GetCompany5TabReportingToolLimitQuery(2, true);

        await handler.Handle(query, CancellationToken.None);

        repoMock.Verify(r => r.GetCompany5TabLatestReportingToolLimitsHistoryAsync(
                It.Is<int>(companyId => companyId == 2),
                It.Is<bool>(isPrevious => isPrevious == true),
                It.IsAny<CancellationToken>()), Times.Once);
    }
}