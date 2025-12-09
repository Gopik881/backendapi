using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Clients.DTOs;

public class UpdateClientCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsTrue()
    {
        var repoMock = new Mock<IClientsRepository>();
        repoMock.Setup(x => x.UpdateClientInformationAsync(It.IsAny<ClientInfoDto>(), 1, 2)).ReturnsAsync(true);

        var handler = new UpdateClientCommandHandler(repoMock.Object);
        var command = new UpdateClientCommand(2, 1, new ClientInfoDto());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsFalse_ReturnsFalse()
    {
        var repoMock = new Mock<IClientsRepository>();
        repoMock.Setup(x => x.UpdateClientInformationAsync(It.IsAny<ClientInfoDto>(), 1, 2)).ReturnsAsync(false);

        var handler = new UpdateClientCommandHandler(repoMock.Object);
        var command = new UpdateClientCommand(2, 1, new ClientInfoDto());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }
}