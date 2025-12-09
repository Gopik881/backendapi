using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetCompanyAdmin;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetCompany5TabCompanyAdminQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompanyAdminDto()
    {
        var repo = new Mock<ICompanyAdminUsersHistoryRepository>();
        var expected = new Company5TabCompanyAdminDto();

        // Fix: Explicitly pass all arguments, including optional ones, to avoid CS0854
        repo.Setup(r => r.GetCompany5TabLatestCompanyAdminHistoryAsync(1, true, CancellationToken.None))
            .ReturnsAsync(expected);

        var handler = new GetCompany5TabCompanyAdminQueryHandler(repo.Object);
        var result = await handler.Handle(new GetCompany5TabCompanyAdminQuery(1, true), CancellationToken.None);

        Assert.Equal(expected, result);
    }
}