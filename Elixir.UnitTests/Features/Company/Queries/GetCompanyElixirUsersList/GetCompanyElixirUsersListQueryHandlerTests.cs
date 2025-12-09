using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompanyElixirUsersList;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetCompanyElixirUsersListQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsUserList_WhenRepositoryReturnsData()
    {
        var mockRepo = new Mock<IElixirUsersRepository>();
        var expected = new ElixirUserListDto();

        // Use the correct overload (string?) and return the expected DTO.
        mockRepo.Setup(r => r.GetUserListsFromUserGroupMappingAsync(It.IsAny<string>()))
                .ReturnsAsync(expected);

        var handler = new GetCompanyElixirUsersListQueryHandler(mockRepo.Object);
        var result = await handler.Handle(new GetCompanyElixirUsersListQuery(), CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenRepositoryReturnsNull()
    {
        var mockRepo = new Mock<IElixirUsersRepository>();

        // Return null using null-forgiving to satisfy the compiler while allowing runtime null.
        mockRepo.Setup(r => r.GetUserListsFromUserGroupMappingAsync(It.IsAny<string>()))
                .ReturnsAsync((ElixirUserListDto)null!);

        var handler = new GetCompanyElixirUsersListQueryHandler(mockRepo.Object);
        var result = await handler.Handle(new GetCompanyElixirUsersListQuery(), CancellationToken.None);

        Assert.Null(result);
    }
}