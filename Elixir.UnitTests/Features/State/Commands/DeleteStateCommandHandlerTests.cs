using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.State.Commands.DeleteState;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.State.Commands
{
    public class DeleteStateCommandHandlerTests
    {
        private readonly Mock<IStateMasterRepository> _stateRepoMock;
        private readonly Mock<IEntityReferenceService> _entityRefServiceMock;
        private readonly DeleteStateCommandHandler _handler;

        public DeleteStateCommandHandlerTests()
        {
            _stateRepoMock = new Mock<IStateMasterRepository>();
            _entityRefServiceMock = new Mock<IEntityReferenceService>();
            _handler = new DeleteStateCommandHandler(_stateRepoMock.Object, _entityRefServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsFalse_WhenHasActiveReferences()
        {
            // Arrange
            _entityRefServiceMock
                .Setup(s => s.HasActiveReferencesAsync(nameof(DeleteStateCommand.StateId), 1))
                .ReturnsAsync(true);

            var command = new DeleteStateCommand(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _entityRefServiceMock.Verify(s => s.HasActiveReferencesAsync(nameof(DeleteStateCommand.StateId), 1), Times.Once);
            _stateRepoMock.Verify(r => r.DeleteStateAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ReturnsTrue_WhenNoReferencesAndDeleteSucceeds()
        {
            // Arrange
            _entityRefServiceMock
                .Setup(s => s.HasActiveReferencesAsync(nameof(DeleteStateCommand.StateId), 2))
                .ReturnsAsync(false);
            _stateRepoMock
                .Setup(r => r.DeleteStateAsync(2))
                .ReturnsAsync(true);

            var command = new DeleteStateCommand(2);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _entityRefServiceMock.Verify(s => s.HasActiveReferencesAsync(nameof(DeleteStateCommand.StateId), 2), Times.Once);
            _stateRepoMock.Verify(r => r.DeleteStateAsync(2), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFalse_WhenNoReferencesAndDeleteFails()
        {
            // Arrange
            _entityRefServiceMock
                .Setup(s => s.HasActiveReferencesAsync(nameof(DeleteStateCommand.StateId), 3))
                .ReturnsAsync(false);
            _stateRepoMock
                .Setup(r => r.DeleteStateAsync(3))
                .ReturnsAsync(false);

            var command = new DeleteStateCommand(3);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _entityRefServiceMock.Verify(s => s.HasActiveReferencesAsync(nameof(DeleteStateCommand.StateId), 3), Times.Once);
            _stateRepoMock.Verify(r => r.DeleteStateAsync(3), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenHasActiveReferencesThrows()
        {
            // Arrange
            _entityRefServiceMock
                .Setup(s => s.HasActiveReferencesAsync(nameof(DeleteStateCommand.StateId), 4))
                .ThrowsAsync(new Exception("Reference check error"));

            var command = new DeleteStateCommand(4);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            _entityRefServiceMock.Verify(s => s.HasActiveReferencesAsync(nameof(DeleteStateCommand.StateId), 4), Times.Once);
            _stateRepoMock.Verify(r => r.DeleteStateAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenDeleteStateAsyncThrows()
        {
            // Arrange
            _entityRefServiceMock
                .Setup(s => s.HasActiveReferencesAsync(nameof(DeleteStateCommand.StateId), 5))
                .ReturnsAsync(false);
            _stateRepoMock
                .Setup(r => r.DeleteStateAsync(5))
                .ThrowsAsync(new Exception("Delete error"));

            var command = new DeleteStateCommand(5);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            _entityRefServiceMock.Verify(s => s.HasActiveReferencesAsync(nameof(DeleteStateCommand.StateId), 5), Times.Once);
            _stateRepoMock.Verify(r => r.DeleteStateAsync(5), Times.Once);
        }
    }
}