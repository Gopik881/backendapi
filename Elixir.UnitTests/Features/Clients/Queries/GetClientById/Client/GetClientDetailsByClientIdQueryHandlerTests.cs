using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.Queries.GetClientById.Client;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Clients.DTOs;

public class GetClientDetailsByClientIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsClientInfo()
    {
        var repoMock = new Mock<IClientsRepository>();
        var expected = new ClientInfoDto();
        repoMock.Setup(x => x.GetClientDetailsByIdAsync(1)).ReturnsAsync(expected);

        var handler = new GetClientDetailsByClientIdQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetClientDetailsByClientIdQuery(1), CancellationToken.None);

        Assert.Equal(expected, result);
    }
}