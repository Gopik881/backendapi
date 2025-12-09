using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.EscalationContacts;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Company.DTOs;

public class GetCompany5TabEscalationContactsHistoryQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsDto_WhenRepositoryReturnsDto()
    {
        var repoMock = new Mock<IEscalationContactsHistoryRepository>();
        var expected = new Company5TabHistoryDto();
        repoMock.Setup(r => r.GetCompany5TabEscalationContactsHistoryByVersionAsync(1, 2, 3))
                .ReturnsAsync(expected);

        var handler = new GetCompany5TabEscalationContactsHistoryQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetCompany5TabEscalationContactsHistoryQuery(1, 2, 3), CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_CallsRepositoryWithCorrectParameters()
    {
        var repoMock = new Mock<IEscalationContactsHistoryRepository>();
        var handler = new GetCompany5TabEscalationContactsHistoryQueryHandler(repoMock.Object);
        var query = new GetCompany5TabEscalationContactsHistoryQuery(4, 5, 6);

        await handler.Handle(query, CancellationToken.None);

        repoMock.Verify(r => r.GetCompany5TabEscalationContactsHistoryByVersionAsync(4, 5, 6), Times.Once);
    }
}