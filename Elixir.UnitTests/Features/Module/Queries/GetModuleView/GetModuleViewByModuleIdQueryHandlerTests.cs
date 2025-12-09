using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Features.Module.Queries.GetModuleView;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetModuleViewByModuleIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_CallsRepositoryWithCorrectId()
    {
        var repoMock = new Mock<IModulesRepository>();
        var moduleId = 5;
        var expected = new ModuleDetailsDto();
        repoMock.Setup(r => r.GetModuleViewAsync(moduleId)).ReturnsAsync(expected);

        var handler = new GetModuleViewByModuleIdQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetModuleViewByModuleIdQuery(moduleId), CancellationToken.None);

        Assert.Equal(expected, result);
        repoMock.Verify(r => r.GetModuleViewAsync(moduleId), Times.Once);
    }
}