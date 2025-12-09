using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Country.Commands.BulkInsert;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Common.DTOs;
using Elixir.Application.Features.Country.DTOs;
using Elixir.Domain.Entities;
using System.Collections.Generic;
using System;

public class BulkInsertCountriesCommandHandlerTests
{
    [Fact]
    public async Task Handle_AllValidCountries_InsertsSuccessfully()
    {
        var mockCountryRepo = new Mock<ICountryMasterRepository>();
        var mockErrorRepo = new Mock<IBulkUploadErrorListRepository>();
        mockCountryRepo.Setup(r => r.GetAllCountriesAsync()).ReturnsAsync(new List<CountryDto>()); // Fix: Adjusted type to match the method signature
        mockCountryRepo.Setup(r => r.BulkInsertCountriesAsync(It.IsAny<List<CountryBulkUploadDto>>())).ReturnsAsync(true);

        var handler = new BulkInsertCountriesCommandHandler(mockCountryRepo.Object, mockErrorRepo.Object);
        var command = new BulkInsertCountriesCommand(new List<CountryBulkUploadDto>
        {
            new CountryBulkUploadDto { CountryName = "A", CountryShortName = "AA", RowId = 1 }
        }, "file.csv");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsPartialSuccess);
    }

    [Fact]
    public async Task Handle_DuplicateCountryName_AddsError()
    {
        var mockCountryRepo = new Mock<ICountryMasterRepository>();
        var mockErrorRepo = new Mock<IBulkUploadErrorListRepository>();
        mockCountryRepo.Setup(r => r.GetAllCountriesAsync()).ReturnsAsync(new List<CountryDto>
        {
            new CountryDto { CountryName = "A", CountryShortName = "AA" } // Fix: Adjusted type to match the method signature
        });

        var handler = new BulkInsertCountriesCommandHandler(mockCountryRepo.Object, mockErrorRepo.Object);
        var command = new BulkInsertCountriesCommand(new List<CountryBulkUploadDto>
        {
            new CountryBulkUploadDto { CountryName = "A", CountryShortName = "BB", RowId = 1 }
        }, "file.csv");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.False(result.IsPartialSuccess);
    }

    [Fact]
    public async Task Handle_PartialSuccess_SetsPartialSuccessFlag()
    {
        var mockCountryRepo = new Mock<ICountryMasterRepository>();
        var mockErrorRepo = new Mock<IBulkUploadErrorListRepository>();
        mockCountryRepo.Setup(r => r.GetAllCountriesAsync()).ReturnsAsync(new List<CountryDto>
        {
            new CountryDto { CountryName = "A", CountryShortName = "AA" } // Fix: Adjusted type to match the method signature
        });
        mockCountryRepo.Setup(r => r.BulkInsertCountriesAsync(It.IsAny<List<CountryBulkUploadDto>>())).ReturnsAsync(true);

        var handler = new BulkInsertCountriesCommandHandler(mockCountryRepo.Object, mockErrorRepo.Object);
        var command = new BulkInsertCountriesCommand(new List<CountryBulkUploadDto>
        {
            new CountryBulkUploadDto { CountryName = "A", CountryShortName = "AA", RowId = 1 },
            new CountryBulkUploadDto { CountryName = "B", CountryShortName = "BB", RowId = 2 }
        }, "file.csv");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsPartialSuccess);
    }
}