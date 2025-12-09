using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Features.Module.Queries.GetPagedCompanyModuleSubModules;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetPagedModuleInfoByCompanyQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsNull_WhenCompanyNotFound()
    {
        var companiesRepo = new Mock<ICompaniesRepository>();
        var mappingRepo = new Mock<IModuleMappingRepository>();

        companiesRepo.Setup(r => r.GetCompanyBasicInfoAsync(It.IsAny<int>())).ReturnsAsync((CompanyBasicInfoDto)null);

        var handler = new GetPagedModuleInfoByCompanyQueryHandler(companiesRepo.Object, mappingRepo.Object);
        var result = await handler.Handle(new GetPagedModuleInfoByCompany(1, "", 1, 10), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ReturnsPaginatedResponse_WhenCompanyFound()
    {
        var companiesRepo = new Mock<ICompaniesRepository>();
        var mappingRepo = new Mock<IModuleMappingRepository>();

        companiesRepo.Setup(r => r.GetCompanyBasicInfoAsync(It.IsAny<int>())).ReturnsAsync(new CompanyBasicInfoDto());

        // Fix for CS1929: Use a Tuple explicitly instead of relying on Moq's inference.
        mappingRepo.Setup(r => r.GetFilteredCompanyModulesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new Tuple<List<CompanyModuleDto>, int>(new List<CompanyModuleDto>(), 0));

        var handler = new GetPagedModuleInfoByCompanyQueryHandler(companiesRepo.Object, mappingRepo.Object);
        var result = await handler.Handle(new GetPagedModuleInfoByCompany(1, "", 1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PaginatedResponse<CompanyModuleDto>>(result);
    }
}