using Elixir.Application.Features.Currency.Commands.DeleteCurrency;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Moq;

namespace Elixir.UnitTests.Features.Currency.Commands
{
    public class DeleteCurrencyCommandHandlerTests
    {
        private readonly Mock<ICurrencyMasterRepository> _repositoryMock;
        private readonly Mock<IEntityReferenceService> _entityReferenceServiceMock;
        private readonly DeleteCurrencyCommandHandler _handler;

        public DeleteCurrencyCommandHandlerTests()
        {
            _repositoryMock = new Mock<ICurrencyMasterRepository>();
            _entityReferenceServiceMock = new Mock<IEntityReferenceService>();
            _handler = new DeleteCurrencyCommandHandler(_repositoryMock.Object, _entityReferenceServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsTrue_WhenDeleteIsSuccessful()
        {
            // Arrange
            int currencyId = 1;
            _repositoryMock.Setup(r => r.DeleteAsync(currencyId)).ReturnsAsync(true);
            var command = new DeleteCurrencyCommand(currencyId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.DeleteAsync(currencyId), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFalse_WhenRepositoryDeleteReturnsFalse()
        {
            // Arrange
            int currencyId = 2;
            _repositoryMock.Setup(r => r.DeleteAsync(currencyId)).ReturnsAsync(false);
            var command = new DeleteCurrencyCommand(currencyId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _repositoryMock.Verify(r => r.DeleteAsync(currencyId), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            int currencyId = 3;
            _repositoryMock.Setup(r => r.DeleteAsync(currencyId)).ThrowsAsync(new Exception("DB error"));
            var command = new DeleteCurrencyCommand(currencyId);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            _repositoryMock.Verify(r => r.DeleteAsync(currencyId), Times.Once);
        }

        [Fact]
        public async Task Handle_CallsRepositoryWithCorrectParameters()
        {
            // Arrange
            int currencyId = 4;
            _repositoryMock.Setup(r => r.DeleteAsync(currencyId)).ReturnsAsync(true);
            var command = new DeleteCurrencyCommand(currencyId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.DeleteAsync(currencyId), Times.Once);
        }
    }
}
