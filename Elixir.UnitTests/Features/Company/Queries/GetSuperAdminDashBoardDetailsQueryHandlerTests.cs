using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Queries.GetSuperAdminDashBoardDetails;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Company.DTOs;

public class GetSuperAdminDashBoardDetailsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsSuperAdminDashBoardDetails()
    {
        var mockRepo = new Mock<ICompaniesRepository>();
        var expectedDto = new SuperAdminDashBoardDetailsDto();
        mockRepo.Setup(r => r.GetSuperAdminDashBoardDetailsAsync())
            .ReturnsAsync(expectedDto);

        var handler = new GetSuperAdminDashBoardDetailsQueryHandler(mockRepo.Object);
        var query = new GetSuperAdminDashBoardDetailsQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(expectedDto, result);
    }
}