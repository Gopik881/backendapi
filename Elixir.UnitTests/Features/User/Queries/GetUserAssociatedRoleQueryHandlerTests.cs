using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
// using Elixir.Application.Features.User.Queries.GetUserAssociateRole;
// using Elixir.Application.Interfaces.Persistance;
// using Elixir.Application.Features.User.DTOs;

public class GetUserAssociatedRoleQueryHandlerTests
{
    // Uncomment and adjust if/when the handler is enabled in codebase
    // [Fact]
    // public async Task Handle_ReturnsUserRoles()
    // {
    //     var repoMock = new Mock<IRolesRepository>();
    //     var roles = new List<UserRoleDto> { new UserRoleDto() };
    //     repoMock.Setup(r => r.GetUserAssociatedRoleAsync(1)).ReturnsAsync(roles);
    //     var handler = new GetUserAssociatedRoleQueryHandler(repoMock.Object);
    //     var query = new GetUserAssociatedRoleQuery(1);
    //     var result = await handler.Handle(query, CancellationToken.None);
    //     Assert.Single(result);
    // }
}