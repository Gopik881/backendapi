using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.Commands.CreateClient.ClientContactData;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class CreateClientContactCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Call_Repository_And_Return_True()
    {
        var repoMock = new Mock<IClientContactDetailsRepository>();
        repoMock.Setup(r => r.CreateClientContactDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<ClientContactInfoDto>>())).ReturnsAsync(true);
        var handler = new CreateClientContactCommandHandler(repoMock.Object);

        var result = await handler.Handle(new CreateClientContactCommand(1, 2, new List<ClientContactInfoDto>()), CancellationToken.None);

        Assert.True(result);
        repoMock.Verify(r => r.CreateClientContactDataAsync(1, 2, It.IsAny<List<ClientContactInfoDto>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_False_When_Repository_Returns_False()
    {
        var repoMock = new Mock<IClientContactDetailsRepository>();
        repoMock.Setup(r => r.CreateClientContactDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<ClientContactInfoDto>>())).ReturnsAsync(false);
        var handler = new CreateClientContactCommandHandler(repoMock.Object);

        var result = await handler.Handle(new CreateClientContactCommand(1, 2, new List<ClientContactInfoDto>()), CancellationToken.None);

        Assert.False(result);
    }
}