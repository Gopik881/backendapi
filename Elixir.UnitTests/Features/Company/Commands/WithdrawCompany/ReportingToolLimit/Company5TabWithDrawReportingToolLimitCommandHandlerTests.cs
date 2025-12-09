using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.ReportingToolLimit;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class Company5TabWithDrawReportingToolLimitCommandHandlerTests
{
    [Fact]
    public async Task Handle_CallsRepositoryAndReturnsResult()
    {
        var repo = new Mock<IReportingToolLimitsHistoryRepository>();
        // Fix: Explicitly pass the CancellationToken argument instead of relying on optional arguments
        var cancellationToken = CancellationToken.None;
        repo.Setup(r => r.WithdrawCompany5TabReportingToolLimitsHistoryAsync(1, 2, cancellationToken)).ReturnsAsync(true);

        var handler = new Company5TabWithDrawReportingToolLimitCommandHandler(repo.Object);
        var result = await handler.Handle(new Company5TabWithDrawReportingToolLimitCommand(1, 2), cancellationToken);

        Assert.True(result);
        repo.Verify(r => r.WithdrawCompany5TabReportingToolLimitsHistoryAsync(1, 2, cancellationToken), Times.Once);
    }
}