using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Module.Queries.GetModuleMastersAndScreens;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetModuleMastersAndScreensQueryHandlerTests
{
    [Fact]
    public async Task Handle_CallsRepositoryWithCorrectParams()
    {
        var repoMock = new Mock<IModulesRepository>();
        var moduleIds = new List<int> { 1, 2 };
        var isMaster = true;
        var expected = new List<object> { new object() };
        repoMock.Setup(r => r.GetModuleMastersAndScreens(moduleIds, isMaster)).Returns(expected);

        var handler = new GetModuleMastersAndScreensQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetModuleMastersAndScreensQuery(moduleIds, isMaster), CancellationToken.None);

        Assert.Equal(expected, result);
        repoMock.Verify(r => r.GetModuleMastersAndScreens(moduleIds, isMaster), Times.Once);
    }
}