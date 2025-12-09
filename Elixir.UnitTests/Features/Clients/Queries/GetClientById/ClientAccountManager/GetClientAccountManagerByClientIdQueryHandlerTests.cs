using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.Queries.GetClientById.ClientAccountManager;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Clients.DTOs;
using System.Collections.Generic;

public class GetClientAccountManagerByClientIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsAccountManagers()
    {
        var repoMock = new Mock<IElixirUsersRepository>();
        var expected = new List<ClientAccountManagersDto> { new() };
        repoMock.Setup(x => x.GetClientAccountManagersByClientIdAsync(1)).ReturnsAsync(expected);

        var handler = new GetClientAccountManagerByClientIdQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetClientAccountManagerByClientIdQuery(1), CancellationToken.None);

        Assert.Equal(expected, result);
    }
}