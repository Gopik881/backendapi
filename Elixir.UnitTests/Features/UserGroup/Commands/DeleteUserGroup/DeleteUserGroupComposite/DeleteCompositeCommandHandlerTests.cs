using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteUserGroupComposite;
using Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteHorizontals;
using Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteReportAccess;
using Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteReportingAdmins;
using Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteUserGroupDetails;
using Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteUserRights;
using Elixir.Application.Interfaces.Persistance;
using MediatR;
using Moq;
using Xunit;

public class DeleteCompositeCommandHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<ITransactionRepository> _transactionRepoMock = new();

    private DeleteCompositeCommandHandler CreateHandler()
    {
        return new DeleteCompositeCommandHandler(
            () => _transactionRepoMock.Object,
            _mediatorMock.Object
        );
    }

    [Fact]
    public async Task Handle_AllDeletesSucceed_CommitsAndReturnsTrue()
    {
        // Arrange
        var userGroupId = 1;
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteHorizontalsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteReportAccessCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteReportingAdminsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserGroupDetailsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserRightsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _transactionRepoMock.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _transactionRepoMock.Setup(t => t.CommitAsync()).Returns(Task.CompletedTask);

        var handler = CreateHandler();
        var command = new DeleteCompositeCommand(userGroupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _transactionRepoMock.Verify(t => t.CommitAsync(), Times.Once);
        _transactionRepoMock.Verify(t => t.RollbackAsync(), Times.Never);
    }

    [Theory]
    [InlineData(0)] // Horizontals fails
    [InlineData(1)] // ReportAccess fails
    [InlineData(2)] // ReportingAdmins fails
    [InlineData(3)] // UserGroupDetails fails
    [InlineData(4)] // UserRights fails
    public async Task Handle_AnyDeleteFails_RollsBackAndReturnsFalse(int failIndex)
    {
        // Arrange
        var userGroupId = 1;
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteHorizontalsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(failIndex != 0);
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteReportAccessCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(failIndex != 1);
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteReportingAdminsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(failIndex != 2);
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserGroupDetailsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(failIndex != 3);
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserRightsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(failIndex != 4);

        _transactionRepoMock.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _transactionRepoMock.Setup(t => t.RollbackAsync()).Returns(Task.CompletedTask);

        var handler = CreateHandler();
        var command = new DeleteCompositeCommand(userGroupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        _transactionRepoMock.Verify(t => t.RollbackAsync(), Times.Once);
        _transactionRepoMock.Verify(t => t.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ExceptionDuringDelete_RollsBackAndReturnsFalse()
    {
        // Arrange
        var userGroupId = 1;
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteHorizontalsCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Delete failed"));

        _transactionRepoMock.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _transactionRepoMock.Setup(t => t.RollbackAsync()).Returns(Task.CompletedTask);

        var handler = CreateHandler();
        var command = new DeleteCompositeCommand(userGroupId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        _transactionRepoMock.Verify(t => t.RollbackAsync(), Times.Once);
        _transactionRepoMock.Verify(t => t.CommitAsync(), Times.Never);
    }
}