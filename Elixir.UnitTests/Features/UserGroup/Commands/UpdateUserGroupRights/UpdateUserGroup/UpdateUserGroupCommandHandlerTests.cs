using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.UpdateUserGroup;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class UpdateUserGroupCommandHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsTrue_ReturnsTrue()
    {
        // Arrange
        var userGroupId = 1;
        var userGroupDto = new CreateUserGroupDto { /* set properties as needed */ };
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.UpdateUserGroupAsync(userGroupDto))
            .ReturnsAsync(true);

        var handler = new UpdateUserGroupCommandHandler(repoMock.Object);
        var command = new UpdateUserGroupCommand(userGroupId, userGroupDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        repoMock.Verify(r => r.UpdateUserGroupAsync(userGroupDto), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsFalse_ReturnsFalse()
    {
        // Arrange
        var userGroupId = 2;
        var userGroupDto = new CreateUserGroupDto { /* set properties as needed */ };
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.UpdateUserGroupAsync(userGroupDto))
            .ReturnsAsync(false);

        var handler = new UpdateUserGroupCommandHandler(repoMock.Object);
        var command = new UpdateUserGroupCommand(userGroupId, userGroupDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        repoMock.Verify(r => r.UpdateUserGroupAsync(userGroupDto), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var userGroupId = 3;
        var userGroupDto = new CreateUserGroupDto { /* set properties as needed */ };
        var repoMock = new Mock<IUserGroupsRepository>();
        repoMock.Setup(r => r.UpdateUserGroupAsync(userGroupDto))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new UpdateUserGroupCommandHandler(repoMock.Object);
        var command = new UpdateUserGroupCommand(userGroupId, userGroupDto);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        repoMock.Verify(r => r.UpdateUserGroupAsync(userGroupDto), Times.Once);
    }
}