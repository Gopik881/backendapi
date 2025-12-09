using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Clients.Queries.GetPagedClientsWithFilters;

public class GetPagedClientsWithFiltersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPaginatedResponse()
    {
        var repoMock = new Mock<IClientsRepository>();
        repoMock.Setup(r => r.GetFilteredClientsAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(new Tuple<List<ClientDto>, int>(new List<ClientDto> { new ClientDto() }, 1));

        var handler = new GetPagedClientsWithFiltersQueryHandler(repoMock.Object);
        var command = new GetPagedClientsWithFiltersQuery(1, 10, null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PaginatedResponse<ClientDto>>(result);
        Assert.Single(result.Data);
    }
}