using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.UpdateHorizontals;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class UpdateHorizontalsCommandHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsTrue_ReturnsTrue()
    {
        // Arrange
        var userGroupId = 1;
        var horizontalsDto = new List<UserGroupHorizontals>
        {
            new UserGroupHorizontals { /* set properties as needed */ }
        };
        var repoMock = new Mock<IHorizontalsRepository>();
        repoMock.Setup(r => r.UpdateHorizontalsAsync(userGroupId, horizontalsDto))
            .ReturnsAsync(true);

        var handler = new UpdateHorizontalsCommandHandler(repoMock.Object);
        var command = new UpdateHorizontalsCommand(userGroupId, horizontalsDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        repoMock.Verify(r => r.UpdateHorizontalsAsync(userGroupId, horizontalsDto), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsFalse_ReturnsFalse()
    {
        // Arrange
        var userGroupId = 2;
        var horizontalsDto = new List<UserGroupHorizontals>
        {
            new UserGroupHorizontals { /* set properties as needed */ }
        };
        var repoMock = new Mock<IHorizontalsRepository>();
        repoMock.Setup(r => r.UpdateHorizontalsAsync(userGroupId, horizontalsDto))
            .ReturnsAsync(false);

        var handler = new UpdateHorizontalsCommandHandler(repoMock.Object);
        var command = new UpdateHorizontalsCommand(userGroupId, horizontalsDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        repoMock.Verify(r => r.UpdateHorizontalsAsync(userGroupId, horizontalsDto), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var userGroupId = 3;
        var horizontalsDto = new List<UserGroupHorizontals>
        {
            new UserGroupHorizontals { /* set properties as needed */ }
        };
        var repoMock = new Mock<IHorizontalsRepository>();
        repoMock.Setup(r => r.UpdateHorizontalsAsync(userGroupId, horizontalsDto))
            .ThrowsAsync(new Exception("Repository error"));

        var handler = new UpdateHorizontalsCommandHandler(repoMock.Object);
        var command = new UpdateHorizontalsCommand(userGroupId, horizontalsDto);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        repoMock.Verify(r => r.UpdateHorizontalsAsync(userGroupId, horizontalsDto), Times.Once);
    }
}