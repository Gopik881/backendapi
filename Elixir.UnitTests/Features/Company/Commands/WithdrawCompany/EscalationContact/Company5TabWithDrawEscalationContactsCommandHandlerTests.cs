using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.EscalationContact;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class Company5TabWithDrawEscalationContactsCommandHandlerTests
{
    [Fact]
    public async Task Handle_CallsRepositoryAndReturnsResult()
    {
        var repo = new Mock<IEscalationContactsHistoryRepository>();
        // Fix: Explicitly pass all arguments, including optional ones, to avoid CS0854
        repo.Setup(r => r.WithdrawCompany5TabEscalationContactsHistoryAsync(1, 2, CancellationToken.None))
            .ReturnsAsync(true);

        var handler = new Company5TabWithDrawEscalationContactsCommandHandler(repo.Object);
        var result = await handler.Handle(new Company5TabWithDrawEscalationContactsCommand(1, 2), CancellationToken.None);

        Assert.True(result);
        repo.Verify(r => r.WithdrawCompany5TabEscalationContactsHistoryAsync(1, 2, CancellationToken.None), Times.Once);
    }
}