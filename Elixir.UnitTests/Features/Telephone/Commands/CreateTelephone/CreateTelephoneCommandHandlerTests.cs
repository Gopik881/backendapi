using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Telephone.Commands.CreateTelephone;
using Elixir.Application.Features.Telephone.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Moq;
using Xunit;

public class CreateTelephoneCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidInput_CreatesAllTelephoneCodesAndReturnsTrue()
    {
        // Arrange
        var telephoneDtos = new List<CreateUpdateTelephoneCodeDto>
        {
            new() { CountryId = 1, TelephoneCode = "123", Description = "Test1" },
            new() { CountryId = 2, TelephoneCode = "456", Description = "Test2" }
        };

        var repoMock = new Mock<ITelephoneCodeMasterRepository>();
        repoMock.Setup(r => r.CreateTelephoneCodeAsync(It.IsAny<TelephoneCodeMaster>()))
            .ReturnsAsync(1);

        var countryRepoMock = new Mock<ICountryMasterRepository>();

        var handler = new CreateTelephoneCommandHandler(countryRepoMock.Object, repoMock.Object);
        var command = new CreateTelephoneCommand(telephoneDtos);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        repoMock.Verify(r => r.CreateTelephoneCodeAsync(It.IsAny<TelephoneCodeMaster>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_EmptyInputList_ReturnsTrueAndDoesNotCallRepository()
    {
        // Arrange
        var repoMock = new Mock<ITelephoneCodeMasterRepository>();
        var countryRepoMock = new Mock<ICountryMasterRepository>();
        var handler = new CreateTelephoneCommandHandler(countryRepoMock.Object, repoMock.Object);
        var command = new CreateTelephoneCommand(new List<CreateUpdateTelephoneCodeDto>());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        repoMock.Verify(r => r.CreateTelephoneCodeAsync(It.IsAny<TelephoneCodeMaster>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagates()
    {
        // Arrange
        var telephoneDtos = new List<CreateUpdateTelephoneCodeDto>
        {
            new() { CountryId = 1, TelephoneCode = "123", Description = "Test1" }
        };

        var repoMock = new Mock<ITelephoneCodeMasterRepository>();
        repoMock.Setup(r => r.CreateTelephoneCodeAsync(It.IsAny<TelephoneCodeMaster>()))
            .ThrowsAsync(new Exception("DB error"));

        var countryRepoMock = new Mock<ICountryMasterRepository>();
        var handler = new CreateTelephoneCommandHandler(countryRepoMock.Object, repoMock.Object);
        var command = new CreateTelephoneCommand(telephoneDtos);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        repoMock.Verify(r => r.CreateTelephoneCodeAsync(It.IsAny<TelephoneCodeMaster>()), Times.Once);
    }

    //[Fact]
    //public async Task Handle_NullInputList_ThrowsArgumentNullException()
    //{
    //    // Arrange
    //    var repoMock = new Mock<ITelephoneCodeMasterRepository>();
    //    var countryRepoMock = new Mock<ICountryMasterRepository>();
    //    var handler = new CreateTelephoneCommandHandler(countryRepoMock.Object, repoMock.Object);
    //    var command = new CreateTelephoneCommand(null);

    //    // Act & Assert
    //    await Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(command, CancellationToken.None));
    //}
}