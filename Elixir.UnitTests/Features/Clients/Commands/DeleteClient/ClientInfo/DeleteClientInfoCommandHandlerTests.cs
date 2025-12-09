using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.Commands.DeleteClient.ClientInfo;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class DeleteClientInfoCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Call_Repository_And_Return_True_When_Delete_Succeeds()
    {
        var repoMock = new Mock<IClientsRepository>();
        repoMock.Setup(r => r.DeleteClientInfoAsync(It.IsAny<int>())).ReturnsAsync(true);

        var handler = new DeleteClientInfoCommandHandler(repoMock.Object);
        var result = await handler.Handle(new DeleteClientInfoCommand(1), CancellationToken.None);

        Assert.True(result);
        repoMock.Verify(r => r.DeleteClientInfoAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_False_When_Repository_Returns_False()
    {
        var repoMock = new Mock<IClientsRepository>();
        repoMock.Setup(r => r.DeleteClientInfoAsync(It.IsAny<int>())).ReturnsAsync(false);

        var handler = new DeleteClientInfoCommandHandler(repoMock.Object);
        var result = await handler.Handle(new DeleteClientInfoCommand(2), CancellationToken.None);

        Assert.False(result);
        repoMock.Verify(r => r.DeleteClientInfoAsync(2), Times.Once);
    }
}