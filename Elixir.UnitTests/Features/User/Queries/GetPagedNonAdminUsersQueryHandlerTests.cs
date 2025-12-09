using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Elixir.Application.Features.User.Queries.GetPagedNonAdminUsers;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.User.DTOs;

public class GetPagedNonAdminUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPaginatedResponse()
    {
        var repoMock = new Mock<IUsersRepository>();
        var users = new List<NonAdminUserDto> { new NonAdminUserDto() };
        var tupleResult = new Tuple<List<NonAdminUserDto>, int>(users, 1);

        repoMock.Setup(r => r.GetFilteredNonAdminUsersAsync("search", 1, 10))
            .ReturnsAsync(tupleResult);

        var handler = new GetPagedNonAdminUsersQueryHandler(repoMock.Object);
        var query = new GetPagedNonAdminUsersQuery("search", 1, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(1, result.Metadata.TotalItems); // Fixed: Accessing 'Metadata' instead of 'Pagination'
    }
}