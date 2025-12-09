using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetUserProfileByUserIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ValidUserIdAndIsSuperUser_ReturnsUserProfileDto()
    {
        // Arrange
        var userId = 1;
        var isSuperUser = true;
        var expectedProfile = new UserProfileDto { /* set properties as needed */ };
        var repoMock = new Mock<IUsersRepository>();
        repoMock.Setup(r => r.GetUserProfileByUserIdAsync(userId, isSuperUser))
            .ReturnsAsync(expectedProfile);

        var handler = new GetUserProfileByUserIdQueryHandler(repoMock.Object);
        var query = new GetUserProfileByUserIdQuery(userId, isSuperUser);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedProfile, result);
    }

    [Fact]
    public async Task Handle_UserIdNotFound_ReturnsNull()
    {
        // Arrange
        var userId = 2;
        var isSuperUser = false;
        var repoMock = new Mock<IUsersRepository>();
        repoMock.Setup(r => r.GetUserProfileByUserIdAsync(userId, isSuperUser))
            .ReturnsAsync((UserProfileDto)null);

        var handler = new GetUserProfileByUserIdQueryHandler(repoMock.Object);
        var query = new GetUserProfileByUserIdQuery(userId, isSuperUser);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var userId = 3;
        var isSuperUser = false;
        var repoMock = new Mock<IUsersRepository>();
        repoMock.Setup(r => r.GetUserProfileByUserIdAsync(userId, isSuperUser))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new GetUserProfileByUserIdQueryHandler(repoMock.Object);
        var query = new GetUserProfileByUserIdQuery(userId, isSuperUser);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
    }
}