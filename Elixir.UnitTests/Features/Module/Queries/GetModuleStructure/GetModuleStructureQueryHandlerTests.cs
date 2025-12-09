using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetModuleStructureQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsModuleStructure()
    {
        var repoMock = new Mock<IModulesRepository>();
        var expected = new List<ModuleStrucureResponseDto> { new ModuleStrucureResponseDto() };
        repoMock.Setup(r => r.GetModulesWithSubModulesAsync()).ReturnsAsync(expected);

        var handler = new GetModuleStructureQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetModuleStructureQuery(), CancellationToken.None);

        Assert.Equal(expected, result);
        repoMock.Verify(r => r.GetModulesWithSubModulesAsync(), Times.Once);
    }
}