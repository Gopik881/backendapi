using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Module.DTOs;

public class GetCleintpopupDetailsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsClientPopupDetailsDto()
    {
        var repoMock = new Mock<IClientsRepository>();
        repoMock.Setup(r => r.GetClientDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync(new ClientPopupDetailsDto());

        var handler = new GetCleintpopupDetailsQueryHandler(repoMock.Object);
        var command = new GetCleintpopupDetailsQuery(1);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<ClientPopupDetailsDto>(result);
    }
}