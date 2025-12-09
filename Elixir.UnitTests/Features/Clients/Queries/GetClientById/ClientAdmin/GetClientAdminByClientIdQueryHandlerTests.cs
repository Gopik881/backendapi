using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;

public class GetClientAdminByClientIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsAdminInfo()
    {
        var repoMock = new Moq.Mock<IClientAdminInfoRepository>();
        var expected = new ClientAdminInfoDto();
        repoMock.Setup(x => x.GetClientAdminInfoByClientIdAsync(1)).ReturnsAsync(expected);

        var handler = new GetClientAdminByClientIdQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetClientAdminByClientIdQuery(1), CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenNoAdmin()
    {
        var repoMock = new Moq.Mock<IClientAdminInfoRepository>();
        repoMock.Setup(x => x.GetClientAdminInfoByClientIdAsync(1)).ReturnsAsync((ClientAdminInfoDto?)null);

        var handler = new GetClientAdminByClientIdQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetClientAdminByClientIdQuery(1), CancellationToken.None);

        Assert.Null(result);
    }
}