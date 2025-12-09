using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Common.DTOs;
using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Features.Telephone.Commands.BulkInsertTelephoneCodes;
using Elixir.Application.Features.Telephone.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Moq;
using Xunit;

public class BulkInsertTelephoneCodesCommandHandlerTests
{
    [Fact]
    public async Task Handle_AllValid_InsertsAndReturnsSuccess()
    {
        var countryRepo = new Mock<ICountryMasterRepository>();
        var telRepo = new Mock<ITelephoneCodeMasterRepository>();
        var errorRepo = new Mock<IBulkUploadErrorListRepository>();

        countryRepo.Setup(r => r.GetAllCountriesAsync()).ReturnsAsync(new List<CountryDto> { new CountryDto { CountryId = 1, CountryName = "A" } });
        telRepo.Setup(r => r.GetAllTelephoneCodesAsync()).ReturnsAsync(new List<TelephoneCodeMasterDto>());
        telRepo.Setup(r => r.BulkInsertTelephoneCodesAsync(It.IsAny<List<TelephoneCodeBulkUploadDto>>())).ReturnsAsync(true);

        var handler = new BulkInsertTelephoneCodesCommandHandler(countryRepo.Object, telRepo.Object, errorRepo.Object);
        var dtos = new List<TelephoneCodeBulkUploadDto> { new TelephoneCodeBulkUploadDto { CountryName = "A", TelephoneCode = "123", RowId = 1 } };
        var command = new BulkInsertTelephoneCodesCommand(dtos, "file.csv");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess || result.IsPartialSuccess);
    }

    [Fact]
    public async Task Handle_DuplicateTelephoneCode_AddsError()
    {
        var countryRepo = new Mock<ICountryMasterRepository>();
        var telRepo = new Mock<ITelephoneCodeMasterRepository>();
        var errorRepo = new Mock<IBulkUploadErrorListRepository>();

        countryRepo.Setup(r => r.GetAllCountriesAsync()).ReturnsAsync(new List<CountryDto> { new CountryDto { CountryId = 1, CountryName = "A" } });

        telRepo.Setup(r => r.GetAllTelephoneCodesAsync())
            .ReturnsAsync(new List<TelephoneCodeMasterDto>
            {
                new TelephoneCodeMasterDto { CountryId = 1, TelephoneCode = "123" }
            });

        var handler = new BulkInsertTelephoneCodesCommandHandler(countryRepo.Object, telRepo.Object, errorRepo.Object);
        var dtos = new List<TelephoneCodeBulkUploadDto> { new TelephoneCodeBulkUploadDto { CountryName = "A", TelephoneCode = "123", RowId = 1 } };
        var command = new BulkInsertTelephoneCodesCommand(dtos, "file.csv");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        errorRepo.Verify(r => r.BulkInsertBulkUploadErrorListAsync(It.IsAny<List<BulkUploadErrorList>>()), Times.Once);
    }
}