using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetAccount;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetAccountCompany5TabAccountQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsAccountDto()
    {
        var repo = new Mock<IAccountHistoryRepository>();
        var expected = new Company5TabAccountDto();
        // Fix: Explicitly pass all arguments without relying on optional parameters
        repo.Setup(r => r.GetCompany5TabLatestAccountHistoryAsync(1, false, CancellationToken.None)).ReturnsAsync(expected);

        var handler = new GetAccountCompany5TabAccountQueryHandler(repo.Object);
        var result = await handler.Handle(new GetAccountCompany5TabAccountQuery(1, false), CancellationToken.None);

        Assert.Equal(expected, result);
    }
}