using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Company.DTOs;

public class CreateCompany5TabCompanyAdminCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsTrue_WhenRepositoryReturnsTrue()
    {
        var repoMock = new Mock<ICompanyAdminUsersHistoryRepository>();
        repoMock.Setup(r => r.Company5TabCreateCompanyAdminDataAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Company5TabCompanyAdminDto>(),
                CancellationToken.None)) // Explicitly pass CancellationToken.None to avoid optional argument issue
            .ReturnsAsync(true);

        var handler = new CreateCompany5TabCompanyAdminCommandHandler(repoMock.Object);
        var command = new CreateCompany5TabCompanyAdminCommand(1, 2, new Company5TabCompanyAdminDto());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ReturnsFalse_WhenRepositoryReturnsFalse()
    {
        var repoMock = new Mock<ICompanyAdminUsersHistoryRepository>();
        repoMock.Setup(r => r.Company5TabCreateCompanyAdminDataAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Company5TabCompanyAdminDto>(),
                CancellationToken.None)) // Explicitly pass CancellationToken.None to avoid optional argument issue
            .ReturnsAsync(false);

        var handler = new CreateCompany5TabCompanyAdminCommandHandler(repoMock.Object);
        var command = new CreateCompany5TabCompanyAdminCommand(1, 2, new Company5TabCompanyAdminDto());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }
}