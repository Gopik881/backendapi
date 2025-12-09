using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Elixir.Application.Features.Country.Commands.CreateCountry;
using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.Country.Commands
{
    public class CreateCountryCommandHandlerTests
    {
        private readonly Mock<ICountryMasterRepository> _repoMock;
        private readonly CreateCountryCommandHandler _handler;

        public CreateCountryCommandHandlerTests()
        {
            _repoMock = new Mock<ICountryMasterRepository>();
            _handler = new CreateCountryCommandHandler(_repoMock.Object);
        }

        //[Fact]
        //public async Task Handle_SuccessfulCreation_ReturnsTrue()
        //{
        //    // Arrange
        //    var dto = new CreateCountryDto { CountryName = "TestCountry" };
        //    _repoMock.Setup(r => r.CountryNameExistsAsync(dto.CountryName)).ReturnsAsync(false);
        //    _repoMock.Setup(r => r.CreateCountryAsync(It.IsAny<CountryMaster>())).ReturnsAsync(1);

        //    var command = new CreateCountryCommand(dto);

        //    // Act
        //    var result = await _handler.Handle(command, CancellationToken.None);

        //    // Assert
        //    Assert.True(result);
        //    _repoMock.Verify(r => r.CreateCountryAsync(It.IsAny<CountryMaster>()), Times.Once);
        //}

        //[Fact]
        //public async Task Handle_CountryNameExists_ReturnsFalse()
        //{
        //    // Arrange
        //    var dto = new CreateCountryDto { CountryName = "TestCountry" };
        //    _repoMock.Setup(r => r.CountryNameExistsAsync(dto.CountryName)).ReturnsAsync(true);

        //    var command = new CreateCountryCommand(dto);

        //    // Act
        //    var result = await _handler.Handle(command, CancellationToken.None);

        //    // Assert
        //    Assert.False(result);
        //    _repoMock.Verify(r => r.CreateCountryAsync(It.IsAny<CountryMaster>()), Times.Never);
        //}

        //[Fact]
        //public async Task Handle_RepositoryThrows_ExceptionPropagates()
        //{
        //    // Arrange
        //    var dto = new CreateCountryDto { CountryName = "TestCountry" };
        //    _repoMock.Setup(r => r.CountryNameExistsAsync(dto.CountryName)).ReturnsAsync(false);
        //    _repoMock.Setup(r => r.CreateCountryAsync(It.IsAny<CountryMaster>())).ThrowsAsync(new Exception("DB error"));

        //    var command = new CreateCountryCommand(dto);

        //    // Act & Assert
        //    await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        //}

        //[Fact]
        //public async Task Handle_NullInput_ThrowsArgumentNullException()
        //{
        //    // Arrange
        //    var command = new CreateCountryCommand(null);

        //    // Act & Assert
        //    await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(command, CancellationToken.None));
        //}
    }
}