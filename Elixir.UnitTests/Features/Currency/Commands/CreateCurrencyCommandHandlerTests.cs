using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Currency.Commands.CreateCurrency;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Currency.DTOs;
using Elixir.Domain.Entities;
using System.Collections.Generic;

public class CreateCurrencyCommandHandlerTests
{
    [Fact]
    public async Task Handle_DuplicateCurrencies_ReturnsTrueWithoutInsert()
    {
        var mockCurrencyRepo = new Mock<ICurrencyMasterRepository>();
        var mockCountryRepo = new Mock<ICountryMasterRepository>();
        mockCurrencyRepo.Setup(r => r.AnyDuplicateCurrenciesExistsAsync(It.IsAny<List<CreateUpdateCurrencyDto>>()))
            .ReturnsAsync(true);

        var handler = new CreateCurrencyCommandHandler(mockCountryRepo.Object, mockCurrencyRepo.Object);
        var command = new CreateCurrencyCommand(new List<CreateUpdateCurrencyDto>
        {
            new CreateUpdateCurrencyDto { CountryId = 1, CurrencyName = "USD", CurrencyShortName = "US", Description = "Dollar" }
        });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        mockCurrencyRepo.Verify(r => r.CreateCurrencyAsync(It.IsAny<CurrencyMaster>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidCurrencies_CreatesCurrency()
    {
        var mockCurrencyRepo = new Mock<ICurrencyMasterRepository>();
        var mockCountryRepo = new Mock<ICountryMasterRepository>();
        mockCurrencyRepo.Setup(r => r.AnyDuplicateCurrenciesExistsAsync(It.IsAny<List<CreateUpdateCurrencyDto>>()))
            .ReturnsAsync(false);

        var handler = new CreateCurrencyCommandHandler(mockCountryRepo.Object, mockCurrencyRepo.Object);
        var command = new CreateCurrencyCommand(new List<CreateUpdateCurrencyDto>
        {
            new CreateUpdateCurrencyDto { CountryId = 1, CurrencyName = "USD", CurrencyShortName = "US", Description = "Dollar" }
        });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        mockCurrencyRepo.Verify(r => r.CreateCurrencyAsync(It.IsAny<CurrencyMaster>()), Times.Once);
    }
}