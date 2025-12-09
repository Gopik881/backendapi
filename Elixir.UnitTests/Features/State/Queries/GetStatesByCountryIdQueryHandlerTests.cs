using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Features.State.Queries.GetStatesbyCountryId;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetStatesByCountryIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCountryWithStates_WhenCountryExists()
    {
        var repoMock = new Mock<IStateMasterRepository>();
        var dto = new CountryWithStatesDto();
        repoMock.Setup(r => r.GetCountryByIdWithStatesAsync(1)).ReturnsAsync(dto);

        var handler = new GetStatesByCountryIdQueryHandler(repoMock.Object);
        var query = new GetStatesByCountryIdQuery(1);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(dto, result);
        repoMock.Verify(r => r.GetCountryByIdWithStatesAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenCountryDoesNotExist()
    {
        var repoMock = new Mock<IStateMasterRepository>();
        repoMock.Setup(r => r.GetCountryByIdWithStatesAsync(99)).ReturnsAsync((CountryWithStatesDto)null);

        var handler = new GetStatesByCountryIdQueryHandler(repoMock.Object);
        var query = new GetStatesByCountryIdQuery(99);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
        repoMock.Verify(r => r.GetCountryByIdWithStatesAsync(99), Times.Once);
    }
}