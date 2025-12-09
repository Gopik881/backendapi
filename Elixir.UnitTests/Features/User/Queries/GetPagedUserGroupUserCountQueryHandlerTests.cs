using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Elixir.Application.Features.User.Queries.GetPagedUserGroupUserCount;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.User.DTOs;

public class GetPagedUserGroupUserCountQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPaginatedResponse()
    {
        var repoMock = new Mock<IUserGroupMappingsRepository>();
        var data = new List<UserGroupUserCountDto> { new UserGroupUserCountDto() };
        var tupleData = new Tuple<List<UserGroupUserCountDto>, int>(data, 1);

        // Fix: Use Tuple explicitly in ReturnsAsync to match the expected type
        repoMock.Setup(r => r.GetFilteredUserAssociatedGroupAsync(false, "search", true, 1, 10))
            .ReturnsAsync(tupleData);

        var handler = new GetPagedUserGroupUserCountQueryHandler(repoMock.Object);
        var query = new GetPagedUserGroupUserCountQuery(false, "search", true, 1, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(1, result.Metadata.TotalItems);
    }
}