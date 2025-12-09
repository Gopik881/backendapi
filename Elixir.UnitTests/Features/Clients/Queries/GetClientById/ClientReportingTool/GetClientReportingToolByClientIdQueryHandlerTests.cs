using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;

public class GetClientReportingToolByClientIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsReportingToolLimits()
    {
        var repoMock = new Moq.Mock<IClientReportingToolLimitsRepository>();
        var expected = new ReportingToolLimitsDto();
        repoMock.Setup(x => x.GetClientReportingToolLimitDataAsync(1)).ReturnsAsync(expected);

        var handler = new GetClientReportingToolByClientIdQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetClientReportingToolByClientIdQuery(1), CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenNoLimits()
    {
        var repoMock = new Moq.Mock<IClientReportingToolLimitsRepository>();
        repoMock.Setup(x => x.GetClientReportingToolLimitDataAsync(1)).ReturnsAsync((ReportingToolLimitsDto?)null);

        var handler = new GetClientReportingToolByClientIdQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetClientReportingToolByClientIdQuery(1), CancellationToken.None);

        Assert.Null(result);
    }
}