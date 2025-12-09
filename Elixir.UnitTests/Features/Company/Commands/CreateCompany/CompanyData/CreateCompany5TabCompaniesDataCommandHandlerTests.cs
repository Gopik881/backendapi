using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.CreateCompany.CompanyData;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Company.DTOs;

public class CreateCompany5TabCompaniesDataCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsTrue_WhenRepositoryReturnsTrue()
    {
        var repoMock = new Mock<ICompanyHistoryRepository>();
        repoMock.Setup(r => r.Company5TabCreateCompanyDataAsync(It.IsAny<int>(), It.IsAny<Company5TabCompanyDto>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreateCompany5TabCompaniesDataCommandHandler(repoMock.Object);
        var command = new CreateCompany5TabCommand(1, new Company5TabCompanyDto(), 10, 5);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ReturnsFalse_WhenRepositoryReturnsFalse()
    {
        var repoMock = new Mock<ICompanyHistoryRepository>();
        repoMock.Setup(r => r.Company5TabCreateCompanyDataAsync(It.IsAny<int>(), It.IsAny<Company5TabCompanyDto>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new CreateCompany5TabCompaniesDataCommandHandler(repoMock.Object);
        var command = new CreateCompany5TabCommand(1, new Company5TabCompanyDto(), 10, 5);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }
}