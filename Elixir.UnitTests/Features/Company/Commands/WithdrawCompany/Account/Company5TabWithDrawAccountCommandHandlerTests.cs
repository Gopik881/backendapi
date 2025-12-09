using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.Account;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class Company5TabWithDrawAccountCommandHandlerTests
{
    [Fact]
    public async Task Handle_CallsRepositoryAndReturnsResult()
    {
        var repo = new Mock<IAccountHistoryRepository>();
        // Fix: Explicitly pass the CancellationToken instead of relying on optional arguments
        var cancellationToken = CancellationToken.None;
        repo.Setup(r => r.WithdrawCompany5TabAccountHistoryAsync(1, 2, cancellationToken)).ReturnsAsync(true);

        var handler = new Company5TabWithDrawAccountCommandHandler(repo.Object);
        var result = await handler.Handle(new Company5TabWithDrawAccountCommand(1, 2), cancellationToken);

        Assert.True(result);
        repo.Verify(r => r.WithdrawCompany5TabAccountHistoryAsync(1, 2, cancellationToken), Times.Once);
    }
}