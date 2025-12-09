using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Module.Queries.GetModuleSubmoduleList;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetModuleSubModuleListQueryHandlerTests
{
    [Fact]
    public async Task Handle_CallsRepositoryWithCorrectParams()
    {
        var repoMock = new Mock<IModulesRepository>();
        var moduleIds = new List<int> { 1, 2 };
        var expected = new List<object> { new object() };
        repoMock.Setup(r => r.GetModuleSubmoduleListAsync(moduleIds)).ReturnsAsync(expected);

        var handler = new GetModuleSubModuleListQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetModuleSubModuleListQuery(moduleIds), CancellationToken.None);

        Assert.Equal(expected, result);
        repoMock.Verify(r => r.GetModuleSubmoduleListAsync(moduleIds), Times.Once);
    }
}