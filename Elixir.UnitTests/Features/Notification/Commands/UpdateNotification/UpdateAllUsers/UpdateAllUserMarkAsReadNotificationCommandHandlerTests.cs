using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Notification.Commands.UpdateNotification.UpdateAllUsers;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.Notification.Commands.UpdateNotification.UpdateAllUsers
{
    public class UpdateAllUserMarkAsReadNotificationCommandHandlerTests
    {
        private readonly Mock<INotificationsRepository> _repoMock;
        private readonly UpdateAllUserMarkAsReadNotificationCommandHandler _handler;

        public UpdateAllUserMarkAsReadNotificationCommandHandlerTests()
        {
            _repoMock = new Mock<INotificationsRepository>();
            _handler = new UpdateAllUserMarkAsReadNotificationCommandHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsTrue_WhenRepositoryReturnsTrue()
        {
            // Arrange
            _repoMock.Setup(r => r.UpdateAllUserMarkAsReadAsync(1)).ReturnsAsync(true);
            var command = new UpdateAllUserMarkAsReadNotificationCommand(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _repoMock.Verify(r => r.UpdateAllUserMarkAsReadAsync(1), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFalse_WhenRepositoryReturnsFalse()
        {
            // Arrange
            _repoMock.Setup(r => r.UpdateAllUserMarkAsReadAsync(2)).ReturnsAsync(false);
            var command = new UpdateAllUserMarkAsReadNotificationCommand(2);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _repoMock.Verify(r => r.UpdateAllUserMarkAsReadAsync(2), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            _repoMock.Setup(r => r.UpdateAllUserMarkAsReadAsync(It.IsAny<int>())).ThrowsAsync(new Exception("DB error"));
            var command = new UpdateAllUserMarkAsReadNotificationCommand(3);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            _repoMock.Verify(r => r.UpdateAllUserMarkAsReadAsync(3), Times.Once);
        }
    }
}