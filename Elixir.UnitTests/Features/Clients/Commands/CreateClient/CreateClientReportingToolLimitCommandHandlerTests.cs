using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class CreateClientReportingToolLimitCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Call_Repository_And_Return_True()
    {
        var repoMock = new Mock<IClientReportingToolLimitsRepository>();
        repoMock.Setup(r => r.CreateClientReportingToolLimitDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ReportingToolLimitsDto>())).ReturnsAsync(true);
        var handler = new CreateClientReportingToolLimitCommandHandler(repoMock.Object);

        var result = await handler.Handle(new CreateClientReportingToolLimitCommand(1, 2, new ReportingToolLimitsDto()), CancellationToken.None);

        Assert.True(result);
        repoMock.Verify(r => r.CreateClientReportingToolLimitDataAsync(1, 2, It.IsAny<ReportingToolLimitsDto>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_False_When_Repository_Returns_False()
    {
        var repoMock = new Mock<IClientReportingToolLimitsRepository>();
        repoMock.Setup(r => r.CreateClientReportingToolLimitDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ReportingToolLimitsDto>())).ReturnsAsync(false);
        var handler = new CreateClientReportingToolLimitCommandHandler(repoMock.Object);

        var result = await handler.Handle(new CreateClientReportingToolLimitCommand(1, 2, new ReportingToolLimitsDto()), CancellationToken.None);

        Assert.False(result);
    }
}