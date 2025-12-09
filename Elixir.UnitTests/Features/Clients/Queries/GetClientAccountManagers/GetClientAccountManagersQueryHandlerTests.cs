using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.Queries.GetClientAccountManagers;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Clients.DTOs;
using System.Collections.Generic;

public class GetClientAccountManagersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsClientGroupsWithAccountManagers()
    {
        var repoMock = new Mock<IClientsRepository>();
        var expected = new List<ClientGroupswithAccountManagersDto> { new() };
        repoMock.Setup(x => x.GetClientGroupswithAccountManagersAsync()).ReturnsAsync(expected);

        var handler = new GetClientAccountManagersQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetClientAccountManagersQuery(), CancellationToken.None);

        Assert.Equal(expected, result);
    }
}