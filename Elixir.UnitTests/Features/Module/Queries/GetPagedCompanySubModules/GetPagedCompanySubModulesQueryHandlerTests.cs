using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Features.Module.Queries.GetPagedCompanySubModules;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetPagedSubModulesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPaginatedResponse()
    {
        var mappingRepo = new Mock<IModuleMappingRepository>();
        mappingRepo.Setup(r => r.GetFilteredCompanyModulesAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(new Tuple<List<CompanyModuleDto>, int>(new List<CompanyModuleDto>(), 0));

        var handler = new GetPagedSubModulesQueryHandler(mappingRepo.Object);
        var result = await handler.Handle(new GetPagedCompanySubModulesQuery(1, "", 1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PaginatedResponse<CompanyModuleDto>>(result);
    }
}