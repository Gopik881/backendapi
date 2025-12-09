using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Queries.CheckCompanyCode;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class CheckCompanyCodeExistsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompanyCode_WhenExists()
    {
        var repo = new Mock<ICompaniesRepository>();
        repo.Setup(r => r.FindCompanyByCodeAsync("EXIST")).ReturnsAsync(123);

        var handler = new CheckCompanyCodeExistsQueryHandler(repo.Object);
        var result = await handler.Handle(new CheckCompanyCodeExistsQuery("EXIST"), CancellationToken.None);

        Assert.Equal(123, result);
    }

    [Fact]
    public async Task Handle_ReturnsZero_WhenNotExists()
    {
        var repo = new Mock<ICompaniesRepository>();
        repo.Setup(r => r.FindCompanyByCodeAsync("NONE")).ReturnsAsync(0);

        var handler = new CheckCompanyCodeExistsQueryHandler(repo.Object);
        var result = await handler.Handle(new CheckCompanyCodeExistsQuery("NONE"), CancellationToken.None);

        Assert.Equal(0, result);
    }
}