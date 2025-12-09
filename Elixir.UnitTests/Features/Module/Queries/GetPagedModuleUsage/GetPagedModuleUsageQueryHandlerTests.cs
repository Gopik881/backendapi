using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Features.Module.Queries.GetPagedModuleUsage;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetPagedModuleUsageQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPaginatedResponse()
    {
        var mappingRepo = new Mock<IModuleMappingRepository>();
        mappingRepo.Setup(r => r.GetFilteredModulesUsageAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new Tuple<List<ModuleUsageDto>, int>(new List<ModuleUsageDto>(), 0));

        var handler = new GetPagedModuleUsageQueryHandler(mappingRepo.Object);
        var result = await handler.Handle(new GetPagedModuleUsageQuery("", 1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PaginatedResponse<ModuleUsageDto>>(result);
    }
}