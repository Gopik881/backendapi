using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Company.DTOs;

public class CreateCompany5TabReportingToolLimitsCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsTrue_WhenRepositoryReturnsTrue()
    {
        var repoMock = new Mock<IReportingToolLimitsHistoryRepository>();
        repoMock.Setup(r => r.Company5TabCreateReportingToolLimitsDataAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Company5TabReportingToolLimitsDto>(),
                It.IsAny<CancellationToken>())) // Explicitly include the optional argument
            .ReturnsAsync(true);

        var handler = new CreateCompany5TabReportingToolLimitsCommandHandler(repoMock.Object);
        var command = new CreateCompany5TabReportingToolLimitsCommand(1, 2, new Company5TabReportingToolLimitsDto());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ReturnsFalse_WhenRepositoryReturnsFalse()
    {
        var repoMock = new Mock<IReportingToolLimitsHistoryRepository>();
        repoMock.Setup(r => r.Company5TabCreateReportingToolLimitsDataAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Company5TabReportingToolLimitsDto>(),
                It.IsAny<CancellationToken>())) // Explicitly include the optional argument
            .ReturnsAsync(false);

        var handler = new CreateCompany5TabReportingToolLimitsCommandHandler(repoMock.Object);
        var command = new CreateCompany5TabReportingToolLimitsCommand(1, 2, new Company5TabReportingToolLimitsDto());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }
}