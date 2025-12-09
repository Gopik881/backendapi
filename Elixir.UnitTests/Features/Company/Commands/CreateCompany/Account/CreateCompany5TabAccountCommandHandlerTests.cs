using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;

public class CreateCompany5TabAccountCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsTrue_WhenRepositoryReturnsTrue()
    {
        var repoMock = new Mock<IAccountHistoryRepository>();
        repoMock.Setup(r => r.Company5TabCreateAccountDataAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Company5TabAccountDto>(), CancellationToken.None)) // Explicitly pass CancellationToken.None
            .ReturnsAsync(true);

        var handler = new CreateCompany5TabAccountCommandHandler(repoMock.Object);
        var command = new CreateCompany5TabAccountCommand(1, 2, new Company5TabAccountDto());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ReturnsFalse_WhenRepositoryReturnsFalse()
    {
        var repoMock = new Mock<IAccountHistoryRepository>();
        repoMock.Setup(r => r.Company5TabCreateAccountDataAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Company5TabAccountDto>(), CancellationToken.None)) // Explicitly pass CancellationToken.None
            .ReturnsAsync(false);

        var handler = new CreateCompany5TabAccountCommandHandler(repoMock.Object);
        var command = new CreateCompany5TabAccountCommand(1, 2, new Company5TabAccountDto());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }
}