using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Common.DTOs;
using Elixir.Application.Features.Currency.Commands.BulkInsertCurrencies;
using Elixir.Application.Features.Currency.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.Currency.Commands
{
    public class BulkInsertCurrencyCommandHandlerTests
    {
        private readonly Mock<ICountryMasterRepository> _countryRepoMock;
        private readonly Mock<ICurrencyMasterRepository> _currencyRepoMock;
        private readonly Mock<IBulkUploadErrorListRepository> _errorRepoMock;
        private readonly BulkInsertCurrencyCommandHandler _handler;

        public BulkInsertCurrencyCommandHandlerTests()
        {
            _countryRepoMock = new Mock<ICountryMasterRepository>();
            _currencyRepoMock = new Mock<ICurrencyMasterRepository>();
            _errorRepoMock = new Mock<IBulkUploadErrorListRepository>();
            _handler = new BulkInsertCurrencyCommandHandler(
                _countryRepoMock.Object,
                _currencyRepoMock.Object,
                _errorRepoMock.Object
            );
        }

        //[Fact]
        //public async Task Handle_SuccessfulBulkInsert_ReturnsSuccessStatus()
        //{
        //    // Arrange
        //    var countries = new List<CountryMaster>
        //    {
        //        new CountryMaster { CountryId = 1, CountryName = "USA" }
        //    };
        //    var existingCurrencies = new List<CurrencyMaster>();
        //    var dtos = new List<CurrencyBulkUploadDto>
        //    {
        //        new CurrencyBulkUploadDto { CountryName = "USA", CurrencyName = "Dollar", CurrencyShortName = "USD" }
        //    };
        //    _countryRepoMock.Setup(r => r.GetAllCountriesAsync()).ReturnsAsync(countries);
        //    _currencyRepoMock.Setup(r => r.GetAllCurrenciesAsync()).ReturnsAsync(existingCurrencies);

        //    var command = new BulkInsertCurrencyCommand(dtos, "file.csv");

        //    // Act
        //    var result = await _handler.Handle(command, CancellationToken.None);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal("file.csv", result.FileName);
        //    Assert.NotEqual(Guid.Empty, result.ProcessId);
        //    Assert.True(result.ProcessedAt <= DateTime.UtcNow);
        //}

        //[Fact]
        //public async Task Handle_DuplicateCurrencyName_AddsToErrorList()
        //{
        //    // Arrange
        //    var countries = new List<CountryMaster>
        //    {
        //        new CountryMaster { CountryId = 1, CountryName = "USA" }
        //    };
        //    var existingCurrencies = new List<CurrencyMaster>
        //    {
        //        new CurrencyMaster { CountryId = 1, CurrencyName = "Dollar", CurrencyShortName = "USD" }
        //    };
        //    var dtos = new List<CurrencyBulkUploadDto>
        //    {
        //        new CurrencyBulkUploadDto { CountryName = "USA", CurrencyName = "Dollar", CurrencyShortName = "USD" }
        //    };
        //    _countryRepoMock.Setup(r => r.GetAllCountriesAsync()).ReturnsAsync(countries);
        //    _currencyRepoMock.Setup(r => r.GetAllCurrenciesAsync()).ReturnsAsync(existingCurrencies);

        //    var command = new BulkInsertCurrencyCommand(dtos, "file.csv");

        //    // Act
        //    var result = await _handler.Handle(command, CancellationToken.None);

        //    // Assert
        //    Assert.NotNull(result);
        //    // Should have error for duplicate currency name
        //    // (You may need to expose error list in BulkUploadStatusDto for more granular asserts)
        //}

        //[Fact]
        //public async Task Handle_InvalidCountryName_AddsToErrorList()
        //{
        //    // Arrange
        //    var countries = new List<CountryMaster>
        //    {
        //        new CountryMaster { CountryId = 1, CountryName = "USA" }
        //    };
        //    var existingCurrencies = new List<CurrencyMaster>();
        //    var dtos = new List<CurrencyBulkUploadDto>
        //    {
        //        new CurrencyBulkUploadDto { CountryName = "Unknown", CurrencyName = "Euro", CurrencyShortName = "EUR" }
        //    };
        //    _countryRepoMock.Setup(r => r.GetAllCountriesAsync()).ReturnsAsync(countries);
        //    _currencyRepoMock.Setup(r => r.GetAllCurrenciesAsync()).ReturnsAsync(existingCurrencies);

        //    var command = new BulkInsertCurrencyCommand(dtos, "file.csv");

        //    // Act
        //    var result = await _handler.Handle(command, CancellationToken.None);

        //    // Assert
        //    Assert.NotNull(result);
        //    // Should have error for invalid country name
        //}

        [Fact]
        public async Task Handle_RepositoryThrows_ExceptionPropagates()
        {
            // Arrange
            _currencyRepoMock.Setup(r => r.GetAllCurrenciesAsync()).ThrowsAsync(new Exception("DB error"));
            var command = new BulkInsertCurrencyCommand(new List<CurrencyBulkUploadDto>(), "file.csv");

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }

        //[Fact]
        //public async Task Handle_EmptyCurrencyList_ReturnsStatusWithNoInsert()
        //{
        //    // Arrange
        //    var countries = new List<CountryMaster>();
        //    var existingCurrencies = new List<CurrencyMaster>();
        //    _countryRepoMock.Setup(r => r.GetAllCountriesAsync()).ReturnsAsync(countries);
        //    _currencyRepoMock.Setup(r => r.GetAllCurrenciesAsync()).ReturnsAsync(existingCurrencies);

        //    var command = new BulkInsertCurrencyCommand(new List<CurrencyBulkUploadDto>(), "file.csv");

        //    // Act
        //    var result = await _handler.Handle(command, CancellationToken.None);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal("file.csv", result.FileName);
        //}

        //[Fact]
        //public async Task Handle_MultipleRecords_MixedValidAndInvalid()
        //{
        //    // Arrange
        //    var countries = new List<CountryMaster>
        //    {
        //        new CountryMaster { CountryId = 1, CountryName = "USA" }
        //    };
        //    var existingCurrencies = new List<CurrencyMaster>
        //    {
        //        new CurrencyMaster { CountryId = 1, CurrencyName = "Dollar", CurrencyShortName = "USD" }
        //    };
        //    var dtos = new List<CurrencyBulkUploadDto>
        //    {
        //        new CurrencyBulkUploadDto { CountryName = "USA", CurrencyName = "Dollar", CurrencyShortName = "USD" }, // duplicate
        //        new CurrencyBulkUploadDto { CountryName = "USA", CurrencyName = "Peso", CurrencyShortName = "MXN" }, // valid
        //        new CurrencyBulkUploadDto { CountryName = "Unknown", CurrencyName = "Euro", CurrencyShortName = "EUR" } // invalid country
        //    };
        //    _countryRepoMock.Setup(r => r.GetAllCountriesAsync()).ReturnsAsync(countries);
        //    _currencyRepoMock.Setup(r => r.GetAllCurrenciesAsync()).ReturnsAsync(existingCurrencies);

        //    var command = new BulkInsertCurrencyCommand(dtos, "file.csv");

        //    // Act
        //    var result = await _handler.Handle(command, CancellationToken.None);

        //    // Assert
        //    Assert.NotNull(result);
        //    // Should process valid and add errors for invalid/duplicate
        //}
    }
}