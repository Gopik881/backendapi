using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.CompanyAdmin;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class Company5TabWithDrawCompanyAdminCommandHandlerTests
{
    [Fact]
    public async Task Handle_CallsRepositoryAndReturnsResult()
    {
        var repo = new Mock<ICompanyAdminUsersHistoryRepository>();
        // Fix: Explicitly pass the CancellationToken instead of relying on optional arguments
        var cancellationToken = CancellationToken.None;
        repo.Setup(r => r.WithdrawCompany5TabCompanyAdminHistoryAsync(1, 2, cancellationToken)).ReturnsAsync(true);

        var handler = new Company5TabWithDrawCompanyAdminCommandHandler(repo.Object);
        var result = await handler.Handle(new Company5TabWithDrawCompanyAdminCommand(1, 2), cancellationToken);

        Assert.True(result);
        repo.Verify(r => r.WithdrawCompany5TabCompanyAdminHistoryAsync(1, 2, cancellationToken), Times.Once);
    }
}