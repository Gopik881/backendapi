using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetElixirUsers;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetElixirUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsUserList_WhenRepositoryReturnsData()
    {
        var mockRepo = new Mock<IElixirUsersRepository>();
        var expected = new ElixirUserListDto();
        mockRepo.Setup(r => r.GetElixirUserListsByCompanyIdAsync(1)).ReturnsAsync(expected);

        var handler = new GetElixirUsersQueryHandler(mockRepo.Object);
        var result = await handler.Handle(new GetElixirUsersQuery(1), CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenRepositoryReturnsNull()
    {
        var mockRepo = new Mock<IElixirUsersRepository>();
        mockRepo.Setup(r => r.GetElixirUserListsByCompanyIdAsync(2)).ReturnsAsync((ElixirUserListDto)null);

        var handler = new GetElixirUsersQueryHandler(mockRepo.Object);
        var result = await handler.Handle(new GetElixirUsersQuery(2), CancellationToken.None);

        Assert.Null(result);
    }
}