using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Queries.GetPagedAccountManagerByCompany;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using System.Collections.Generic;

public class GetPagedAccountManagerByCompnayQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPaginatedResponse()
    {
        var mockRepo = new Mock<IElixirUsersRepository>();
        var expectedData = new List<CompanyUserDto> { new CompanyUserDto() };
        var expectedResult = new Tuple<List<CompanyUserDto>, int>(expectedData, 1);

        mockRepo.Setup(r => r.GetFilteredAccountManagersAsync(1, "search", 1, 10))
            .ReturnsAsync(expectedResult);

        var handler = new GetPagedAccountManagerByCompnayQueryHandler(mockRepo.Object);
        var query = new GetPagedAccountManagerByCompnayQuery(1, "search", 1, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(1, result.Metadata.TotalItems); // Fix: Access 'Metadata' instead of 'Pagination'
    }
}