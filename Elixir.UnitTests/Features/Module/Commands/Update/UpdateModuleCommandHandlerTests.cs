using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Module.Commands.Update;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class UpdateModuleCommandHandlerTests
{
    [Fact]
    public async Task Handle_CallsRepositoryAndReturnsResult()
    {
        var repoMock = new Mock<IModulesRepository>();
        var dto = new ModuleCreateDto();
        var expected = new ModuleStructureResponseV2();
        repoMock.Setup(r => r.UpdateModuleStructure(dto)).ReturnsAsync(expected);

        var handler = new UpdateModuleCommandHandler(repoMock.Object);
        var result = await handler.Handle(new UpdateModuleCommand(dto), CancellationToken.None);

        Assert.Equal(expected, result);
        repoMock.Verify(r => r.UpdateModuleStructure(dto), Times.Once);
    }
}