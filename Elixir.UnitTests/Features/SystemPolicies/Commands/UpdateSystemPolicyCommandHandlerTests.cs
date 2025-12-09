using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.SystemPolicies.Commands.UpdateSystemPolicy;
using Elixir.Application.Features.SystemPolicies.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.SystemPolicies.Commands
{
    public class UpdateSystemPolicyCommandHandlerTests
    {
        private readonly Mock<ISystemPoliciesRepository> _repositoryMock;
        private readonly UpdateSystemPolicyCommandHandler _handler;

        public UpdateSystemPolicyCommandHandlerTests()
        {
            _repositoryMock = new Mock<ISystemPoliciesRepository>();
            _handler = new UpdateSystemPolicyCommandHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsTrue_WhenUpdateIsSuccessful()
        {
            // Arrange
            var systemPolicy = new SystemPolicy { Id = 1 };
            var updateDto = new CreateUpdateSystemPolicyDto
            {
                NoOfSpecialCharacters = 2,
                NoOfUpperCase = 3,
                NoOfLowerCase = 4,
                MinLength = 8,
                MaxLength = 16,
                SpecialCharactersAllowed = "!@#",
                HistoricalPasswords = 5,
                PasswordValidityDays = 90,
                UnsuccessfulAttempts = 3,
                LockInPeriodInMinutes = 10,
                SessionTimeoutMinutes = 30,
                FileSizeLimitMb = 20
            };
            _repositoryMock.Setup(r => r.GetSystemPolicyByIdAsync(1)).ReturnsAsync(systemPolicy);
            _repositoryMock.Setup(r => r.UpdateSystemPolicyAsync(systemPolicy)).ReturnsAsync(true);

            var command = new UpdateSystemPolicyCommand(1, updateDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.GetSystemPolicyByIdAsync(1), Times.Once);
            _repositoryMock.Verify(r => r.UpdateSystemPolicyAsync(systemPolicy), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFalse_WhenSystemPolicyNotFound()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetSystemPolicyByIdAsync(1)).ReturnsAsync((SystemPolicy)null);
            var updateDto = new CreateUpdateSystemPolicyDto();
            var command = new UpdateSystemPolicyCommand(1, updateDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _repositoryMock.Verify(r => r.GetSystemPolicyByIdAsync(1), Times.Once);
            _repositoryMock.Verify(r => r.UpdateSystemPolicyAsync(It.IsAny<SystemPolicy>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ReturnsFalse_WhenRepositoryUpdateReturnsFalse()
        {
            // Arrange
            var systemPolicy = new SystemPolicy { Id = 1 };
            var updateDto = new CreateUpdateSystemPolicyDto();
            _repositoryMock.Setup(r => r.GetSystemPolicyByIdAsync(1)).ReturnsAsync(systemPolicy);
            _repositoryMock.Setup(r => r.UpdateSystemPolicyAsync(systemPolicy)).ReturnsAsync(false);

            var command = new UpdateSystemPolicyCommand(1, updateDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _repositoryMock.Verify(r => r.GetSystemPolicyByIdAsync(1), Times.Once);
            _repositoryMock.Verify(r => r.UpdateSystemPolicyAsync(systemPolicy), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrowsOnGet()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetSystemPolicyByIdAsync(1)).ThrowsAsync(new Exception("DB error"));
            var updateDto = new CreateUpdateSystemPolicyDto();
            var command = new UpdateSystemPolicyCommand(1, updateDto);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            _repositoryMock.Verify(r => r.GetSystemPolicyByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrowsOnUpdate()
        {
            // Arrange
            var systemPolicy = new SystemPolicy { Id = 1 };
            var updateDto = new CreateUpdateSystemPolicyDto();
            _repositoryMock.Setup(r => r.GetSystemPolicyByIdAsync(1)).ReturnsAsync(systemPolicy);
            _repositoryMock.Setup(r => r.UpdateSystemPolicyAsync(systemPolicy)).ThrowsAsync(new Exception("DB error"));

            var command = new UpdateSystemPolicyCommand(1, updateDto);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            _repositoryMock.Verify(r => r.GetSystemPolicyByIdAsync(1), Times.Once);
            _repositoryMock.Verify(r => r.UpdateSystemPolicyAsync(systemPolicy), Times.Once);
        }
    }
}