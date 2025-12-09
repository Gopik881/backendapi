using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Clients.Queries.GetPagedClientCompanies;

public class GetPagedClientCompaniesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPaginatedResponse()
    {
        var repoMock = new Mock<IClientsRepository>();
        repoMock.Setup(r => r.GetFilteredClientCompaniesAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(new Tuple<List<ClientCompanyDto>, int>(
                new List<ClientCompanyDto>
                {
                    new ClientCompanyDto { Companies = new List<CompanyDto> { new CompanyDto() } }
                }, 1));

        var handler = new GetPagedClientCompaniesQueryHandler(repoMock.Object);
        var command = new GetPagedClientCompaniesQuery(1, 1, 10, null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PaginatedResponse<CompanyDto>>(result);
        Assert.Single(result.Data); // Fixed: Changed 'Items' to 'Data' based on the PaginatedResponse<T> definition.
    }
}