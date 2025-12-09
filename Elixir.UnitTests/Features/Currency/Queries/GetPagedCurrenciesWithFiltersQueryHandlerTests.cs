using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Currency.DTOs;
using Elixir.Application.Features.Currency.Queries.GetPagedCurrenciesWithFilters;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.Currency.Queries
{
    public class GetPagedCurrenciesWithFiltersQueryHandlerTests
    {
        private readonly Mock<ICurrencyMasterRepository> _repositoryMock;
        private readonly GetPagedCurrenciesWithFiltersQueryHandler _handler;

        public GetPagedCurrenciesWithFiltersQueryHandlerTests()
        {
            _repositoryMock = new Mock<ICurrencyMasterRepository>();
            _handler = new GetPagedCurrenciesWithFiltersQueryHandler(_repositoryMock.Object);
        }

        //[Fact]
        //public async Task Handle_ReturnsPaginatedResponse_WhenRepositoryReturnsData()
        //{
        //    // Arrange
        //    var dtos = new List<CurrencyDto>
        //    {
        //        new CurrencyDto { CurrencyId = 1, CurrencyName = "USD" },
        //        new CurrencyDto { CurrencyId = 2, CurrencyName = "EUR" }
        //    };
        //    int totalCount = 2;
        //    _repositoryMock
        //        .Setup(r => r.GetFilteredCurrenciesAsync("test", 1, 10))
        //        .ReturnsAsync(new Tuple<List<CurrencyDto>, int>(dtos, totalCount));

        //    var query = new GetPagedCurrenciesWithFiltersQuery("test", 1, 10);

        //    // Act
        //    var result = await _handler.Handle(query, CancellationToken.None);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(2, result.Data.Count);
        //    Assert.Equal(totalCount, result.Pagination.TotalCount);
        //    Assert.Equal(1, result.Pagination.PageNumber);
        //    Assert.Equal(10, result.Pagination.PageSize);
        //    _repositoryMock.Verify(r => r.GetFilteredCurrenciesAsync("test", 1, 10), Times.Once);
        //}

        //[Fact]
        //public async Task Handle_ReturnsEmptyPaginatedResponse_WhenRepositoryReturnsEmpty()
        //{
        //    // Arrange
        //    var dtos = new List<CurrencyDto>();
        //    int totalCount = 0;
        //    _repositoryMock
        //        .Setup(r => r.GetFilteredCurrenciesAsync(null, 2, 5))
        //        .ReturnsAsync(new Tuple<List<CurrencyDto>, int>(dtos, totalCount));

        //    var query = new GetPagedCurrenciesWithFiltersQuery(null, 2, 5);

        //    // Act
        //    var result = await _handler.Handle(query, CancellationToken.None);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Empty(result.Data);
        //    Assert.Equal(0, result.Pagination.TotalCount);
        //    Assert.Equal(2, result.Pagination.PageNumber);
        //    Assert.Equal(5, result.Pagination.PageSize);
        //    _repositoryMock.Verify(r => r.GetFilteredCurrenciesAsync(null, 2, 5), Times.Once);
        //}

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetFilteredCurrenciesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("DB error"));

            var query = new GetPagedCurrenciesWithFiltersQuery("fail", 1, 10);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            _repositoryMock.Verify(r => r.GetFilteredCurrenciesAsync("fail", 1, 10), Times.Once);
        }

        //[Fact]
        //public async Task Handle_CorrectlyMapsPaginationMetadata()
        //{
        //    // Arrange
        //    var dtos = new List<CurrencyDto> { new CurrencyDto { CurrencyId = 1, CurrencyName = "INR" } };
        //    int totalCount = 15;
        //    int pageNumber = 2;
        //    int pageSize = 5;
        //    _repositoryMock
        //        .Setup(r => r.GetFilteredCurrenciesAsync("INR", pageNumber, pageSize))
        //        .ReturnsAsync(new Tuple<List<CurrencyDto>, int>(dtos, totalCount));

        //    var query = new GetPagedCurrenciesWithFiltersQuery("INR", pageNumber, pageSize);

        //    // Act
        //    var result = await _handler.Handle(query, CancellationToken.None);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Single(result.Data);
        //    Assert.Equal(totalCount, result.Pagination.TotalCount);
        //    Assert.Equal(pageNumber, result.Pagination.PageNumber);
        //    Assert.Equal(pageSize, result.Pagination.PageSize);
        //}

        [Fact]
        public async Task Handle_CallsRepositoryWithCorrectParameters()
        {
            // Arrange
            var dtos = new List<CurrencyDto>();
            int totalCount = 0;
            string searchTerm = "JPY";
            int pageNumber = 3;
            int pageSize = 7;
            _repositoryMock
                .Setup(r => r.GetFilteredCurrenciesAsync(searchTerm, pageNumber, pageSize))
                .ReturnsAsync(new Tuple<List<CurrencyDto>, int>(dtos, totalCount));

            var query = new GetPagedCurrenciesWithFiltersQuery(searchTerm, pageNumber, pageSize);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.GetFilteredCurrenciesAsync(searchTerm, pageNumber, pageSize), Times.Once);
        }
    }
}