using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.User.Queries.GetPagedUserByUserGroup;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetPagedUserByUserGroupQueryHandlerTests
{
    [Fact]
    public async Task Handle_Always_ThrowsNotImplementedException()
    {
        // Arrange
        var userGroupMappingsRepo = new Mock<IUserGroupMappingsRepository>();
        var userGroupsRepo = new Mock<IUserGroupsRepository>();
        var usersRepo = new Mock<IUsersRepository>();

        var handler = new GetPagedUserByUserGroupQueryHandler(
            userGroupMappingsRepo.Object,
            usersRepo.Object,
            userGroupsRepo.Object
        );
        var query = new GetPagedUserByUserGroupQuery(1, 1, "search", 1, 10);

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => handler.Handle(query, CancellationToken.None));
    }
}