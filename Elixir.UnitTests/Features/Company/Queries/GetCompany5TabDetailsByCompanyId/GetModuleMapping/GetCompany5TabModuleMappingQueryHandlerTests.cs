using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetModuleMapping;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetCompany5TabModuleMappingQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsModuleMappings()
    {
        var repo = new Mock<IModuleMappingHistoryRepository>();
        var expected = new List<Company5TabModuleMappingDto> { new Company5TabModuleMappingDto() };

        // Fix for CS0854: Explicitly specify all arguments instead of relying on optional arguments
        repo.Setup(r => r.GetCompany5TabLatestModuleMappingHistoryAsync(1, false, CancellationToken.None))
            .ReturnsAsync(expected);

        var handler = new GetCompany5TabModuleMappingQueryHandler(repo.Object);
        var result = await handler.Handle(new GetCompany5TabModuleMappingQuery(1, false), CancellationToken.None);

        Assert.Equal(expected, result);
    }
}