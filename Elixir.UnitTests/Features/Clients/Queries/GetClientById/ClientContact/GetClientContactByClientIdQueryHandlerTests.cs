using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.DTOs;
using System.Collections.Generic;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Clients.Queries.GetClientById.ClientContact;

public class GetClientContactByClientIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsContactInfo()
    {
        var repoMock = new Moq.Mock<IClientContactDetailsRepository>();
        var expected = new List<ClientContactInfoDto> { new() };
        repoMock.Setup(x => x.GetClientContactDataByClientIdAsync(1)).ReturnsAsync(expected);

        var handler = new GetClientContactByClientIdQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetClientContactByClientIdQuery(1), CancellationToken.None);

        Assert.Equal(expected, result);
    }
}