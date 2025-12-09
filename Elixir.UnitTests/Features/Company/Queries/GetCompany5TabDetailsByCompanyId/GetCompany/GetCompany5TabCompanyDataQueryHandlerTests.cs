using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetCompany;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetCompany5TabCompanyDataQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompanyDto()
    {
        var repo = new Mock<ICompanyHistoryRepository>();
        var expected = new Company5TabCompanyDto();

        // Fix: Explicitly pass all arguments without relying on optional parameters
        repo.Setup(r => r.GetCompany5TabLatestCompanyHistoryAsync(1, 2, false, CancellationToken.None))
            .ReturnsAsync(expected);

        var handler = new GetCompany5TabCompanyDataQueryHandler(repo.Object);
        var result = await handler.Handle(new GetCompany5TabCompanyDataQuery(1, 2, false), CancellationToken.None);

        Assert.Equal(expected, result);
    }
}