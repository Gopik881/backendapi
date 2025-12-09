using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.ApproveCompany.CompanyAdmin;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Moq;
using Xunit;

public class ApproveCompany5TabCompanyAdminCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenRepositoryReturnsTrue()
    {
        var repoMock = new Mock<ICompanyAdminUsersRepository>();
        var cryptoMock = new Mock<ICryptoService>();
        var dto = new Company5TabCompanyAdminDto { CompanyAdminEmailId = "test@email.com" };

        // Fix for CS0854: Explicitly specify the optional argument instead of relying on its default value
        cryptoMock.Setup(x => x.GenerateIntegerHashForString(dto.CompanyAdminEmailId, true)).Returns(123);

        // Fix for CS0854: Explicitly pass the CancellationToken argument instead of relying on its default value
        repoMock.Setup(x => x.Company5TabApproveCompanyAdminDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Company5TabCompanyAdminDto>(), CancellationToken.None))
            .ReturnsAsync(true);

        var handler = new ApproveCompany5TabCompanyAdminCommandHandler(repoMock.Object, cryptoMock.Object);
        var command = new ApproveCompany5TabCompanyAdminCommand(1, 2, dto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        Assert.Equal(123, dto.CompanyAdminEmailHash);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenRepositoryReturnsFalse()
    {
        var repoMock = new Mock<ICompanyAdminUsersRepository>();
        var cryptoMock = new Mock<ICryptoService>();
        var dto = new Company5TabCompanyAdminDto { CompanyAdminEmailId = "test@email.com" };

        // Fix for CS0854: Explicitly specify the optional argument instead of relying on its default value
        cryptoMock.Setup(x => x.GenerateIntegerHashForString(dto.CompanyAdminEmailId, true)).Returns(123);

        // Fix for CS0854: Explicitly pass the CancellationToken argument instead of relying on its default value
        repoMock.Setup(x => x.Company5TabApproveCompanyAdminDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Company5TabCompanyAdminDto>(), CancellationToken.None))
            .ReturnsAsync(false);

        var handler = new ApproveCompany5TabCompanyAdminCommandHandler(repoMock.Object, cryptoMock.Object);
        var command = new ApproveCompany5TabCompanyAdminCommand(1, 2, dto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }
}