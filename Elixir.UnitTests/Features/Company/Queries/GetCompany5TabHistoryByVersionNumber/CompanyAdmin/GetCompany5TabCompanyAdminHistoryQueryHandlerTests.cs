using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.CompanyAdmin;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Company.DTOs;

public class GetCompany5TabCompanyAdminHistoryQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsDto_WhenRepositoryReturnsDto()
    {
        var repoMock = new Mock<ICompanyAdminUsersHistoryRepository>();
        var expected = new Company5TabHistoryDto();
        repoMock.Setup(r => r.GetCompany5TabCompanyAdminHistoryByVersionAsync(1, 2, 3))
                .ReturnsAsync(expected);

        var handler = new GetCompany5TabCompanyAdminHistoryQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetCompany5TabCompanyAdminHistoryQuery(1, 2, 3), CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_CallsRepositoryWithCorrectParameters()
    {
        var repoMock = new Mock<ICompanyAdminUsersHistoryRepository>();
        var handler = new GetCompany5TabCompanyAdminHistoryQueryHandler(repoMock.Object);
        var query = new GetCompany5TabCompanyAdminHistoryQuery(4, 5, 6);

        await handler.Handle(query, CancellationToken.None);

        repoMock.Verify(r => r.GetCompany5TabCompanyAdminHistoryByVersionAsync(4, 5, 6), Times.Once);
    }
}