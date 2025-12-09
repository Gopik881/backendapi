using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetAllModulesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsAllModules()
    {
        var repoMock = new Mock<IModulesRepository>();
        var expected = new List<ModuleDto> { new ModuleDto() };
        repoMock.Setup(r => r.GetAllModulesAsync()).ReturnsAsync(expected);

        var handler = new GetAllModulesQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetAllModulesQuery(), CancellationToken.None);

        Assert.Equal(expected, result);
        repoMock.Verify(r => r.GetAllModulesAsync(), Times.Once);
    }
}