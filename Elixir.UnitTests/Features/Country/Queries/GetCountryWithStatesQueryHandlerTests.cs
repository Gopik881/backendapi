using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Features.State.Queries.GetStatesbyCountryId;
using Elixir.Application.Interfaces.Persistance;
using Moq;

namespace Elixir.UnitTests.Features.Country.Queries
{
    public class GetCountryWithStatesQueryHandlerTests
    {
        private readonly Mock<IStateMasterRepository> _stateRepoMock;
        private readonly GetStatesByCountryIdQueryHandler _handler;

        public GetCountryWithStatesQueryHandlerTests()
        {
            _stateRepoMock = new Mock<IStateMasterRepository>();
            _handler = new GetStatesByCountryIdQueryHandler(_stateRepoMock.Object);
        }

        //[Fact]
        //public async Task Handle_ReturnsCountryWithStates_WhenRepositoryReturnsData()
        //{
        //    // Arrange
        //    var countryWithStates = new CountryWithStatesDto
        //    {
        //        CountryId = 1,
        //        CountryName = "USA",
        //        States = new System.Collections.Generic.List<Elixir.Application.Features.State.DTOs.StateDto>
        //        {
        //            new Elixir.Application.Features.State.DTOs.StateDto { StateId = 1, StateName = "California" },
        //            new Elixir.Application.Features.State.DTOs.StateDto { StateId = 2, StateName = "Texas" }
        //        }
        //    };
        //    _stateRepoMock
        //        .Setup(r => r.GetCountryByIdWithStatesAsync(1))
        //        .ReturnsAsync(countryWithStates);

        //    var query = new GetStatesByCountryIdQuery(1);

        //    // Act
        //    var result = await _handler.Handle(query, CancellationToken.None);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(1, result.CountryId);
        //    Assert.Equal("USA", result.CountryName);
        //    Assert.Equal(2, result.States.Count);
        //    _stateRepoMock.Verify(r => r.GetCountryByIdWithStatesAsync(1), Times.Once);
        //}

        [Fact]
        public async Task Handle_ReturnsNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            _stateRepoMock
                .Setup(r => r.GetCountryByIdWithStatesAsync(99))
                .ReturnsAsync((CountryWithStatesDto)null);

            var query = new GetStatesByCountryIdQuery(99);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _stateRepoMock.Verify(r => r.GetCountryByIdWithStatesAsync(99), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            _stateRepoMock
                .Setup(r => r.GetCountryByIdWithStatesAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("DB error"));

            var query = new GetStatesByCountryIdQuery(1);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            _stateRepoMock.Verify(r => r.GetCountryByIdWithStatesAsync(1), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Handle_HandlesEdgeCases_CountryId(int countryId)
        {
            // Arrange
            _stateRepoMock
                .Setup(r => r.GetCountryByIdWithStatesAsync(countryId))
                .ReturnsAsync((CountryWithStatesDto)null);

            var query = new GetStatesByCountryIdQuery(countryId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _stateRepoMock.Verify(r => r.GetCountryByIdWithStatesAsync(countryId), Times.Once);
        }

        [Fact]
        public async Task Handle_CallsRepositoryWithCorrectParameter()
        {
            // Arrange
            int countryId = 5;
            _stateRepoMock
                .Setup(r => r.GetCountryByIdWithStatesAsync(countryId))
                .ReturnsAsync((CountryWithStatesDto)null);

            var query = new GetStatesByCountryIdQuery(countryId);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _stateRepoMock.Verify(r => r.GetCountryByIdWithStatesAsync(countryId), Times.Once);
        }
    }
}