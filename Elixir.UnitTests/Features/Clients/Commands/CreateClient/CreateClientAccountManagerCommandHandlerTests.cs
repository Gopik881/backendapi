using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.Commands.CreateClient.ClientAccountManagersData;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class CreateClientAccountManagerCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Call_Repository_And_Return_True()
    {
        var repoMock = new Mock<IElixirUsersHistoryRepository>();
        repoMock.Setup(r => r.CreateClientAccountManagerDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<ClientAccountManagersDto>>())).ReturnsAsync(true);
        var handler = new CreateClientAccountManagerCommandHandler(repoMock.Object);

        var result = await handler.Handle(new CreateClientAccountManagerCommand(1, 2, new List<ClientAccountManagersDto>()), CancellationToken.None);

        Assert.True(result);
        repoMock.Verify(r => r.CreateClientAccountManagerDataAsync(1, 2, It.IsAny<List<ClientAccountManagersDto>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_False_When_Repository_Returns_False()
    {
        var repoMock = new Mock<IElixirUsersHistoryRepository>();
        repoMock.Setup(r => r.CreateClientAccountManagerDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<ClientAccountManagersDto>>())).ReturnsAsync(false);
        var handler = new CreateClientAccountManagerCommandHandler(repoMock.Object);

        var result = await handler.Handle(new CreateClientAccountManagerCommand(1, 2, new List<ClientAccountManagersDto>()), CancellationToken.None);

        Assert.False(result);
    }
}