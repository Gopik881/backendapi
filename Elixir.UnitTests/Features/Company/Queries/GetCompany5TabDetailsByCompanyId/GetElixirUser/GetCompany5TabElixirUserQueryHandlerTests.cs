using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetElixirUser;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetCompany5TabElixirUserQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsElixirUserDtos()
    {
        var repo = new Mock<IElixirUsersHistoryRepository>();
        var expected = new List<Company5TabElixirUserDto> { new Company5TabElixirUserDto() };

        // Fix: Explicitly pass all arguments, including optional ones, to avoid CS0854
        repo.Setup(r => r.GetCompany5TabLatestElixirUsersHistoryAsync(1, false, CancellationToken.None))
            .ReturnsAsync(expected);

        var handler = new GetCompany5TabElixirUserQueryHandler(repo.Object);
        var result = await handler.Handle(new GetCompany5TabElixirUserQuery(1, false), CancellationToken.None);

        Assert.Equal(expected, result);
    }
}