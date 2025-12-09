using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Features.Clients.Queries.GetClientUnmappedCompanies;

public class GetClientUnmappedComapniesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsListOfClientUnmappedCompaniesDto()
    {
        var repoMock = new Mock<IClientsRepository>();
        repoMock.Setup(r => r.GetClientUnmappedCompaniesAsync())
            .ReturnsAsync(new List<ClientUnmappedCompaniesDto>());

        var handler = new GetClientUnmappedComapniesQueryHandler(repoMock.Object);
        var command = new GetClientUnmappedCompaniesQuery();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<ClientUnmappedCompaniesDto>>(result);
    }
}