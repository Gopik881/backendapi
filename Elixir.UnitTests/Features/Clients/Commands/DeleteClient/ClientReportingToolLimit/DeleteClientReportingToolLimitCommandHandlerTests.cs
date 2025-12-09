using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.Commands.DeleteClient.ClientReportingToolLimit;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class DeleteClientReportingToolLimitCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Call_Repository_And_Return_True_When_Delete_Succeeds()
    {
        var repoMock = new Mock<IClientReportingToolLimitsRepository>();
        repoMock.Setup(r => r.DeleteClientReportingToolLimitsAsync(It.IsAny<int>())).ReturnsAsync(true);

        var handler = new DeleteClientReportingToolLimitCommandHandler(repoMock.Object);
        var result = await handler.Handle(new DeleteClientReportingToolLimitCommand(1), CancellationToken.None);

        Assert.True(result);
        repoMock.Verify(r => r.DeleteClientReportingToolLimitsAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_False_When_Repository_Returns_False()
    {
        var repoMock = new Mock<IClientReportingToolLimitsRepository>();
        repoMock.Setup(r => r.DeleteClientReportingToolLimitsAsync(It.IsAny<int>())).ReturnsAsync(false);

        var handler = new DeleteClientReportingToolLimitCommandHandler(repoMock.Object);
        var result = await handler.Handle(new DeleteClientReportingToolLimitCommand(2), CancellationToken.None);

        Assert.False(result);
        repoMock.Verify(r => r.DeleteClientReportingToolLimitsAsync(2), Times.Once);
    }
}