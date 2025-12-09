using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Telephone.Commands.DeleteTelephone;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.Telephone.Commands.DeleteTelephone
{
    public class DeleteTelephoneCommandHandlerTests
    {
        private readonly Mock<ITelephoneCodeMasterRepository> _repoMock;
        private readonly Mock<IEntityReferenceService> _referenceServiceMock;
        private readonly DeleteTelephoneCommandHandler _handler;

        public DeleteTelephoneCommandHandlerTests()
        {
            _repoMock = new Mock<ITelephoneCodeMasterRepository>();
            _referenceServiceMock = new Mock<IEntityReferenceService>();
            _handler = new DeleteTelephoneCommandHandler(_repoMock.Object, _referenceServiceMock.Object);
        }

        [Fact]
        public async Task Handle_HasActiveReferences_ReturnsFalse_RepositoryNotCalled()
        {
            // Arrange
            var telephoneCodeId = 1;
            _referenceServiceMock
                .Setup(s => s.HasActiveReferencesAsync(nameof(DeleteTelephoneCommand.TelephoneCodeId), telephoneCodeId))
                .ReturnsAsync(true);

            var command = new DeleteTelephoneCommand(telephoneCodeId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _repoMock.Verify(r => r.DeleteTelephoneCodeAsync(It.IsAny<int>()), Times.Never);
            _referenceServiceMock.Verify(s => s.HasActiveReferencesAsync(nameof(DeleteTelephoneCommand.TelephoneCodeId), telephoneCodeId), Times.Once);
        }

        [Fact]
        public async Task Handle_NoActiveReferences_RepositoryReturnsTrue_ReturnsTrue()
        {
            // Arrange
            var telephoneCodeId = 2;
            _referenceServiceMock
                .Setup(s => s.HasActiveReferencesAsync(nameof(DeleteTelephoneCommand.TelephoneCodeId), telephoneCodeId))
                .ReturnsAsync(false);
            _repoMock
                .Setup(r => r.DeleteTelephoneCodeAsync(telephoneCodeId))
                .ReturnsAsync(true);

            var command = new DeleteTelephoneCommand(telephoneCodeId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _repoMock.Verify(r => r.DeleteTelephoneCodeAsync(telephoneCodeId), Times.Once);
            _referenceServiceMock.Verify(s => s.HasActiveReferencesAsync(nameof(DeleteTelephoneCommand.TelephoneCodeId), telephoneCodeId), Times.Once);
        }

        [Fact]
        public async Task Handle_NoActiveReferences_RepositoryReturnsFalse_ReturnsFalse()
        {
            // Arrange
            var telephoneCodeId = 3;
            _referenceServiceMock
                .Setup(s => s.HasActiveReferencesAsync(nameof(DeleteTelephoneCommand.TelephoneCodeId), telephoneCodeId))
                .ReturnsAsync(false);
            _repoMock
                .Setup(r => r.DeleteTelephoneCodeAsync(telephoneCodeId))
                .ReturnsAsync(false);

            var command = new DeleteTelephoneCommand(telephoneCodeId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _repoMock.Verify(r => r.DeleteTelephoneCodeAsync(telephoneCodeId), Times.Once);
            _referenceServiceMock.Verify(s => s.HasActiveReferencesAsync(nameof(DeleteTelephoneCommand.TelephoneCodeId), telephoneCodeId), Times.Once);
        }

        [Fact]
        public async Task Handle_ReferenceServiceThrows_ExceptionPropagates()
        {
            // Arrange
            var telephoneCodeId = 4;
            _referenceServiceMock
                .Setup(s => s.HasActiveReferencesAsync(nameof(DeleteTelephoneCommand.TelephoneCodeId), telephoneCodeId))
                .ThrowsAsync(new Exception("Reference service error"));

            var command = new DeleteTelephoneCommand(telephoneCodeId);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            _repoMock.Verify(r => r.DeleteTelephoneCodeAsync(It.IsAny<int>()), Times.Never);
            _referenceServiceMock.Verify(s => s.HasActiveReferencesAsync(nameof(DeleteTelephoneCommand.TelephoneCodeId), telephoneCodeId), Times.Once);
        }

        [Fact]
        public async Task Handle_RepositoryThrows_ExceptionPropagates()
        {
            // Arrange
            var telephoneCodeId = 5;
            _referenceServiceMock
                .Setup(s => s.HasActiveReferencesAsync(nameof(DeleteTelephoneCommand.TelephoneCodeId), telephoneCodeId))
                .ReturnsAsync(false);
            _repoMock
                .Setup(r => r.DeleteTelephoneCodeAsync(telephoneCodeId))
                .ThrowsAsync(new Exception("Repository error"));

            var command = new DeleteTelephoneCommand(telephoneCodeId);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            _repoMock.Verify(r => r.DeleteTelephoneCodeAsync(telephoneCodeId), Times.Once);
            _referenceServiceMock.Verify(s => s.HasActiveReferencesAsync(nameof(DeleteTelephoneCommand.TelephoneCodeId), telephoneCodeId), Times.Once);
        }
    }
}