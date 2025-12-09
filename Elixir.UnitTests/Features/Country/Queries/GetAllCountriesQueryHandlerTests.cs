using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Features.Country.Queries.GetAllCountries;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.Country.Queries
{
    public class GetAllCountriesQueryHandlerTests
    {
        private readonly Mock<ICountryMasterRepository> _repoMock;
        private readonly GetAllCountriesQueryHandler _handler;

        public GetAllCountriesQueryHandlerTests()
        {
            _repoMock = new Mock<ICountryMasterRepository>();
            _handler = new GetAllCountriesQueryHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsAllCountries_WhenRepositoryReturnsData()
        {
            // Arrange
            var countries = new List<CountryDto>
            {
                new CountryDto { CountryId = 1, CountryName = "A" },
                new CountryDto { CountryId = 2, CountryName = "B" }
            };
            _repoMock.Setup(r => r.GetAllCountriesAsync()).ReturnsAsync(countries);

            var query = new GetAllCountriesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<CountryDto>)result).Count);
            Assert.Equal("A", ((List<CountryDto>)result)[0].CountryName);
            _repoMock.Verify(r => r.GetAllCountriesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyList_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            _repoMock.Setup(r => r.GetAllCountriesAsync()).ReturnsAsync(new List<CountryDto>());

            var query = new GetAllCountriesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _repoMock.Verify(r => r.GetAllCountriesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_RepositoryThrows_ExceptionPropagates()
        {
            // Arrange
            _repoMock.Setup(r => r.GetAllCountriesAsync()).ThrowsAsync(new Exception("DB error"));

            var query = new GetAllCountriesQuery();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            _repoMock.Verify(r => r.GetAllCountriesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_CallsRepositoryMethod()
        {
            // Arrange
            _repoMock.Setup(r => r.GetAllCountriesAsync()).ReturnsAsync(new List<CountryDto>());

            var query = new GetAllCountriesQuery();

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repoMock.Verify(r => r.GetAllCountriesAsync(), Times.Once);
        }
    }
}