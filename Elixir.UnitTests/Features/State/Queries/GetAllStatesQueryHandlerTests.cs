using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Features.State.Queries.GetAllStates;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.State.Queries
{
    public class GetAllStatesQueryHandlerTests
    {
        private readonly Mock<IStateMasterRepository> _repositoryMock;
        private readonly GetAllStatesQueryHandler _handler;

        public GetAllStatesQueryHandlerTests()
        {
            _repositoryMock = new Mock<IStateMasterRepository>();
            _handler = new GetAllStatesQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsListOfStates_WhenRepositoryReturnsData()
        {
            // Arrange
            var states = new List<StateDto>
            {
                new StateDto { StateId = 1, StateName = "State1" },
                new StateDto { StateId = 2, StateName = "State2" }
            };
            _repositoryMock
                .Setup(r => r.GetAllStatesAsync())
                .ReturnsAsync(states);

            var query = new GetAllStatesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<StateDto>)result).Count);
            Assert.Equal("State1", ((List<StateDto>)result)[0].StateName);
            _repositoryMock.Verify(r => r.GetAllStatesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyList_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllStatesAsync())
                .ReturnsAsync(new List<StateDto>());

            var query = new GetAllStatesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _repositoryMock.Verify(r => r.GetAllStatesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllStatesAsync())
                .ThrowsAsync(new Exception("Repository error"));

            var query = new GetAllStatesQuery();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            _repositoryMock.Verify(r => r.GetAllStatesAsync(), Times.Once);
        }
    }
}