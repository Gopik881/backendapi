using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Currency.DTOs;
using Elixir.Application.Features.Currency.Queries.GetAllCurrency;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.Currency.Queries
{
    public class GetAllCurrencyQueryHandlerTests
    {
        private readonly Mock<ICurrencyMasterRepository> _repositoryMock;
        private readonly GetAllCurrencyQueryHandler _handler;

        public GetAllCurrencyQueryHandlerTests()
        {
            _repositoryMock = new Mock<ICurrencyMasterRepository>();
            _handler = new GetAllCurrencyQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsAllCurrencies_WhenRepositoryReturnsData()
        {
            // Arrange
            var dtos = new List<CurrencyDto>
            {
                new CurrencyDto { CurrencyId = 1, CurrencyName = "USD" },
                new CurrencyDto { CurrencyId = 2, CurrencyName = "EUR" }
            };
            _repositoryMock
                .Setup(r => r.GetAllCurrenciesAsync())
                .ReturnsAsync(dtos);

            var query = new GetAllCurrencyQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<CurrencyDto>)result).Count);
            _repositoryMock.Verify(r => r.GetAllCurrenciesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyList_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            var dtos = new List<CurrencyDto>();
            _repositoryMock
                .Setup(r => r.GetAllCurrenciesAsync())
                .ReturnsAsync(dtos);

            var query = new GetAllCurrencyQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _repositoryMock.Verify(r => r.GetAllCurrenciesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllCurrenciesAsync())
                .ThrowsAsync(new Exception("DB error"));

            var query = new GetAllCurrencyQuery();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            _repositoryMock.Verify(r => r.GetAllCurrenciesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_CallsRepositoryExactlyOnce()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllCurrenciesAsync())
                .ReturnsAsync(new List<CurrencyDto>());

            var query = new GetAllCurrencyQuery();

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.GetAllCurrenciesAsync(), Times.Once);
        }
    }
}