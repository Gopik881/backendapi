using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.Queries.GetClientById.ClientAccess;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Clients.DTOs;

public class GetClientAccessByClientIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsClientAccess()
    {
        var repoMock = new Mock<IClientAccessRepository>();
        var expected = new ClientAccessDto();
        repoMock.Setup(x => x.GetClientAccessByClientIdAsync(1)).ReturnsAsync(expected);

        var handler = new GetClientAccessByClientIdQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetClientAccessByClientIdQuery(1), CancellationToken.None);

        Assert.Equal(expected, result);
    }
}