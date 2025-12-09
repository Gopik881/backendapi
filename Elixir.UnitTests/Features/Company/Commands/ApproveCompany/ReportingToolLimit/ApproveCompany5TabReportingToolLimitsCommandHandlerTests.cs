using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.ApproveCompany.ReportingToolLimit;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;

public class ApproveCompany5TabReportingToolLimitsCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsTrue_WhenRepositoryReturnsTrue()
    {
        var repoMock = new Mock<IReportingToolLimitsRepository>();
        repoMock.Setup(r => r.Company5TabApproveReportingToolLimitsDataAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Company5TabReportingToolLimitsDto>(), CancellationToken.None)) // Explicitly pass CancellationToken.None
            .ReturnsAsync(true);

        var handler = new ApproveCompany5TabReportingToolLimitsCommandHandler(repoMock.Object);
        var command = new ApproveCompany5TabReportingToolLimitsCommand(1, 2, new Company5TabReportingToolLimitsDto());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ReturnsFalse_WhenRepositoryReturnsFalse()
    {
        var repoMock = new Mock<IReportingToolLimitsRepository>();
        repoMock.Setup(r => r.Company5TabApproveReportingToolLimitsDataAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Company5TabReportingToolLimitsDto>(), CancellationToken.None)) // Explicitly pass CancellationToken.None
            .ReturnsAsync(false);

        var handler = new ApproveCompany5TabReportingToolLimitsCommandHandler(repoMock.Object);
        var command = new ApproveCompany5TabReportingToolLimitsCommand(1, 2, new Company5TabReportingToolLimitsDto());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }
}