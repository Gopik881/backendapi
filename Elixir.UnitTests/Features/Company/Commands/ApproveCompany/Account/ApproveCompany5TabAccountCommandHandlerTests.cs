using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.ApproveCompany.Account;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;

public class ApproveCompany5TabAccountCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsTrue_WhenRepositoryReturnsTrue()
    {
        var repoMock = new Mock<IAccountRepository>();
        repoMock.Setup(r => r.Company5TabApproveAccountInfoAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Company5TabAccountDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new ApproveCompany5TabAccountCommandHandler(repoMock.Object);
        var command = new ApproveCompany5TabAccountCommand(1, 2, new Company5TabAccountDto());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ReturnsFalse_WhenRepositoryReturnsFalse()
    {
        var repoMock = new Mock<IAccountRepository>();
        repoMock.Setup(r => r.Company5TabApproveAccountInfoAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Company5TabAccountDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new ApproveCompany5TabAccountCommandHandler(repoMock.Object);
        var command = new ApproveCompany5TabAccountCommand(1, 2, new Company5TabAccountDto());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }
}