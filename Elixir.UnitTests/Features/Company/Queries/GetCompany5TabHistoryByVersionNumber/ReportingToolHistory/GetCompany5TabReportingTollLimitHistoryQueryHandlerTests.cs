using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.ReportingToolHistory;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Company.DTOs;

public class GetCompany5TabReportingTollLimitHistoryQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsDto_WhenRepositoryReturnsDto()
    {
        var repoMock = new Mock<IReportingToolLimitsHistoryRepository>();
        var expected = new Company5TabHistoryDto();
        repoMock.Setup(r => r.GetCompany5TabReportingToolLimitsHistoryByVersionAsync(1, 2, 3))
                .ReturnsAsync(expected);

        var handler = new GetCompany5TabReportingTollLimitHistoryQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetCompany5TabReportingToolLimitHistoryQuery(1, 2, 3), CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_CallsRepositoryWithCorrectParameters()
    {
        var repoMock = new Mock<IReportingToolLimitsHistoryRepository>();
        var handler = new GetCompany5TabReportingTollLimitHistoryQueryHandler(repoMock.Object);
        var query = new GetCompany5TabReportingToolLimitHistoryQuery(4, 5, 6);

        await handler.Handle(query, CancellationToken.None);

        repoMock.Verify(r => r.GetCompany5TabReportingToolLimitsHistoryByVersionAsync(4, 5, 6), Times.Once);
    }
}