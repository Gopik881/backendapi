using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.ApproveCompany.CompanyData;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class ApproveCompany5TabCompaniesDataCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenRepositoryReturnsTrue()
    {
        var repoMock = new Mock<ICompaniesRepository>();
        repoMock.Setup(x => x.Company5TabApproveCompanyDataAsync(It.IsAny<int>(), It.IsAny<Company5TabCompanyDto>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new ApproveCompany5TabCompaniesDataCommandHandler(repoMock.Object);
        var command = new ApproveCompany5TabCommand(1, new Company5TabCompanyDto(), 10, 20);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenRepositoryReturnsFalse()
    {
        var repoMock = new Mock<ICompaniesRepository>();
        repoMock.Setup(x => x.Company5TabApproveCompanyDataAsync(It.IsAny<int>(), It.IsAny<Company5TabCompanyDto>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new ApproveCompany5TabCompaniesDataCommandHandler(repoMock.Object);
        var command = new ApproveCompany5TabCommand(1, new Company5TabCompanyDto(), 10, 20);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }
}