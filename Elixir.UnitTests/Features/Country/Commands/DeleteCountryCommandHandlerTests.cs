using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Country.Commands.DeleteCountry;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.Country.Commands
{
    public class DeleteCountryCommandHandlerTests
    {
        private readonly Mock<ICountryMasterRepository> _repoMock;
        private readonly DeleteCountryCommandHandler _handler;

        public DeleteCountryCommandHandlerTests()
        {
            _repoMock = new Mock<ICountryMasterRepository>();
            var entityReferenceServiceMock = new Mock<IEntityReferenceService>();
            _handler = new DeleteCountryCommandHandler(_repoMock.Object, entityReferenceServiceMock.Object);
        }

        [Fact]
        public async Task Handle_SuccessfulDelete_ReturnsTrue()
        {
            // Arrange
            int countryId = 1;
            _repoMock.Setup(r => r.DeleteCountryAsync(countryId)).ReturnsAsync(true);

            var command = new DeleteCountryCommand(countryId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _repoMock.Verify(r => r.DeleteCountryAsync(countryId), Times.Once);
        }

        [Fact]
        public async Task Handle_DeleteFails_ReturnsFalse()
        {
            // Arrange
            int countryId = 2;
            _repoMock.Setup(r => r.DeleteCountryAsync(countryId)).ReturnsAsync(false);

            var command = new DeleteCountryCommand(countryId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _repoMock.Verify(r => r.DeleteCountryAsync(countryId), Times.Once);
        }

        [Fact]
        public async Task Handle_RepositoryThrows_ExceptionPropagates()
        {
            // Arrange
            int countryId = 3;
            _repoMock.Setup(r => r.DeleteCountryAsync(countryId)).ThrowsAsync(new Exception("DB error"));

            var command = new DeleteCountryCommand(countryId);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Handle_InvalidCountryId_ReturnsFalse(int countryId)
        {
            // Arrange
            _repoMock.Setup(r => r.DeleteCountryAsync(countryId)).ReturnsAsync(false);

            var command = new DeleteCountryCommand(countryId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _repoMock.Verify(r => r.DeleteCountryAsync(countryId), Times.Once);
        }
    }
}