using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Notification.Commands.UpdateNotification.UpdateUser;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.Notification.Commands.UpdateNotification.UpdateUser
{
    public class UpdateUserMarkAsReadNotificationCommandHandlerTests
    {
        private readonly Mock<INotificationsRepository> _repoMock;
        private readonly UpdateUserMarkAsReadNotificationCommandHandler _handler;

        public UpdateUserMarkAsReadNotificationCommandHandlerTests()
        {
            _repoMock = new Mock<INotificationsRepository>();
            _handler = new UpdateUserMarkAsReadNotificationCommandHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsTrue_WhenRepositoryReturnsTrue()
        {
            // Arrange
            _repoMock.Setup(r => r.UpdateUserMarkAsReadAsync(1)).ReturnsAsync(true);
            var command = new UpdateUserMarkAsReadNotificationCommand(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _repoMock.Verify(r => r.UpdateUserMarkAsReadAsync(1), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFalse_WhenRepositoryReturnsFalse()
        {
            // Arrange
            _repoMock.Setup(r => r.UpdateUserMarkAsReadAsync(2)).ReturnsAsync(false);
            var command = new UpdateUserMarkAsReadNotificationCommand(2);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _repoMock.Verify(r => r.UpdateUserMarkAsReadAsync(2), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            _repoMock.Setup(r => r.UpdateUserMarkAsReadAsync(It.IsAny<int>())).ThrowsAsync(new Exception("DB error"));
            var command = new UpdateUserMarkAsReadNotificationCommand(3);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            _repoMock.Verify(r => r.UpdateUserMarkAsReadAsync(3), Times.Once);
        }
    }
}