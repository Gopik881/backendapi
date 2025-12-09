using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class CreateClientCompanyMapCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Call_Repository_And_Return_True()
    {
        var repoMock = new Mock<IClientCompaniesMappingRepository>();
        repoMock.Setup(r => r.CreateClientCompanyMapDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<ClientCompanyMappingDto>>(), It.IsAny<string>())).ReturnsAsync(true);
        var handler = new CreateClientCompanyMapCommandHandler(repoMock.Object);

        var result = await handler.Handle(new CreateClientCompanyMapCommand(1, 2, new List<ClientCompanyMappingDto>(), "TestClient"), CancellationToken.None);

        Assert.True(result);
        repoMock.Verify(r => r.CreateClientCompanyMapDataAsync(1, 2, It.IsAny<List<ClientCompanyMappingDto>>(), "TestClient"), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_False_When_Repository_Returns_False()
    {
        var repoMock = new Mock<IClientCompaniesMappingRepository>();
        repoMock.Setup(r => r.CreateClientCompanyMapDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<ClientCompanyMappingDto>>(), It.IsAny<string>())).ReturnsAsync(false);
        var handler = new CreateClientCompanyMapCommandHandler(repoMock.Object);

        var result = await handler.Handle(new CreateClientCompanyMapCommand(1, 2, new List<ClientCompanyMappingDto>(), "TestClient"), CancellationToken.None);

        Assert.False(result);
    }
}