using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Features.Module.Queries.GetPagedSubModules;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetPagedSubModulesQueryHandlerTestss
{
    [Fact]
    public async Task Handle_ReturnsPaginatedResponse_Success()
    {
        var subModulesRepo = new Mock<ISubModulesRepository>();
        subModulesRepo
            .Setup(r => r.GetFilteredSubModulesByModuleAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new Tuple<List<SubModuleDto>, int>(new List<SubModuleDto>(), 0));

        var handler = new GetPagedSubModulesQueryHandler(subModulesRepo.Object);
        var result = await handler.Handle(new GetPagedSubModulesQuery(1, "", 1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PaginatedResponse<SubModuleDto>>(result);
    }
}
