using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Telephone.DTOs;
using Elixir.Application.Features.Telephone.Queries.GetAllTelephoneCodes;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.Telephone.Queries
{
    public class GetAllTelephoneCodesQueryHandlerTests
    {
        private readonly Mock<ITelephoneCodeMasterRepository> _repositoryMock;
        private readonly GetAllTelephoneCodesQueryHandler _handler;

        public GetAllTelephoneCodesQueryHandlerTests()
        {
            _repositoryMock = new Mock<ITelephoneCodeMasterRepository>();
            _handler = new GetAllTelephoneCodesQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsList_WhenRepositoryReturnsData()
        {
            // Arrange
            var dtos = new List<TelephoneCodeMasterDto>
            {
                new TelephoneCodeMasterDto { /* set properties as needed */ }
            };
            _repositoryMock
                .Setup(r => r.GetAllTelephoneCodesAsync())
                .ReturnsAsync(dtos);

            var query = new GetAllTelephoneCodesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            _repositoryMock.Verify(r => r.GetAllTelephoneCodesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyList_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            var dtos = new List<TelephoneCodeMasterDto>();
            _repositoryMock
                .Setup(r => r.GetAllTelephoneCodesAsync())
                .ReturnsAsync(dtos);

            var query = new GetAllTelephoneCodesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _repositoryMock.Verify(r => r.GetAllTelephoneCodesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllTelephoneCodesAsync())
                .ThrowsAsync(new Exception("Repository error"));

            var query = new GetAllTelephoneCodesQuery();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            _repositoryMock.Verify(r => r.GetAllTelephoneCodesAsync(), Times.Once);
        }
    }
}