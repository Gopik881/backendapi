using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class CreateClientAccessCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Call_Repository_And_Return_True()
    {
        var repoMock = new Mock<IClientAccessRepository>();
        repoMock.Setup(r => r.CreateClientAccessDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ClientAccessDto>())).ReturnsAsync(true);
        var handler = new CreateClientAccessCommandHandler(repoMock.Object);

        var result = await handler.Handle(new CreateClientAccessCommand(1, 2, new ClientAccessDto()), CancellationToken.None);

        Assert.True(result);
        repoMock.Verify(r => r.CreateClientAccessDataAsync(1, 2, It.IsAny<ClientAccessDto>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_False_When_Repository_Returns_False()
    {
        var repoMock = new Mock<IClientAccessRepository>();
        repoMock.Setup(r => r.CreateClientAccessDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ClientAccessDto>())).ReturnsAsync(false);
        var handler = new CreateClientAccessCommandHandler(repoMock.Object);

        var result = await handler.Handle(new CreateClientAccessCommand(1, 2, new ClientAccessDto()), CancellationToken.None);

        Assert.False(result);
    }
}