using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.ModuleMapping;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class Company5TabWithDrawModuleMappingCommandHandlerTests
{
    [Fact]
    public async Task Handle_CallsRepositoryAndReturnsResult()
    {
        var repo = new Mock<IModuleMappingHistoryRepository>();
        // Fix: Explicitly specify all arguments, including optional ones, to avoid the CS0854 error.
        repo.Setup(r => r.WithdrawCompany5TabModuleMappingHistoryAsync(1, 2, CancellationToken.None))
            .ReturnsAsync(true);

        var handler = new Company5TabWithDrawModuleMappingCommandHandler(repo.Object);
        var result = await handler.Handle(new Company5TabWithDrawModuleMappingCommand(1, 2), CancellationToken.None);

        Assert.True(result);
        repo.Verify(r => r.WithdrawCompany5TabModuleMappingHistoryAsync(1, 2, CancellationToken.None), Times.Once);
    }
}