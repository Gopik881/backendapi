using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.State.Commands.CreateState;
using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.State.Commands
{
    public class CreateStateCommandHandlerTests
    {
        private readonly Mock<IStateMasterRepository> _stateRepoMock;
        private readonly Mock<ICountryMasterRepository> _countryRepoMock;
        private readonly CreateStateCommandHandler _handler;

        public CreateStateCommandHandlerTests()
        {
            _stateRepoMock = new Mock<IStateMasterRepository>();
            _countryRepoMock = new Mock<ICountryMasterRepository>();
            _handler = new CreateStateCommandHandler(_stateRepoMock.Object, _countryRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsFalse_WhenAnyDuplicateStatesExists()
        {
            // Arrange
            var dtos = new List<CreateUpdateStateDto>
            {
                new CreateUpdateStateDto { CountryId = 1, StateName = "A" }
            };
            _stateRepoMock.Setup(r => r.AnyDuplicateStatesExistsAsync(dtos)).ReturnsAsync(true);

            var command = new CreateStateCommand(dtos);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _stateRepoMock.Verify(r => r.AnyDuplicateStatesExistsAsync(dtos), Times.Once);
            _stateRepoMock.Verify(r => r.CreateStateAsync(It.IsAny<StateMaster>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ReturnsTrue_AndCreatesAllStates_WhenNoDuplicates()
        {
            // Arrange
            var dtos = new List<CreateUpdateStateDto>
            {
                new CreateUpdateStateDto { CountryId = 1, StateName = "A", StateShortName = "AA", Description = "desc1" },
                new CreateUpdateStateDto { CountryId = 2, StateName = "B", StateShortName = "BB", Description = "desc2" }
            };
            _stateRepoMock.Setup(r => r.AnyDuplicateStatesExistsAsync(dtos)).ReturnsAsync(false);
            _stateRepoMock.Setup(r => r.CreateStateAsync(It.IsAny<StateMaster>())).ReturnsAsync(1);

            var command = new CreateStateCommand(dtos);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _stateRepoMock.Verify(r => r.AnyDuplicateStatesExistsAsync(dtos), Times.Once);
            _stateRepoMock.Verify(r => r.CreateStateAsync(It.IsAny<StateMaster>()), Times.Exactly(dtos.Count));
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenAnyDuplicateStatesExistsThrows()
        {
            // Arrange
            var dtos = new List<CreateUpdateStateDto>
            {
                new CreateUpdateStateDto { CountryId = 1, StateName = "A" }
            };
            _stateRepoMock.Setup(r => r.AnyDuplicateStatesExistsAsync(dtos)).ThrowsAsync(new Exception("Dup check error"));

            var command = new CreateStateCommand(dtos);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            _stateRepoMock.Verify(r => r.AnyDuplicateStatesExistsAsync(dtos), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenCreateStateAsyncThrows()
        {
            // Arrange
            var dtos = new List<CreateUpdateStateDto>
            {
                new CreateUpdateStateDto { CountryId = 1, StateName = "A" }
            };
            _stateRepoMock.Setup(r => r.AnyDuplicateStatesExistsAsync(dtos)).ReturnsAsync(false);
            _stateRepoMock.Setup(r => r.CreateStateAsync(It.IsAny<StateMaster>())).ThrowsAsync(new Exception("Create error"));

            var command = new CreateStateCommand(dtos);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            _stateRepoMock.Verify(r => r.AnyDuplicateStatesExistsAsync(dtos), Times.Once);
            _stateRepoMock.Verify(r => r.CreateStateAsync(It.IsAny<StateMaster>()), Times.Once);
        }
    }
}