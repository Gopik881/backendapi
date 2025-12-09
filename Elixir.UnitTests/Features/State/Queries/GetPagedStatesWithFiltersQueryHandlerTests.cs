using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Features.State.Queries.GetPagedStatesWithFilters;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetPagedStatesWithFiltersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPaginatedResponse_WhenRepositoryReturnsData()
    {
        var repoMock = new Mock<IStateMasterRepository>();
        var dtos = new List<StateDto> { new StateDto() };
        repoMock.Setup(r => r.GetFilteredStatesAsync("test", 1, 10))
            .ReturnsAsync(new System.Tuple<List<StateDto>, int>(dtos, 1));

        var handler = new GetPagedStatesWithFiltersQueryHandler(repoMock.Object);
        var query = new GetPagedStatesWithFiltersQuery("test", 1, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
        repoMock.Verify(r => r.GetFilteredStatesAsync("test", 1, 10), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyPaginatedResponse_WhenRepositoryReturnsEmpty()
    {
        var repoMock = new Mock<IStateMasterRepository>();
        repoMock.Setup(r => r.GetFilteredStatesAsync(null, 2, 5))
            .ReturnsAsync(new System.Tuple<List<StateDto>, int>(new List<StateDto>(), 0));

        var handler = new GetPagedStatesWithFiltersQueryHandler(repoMock.Object);
        var query = new GetPagedStatesWithFiltersQuery(null, 2, 5);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Data);
        repoMock.Verify(r => r.GetFilteredStatesAsync(null, 2, 5), Times.Once);
    }
}