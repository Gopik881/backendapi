using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Features.Country.Queries.GetPagedCountriesWithFilters;
using Elixir.Application.Interfaces.Persistance;
using Moq;

namespace Elixir.UnitTests.Features.Country.Queries
{
    public class GetPagedCountriesWithFiltersQueryHandlerTests
    {
        private readonly Mock<ICountryMasterRepository> _repositoryMock;
        private readonly GetPagedCountriesWithFiltersQueryHandler _handler;

        public GetPagedCountriesWithFiltersQueryHandlerTests()
        {
            _repositoryMock = new Mock<ICountryMasterRepository>();
            _handler = new GetPagedCountriesWithFiltersQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsPaginatedResponse_WhenRepositoryReturnsData()
        {
            // Arrange
            var dtos = new List<CountryDto>
            {
                new CountryDto { CountryId = 1, CountryName = "USA" },
                new CountryDto { CountryId = 2, CountryName = "Canada" }
            };
            int totalCount = 2;
            _repositoryMock
                .Setup(r => r.GetFilteredCountriesAsync("test", 1, 10))
                .ReturnsAsync(new Tuple<List<CountryDto>, int>(dtos, totalCount));

            var query = new GetPagedCountriesWithFiltersQuery("test", 1, 10);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Data.Count);
            //Assert.Equal(totalCount, result.Pagination.TotalCount);
            //Assert.Equal(1, result.Pagination.PageNumber);
            //Assert.Equal(10, result.Pagination.PageSize);
            _repositoryMock.Verify(r => r.GetFilteredCountriesAsync("test", 1, 10), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyPaginatedResponse_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            var dtos = new List<CountryDto>();
            int totalCount = 0;
            _repositoryMock
                .Setup(r => r.GetFilteredCountriesAsync(null, 2, 5))
                .ReturnsAsync(new Tuple<List<CountryDto>, int>(dtos, totalCount));

            var query = new GetPagedCountriesWithFiltersQuery(null, 2, 5);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Data);
            //Assert.Equal(0, result.Pagination.TotalCount);
            //Assert.Equal(2, result.Pagination.PageNumber);
            //Assert.Equal(5, result.Pagination.PageSize);
            _repositoryMock.Verify(r => r.GetFilteredCountriesAsync(null, 2, 5), Times.Once);
        }

        [Fact]
        public async Task Handle_HandlesNullSearchTerm()
        {
            // Arrange
            var dtos = new List<CountryDto>
            {
                new CountryDto { CountryId = 1, CountryName = "India" }
            };
            int totalCount = 1;
            _repositoryMock
                .Setup(r => r.GetFilteredCountriesAsync(null, 1, 10))
                .ReturnsAsync(new Tuple<List<CountryDto>, int>(dtos, totalCount));

            var query = new GetPagedCountriesWithFiltersQuery(null, 1, 10);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Data);
            Assert.Equal("India", result.Data[0].CountryName);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetFilteredCountriesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("DB error"));

            var query = new GetPagedCountriesWithFiltersQuery("test", 1, 10);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(-1, 10)]
        [InlineData(1, -5)]
        public async Task Handle_HandlesEdgeCases_PageNumberAndPageSize(int pageNumber, int pageSize)
        {
            // Arrange
            var dtos = new List<CountryDto>();
            int totalCount = 0;
            _repositoryMock
                .Setup(r => r.GetFilteredCountriesAsync(It.IsAny<string>(), pageNumber, pageSize))
                .ReturnsAsync(new Tuple<List<CountryDto>, int>(dtos, totalCount));

            var query = new GetPagedCountriesWithFiltersQuery("test", pageNumber, pageSize);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            //Assert.Equal(pageNumber, result.Pagination.PageNumber);
            //Assert.Equal(pageSize, result.Pagination.PageSize);
        }

        [Fact]
        public async Task Handle_CallsRepositoryWithCorrectParameters()
        {
            // Arrange
            var dtos = new List<CountryDto>();
            int totalCount = 0;
            string searchTerm = "abc";
            int pageNumber = 3;
            int pageSize = 15;
            _repositoryMock
                .Setup(r => r.GetFilteredCountriesAsync(searchTerm, pageNumber, pageSize))
                .ReturnsAsync(new Tuple<List<CountryDto>, int>(dtos, totalCount));

            var query = new GetPagedCountriesWithFiltersQuery(searchTerm, pageNumber, pageSize);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.GetFilteredCountriesAsync(searchTerm, pageNumber, pageSize), Times.Once);
        }
    }
}