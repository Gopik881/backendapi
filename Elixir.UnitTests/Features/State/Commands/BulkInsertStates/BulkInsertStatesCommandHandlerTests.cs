using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Common.DTOs;
using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Features.State.Commands.BulkInsertStates;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Moq;
using Xunit;

public class BulkInsertStatesCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsFailed_WhenCountryNotFound()
    {
        var countryRepo = new Mock<ICountryMasterRepository>();
        var stateRepo = new Mock<IStateMasterRepository>();
        var errorRepo = new Mock<IBulkUploadErrorListRepository>();
        countryRepo.Setup(r => r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(false);

        var handler = new BulkInsertStatesCommandHandler(countryRepo.Object, stateRepo.Object, errorRepo.Object);
        var result = await handler.Handle(new BulkInsertStatesCommand(new List<StateBulkUploadDto>(), 1, "file.csv"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Failed, No such Country found.", result.AdditionalMessage);
    }

    // Additional tests can be written for:
    // - CountryId not provided, country name not found
    // - Duplicate state name/short name
    // - Successful insert
    // - Partial success (some errors, some inserted)
}