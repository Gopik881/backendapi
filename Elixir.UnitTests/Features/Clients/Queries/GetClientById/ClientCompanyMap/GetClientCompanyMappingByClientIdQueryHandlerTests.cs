using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.DTOs;
using System.Collections.Generic;
using Elixir.Application.Interfaces.Persistance;

public class GetClientCompanyMappingByClientIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompanyMappings()
    {
        var repoMock = new Moq.Mock<IClientCompaniesMappingRepository>();
        var expected = new List<ClientCompanyMappingDto> { new() };
        repoMock.Setup(x => x.GetClientCompanyMappingByClientIdAsync(1)).ReturnsAsync(expected);

        var handler = new GetClientCompanyMappingByClientIdQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetClientCompanyMappingByClientIdQuery(1), CancellationToken.None);

        Assert.Equal(expected, result);
    }
}