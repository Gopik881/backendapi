using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.ElixirUser;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class Company5TabWithDrawElixirUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_CallsRepositoryAndReturnsResult()
    {
        var repo = new Mock<IElixirUsersHistoryRepository>();
        // Fix: Explicitly pass all arguments, including optional ones, to avoid CS0854
        repo.Setup(r => r.WithdrawCompany5TabElixirUsersHistoryAsync(1, 2, CancellationToken.None)).ReturnsAsync(true);

        var handler = new Company5TabWithDrawElixirUserCommandHandler(repo.Object);
        var result = await handler.Handle(new Company5TabWithDrawElixirUserCommand(1, 2), CancellationToken.None);

        Assert.True(result);
        repo.Verify(r => r.WithdrawCompany5TabElixirUsersHistoryAsync(1, 2, CancellationToken.None), Times.Once);
    }
}