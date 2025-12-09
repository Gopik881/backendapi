using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class CreateClientAdminCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Call_Repository_And_Return_True()
    {
        var repoMock = new Mock<IClientAdminInfoRepository>();
        repoMock.Setup(r => r.CreateClientAdminInfoDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ClientAdminInfoDto>())).ReturnsAsync(true);
        var handler = new CreateClientAdminCommandHandler(repoMock.Object);

        var result = await handler.Handle(new CreateClientAdminCommand(1, 2, new ClientAdminInfoDto()), CancellationToken.None);

        Assert.True(result);
        repoMock.Verify(r => r.CreateClientAdminInfoDataAsync(1, 2, It.IsAny<ClientAdminInfoDto>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_False_When_Repository_Returns_False()
    {
        var repoMock = new Mock<IClientAdminInfoRepository>();
        repoMock.Setup(r => r.CreateClientAdminInfoDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ClientAdminInfoDto>())).ReturnsAsync(false);
        var handler = new CreateClientAdminCommandHandler(repoMock.Object);

        var result = await handler.Handle(new CreateClientAdminCommand(1, 2, new ClientAdminInfoDto()), CancellationToken.None);

        Assert.False(result);
    }
}