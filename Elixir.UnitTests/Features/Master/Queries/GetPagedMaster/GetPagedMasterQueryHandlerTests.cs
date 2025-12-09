using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Master.DTOs;
using Elixir.Application.Features.Master.Queries.GetPagedMaster;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.Master.Queries.GetPagedMaster
{
    public class GetPagedMasterQueryHandlerTests
    {
        private readonly Mock<IMasterRepository> _repositoryMock;
        private readonly GetPagedMasterQueryHandler _handler;

        public GetPagedMasterQueryHandlerTests()
        {
            _repositoryMock = new Mock<IMasterRepository>();
            _handler = new GetPagedMasterQueryHandler(_repositoryMock.Object);
        }

        //[Fact]
        //public async Task Handle_ReturnsPaginatedResponse_WhenRepositoryReturnsData()
        //{
        //    // Arrange
        //    var dtos = new List<MasterDto>
        //    {
        //        new MasterDto(), new MasterDto()
        //    };
        //    int totalCount = 2;
        //    _repositoryMock
        //        .Setup(r => r.GetFilteredMasterAsync("test", 1, 10))
        //        .ReturnsAsync((dtos, totalCount));

        //    var query = new GetPagedMasterQuery("test", 1, 10);

        //    // Act
        //    var result = await _handler.Handle(query, CancellationToken.None);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(2, result.Data.Count);
        //    Assert.Equal(totalCount, result.Pagination.TotalCount);
        //    Assert.Equal(1, result.Pagination.PageNumber);
        //    Assert.Equal(10, result.Pagination.PageSize);
        //    _repositoryMock.Verify(r => r.GetFilteredMasterAsync("test", 1, 10), Times.Once);
        //}

        //[Fact]
        //public async Task Handle_ReturnsEmptyPaginatedResponse_WhenRepositoryReturnsEmpty()
        //{
        //    // Arrange
        //    var dtos = new List<MasterDto>();
        //    int totalCount = 0;
        //    _repositoryMock
        //        .Setup(r => r.GetFilteredMasterAsync(null, 2, 5))
        //        .ReturnsAsync((dtos, totalCount));

        //    var query = new GetPagedMasterQuery(null, 2, 5);

        //    // Act
        //    var result = await _handler.Handle(query, CancellationToken.None);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Empty(result.Data);
        //    Assert.Equal(0, result.Pagination.TotalCount);
        //    Assert.Equal(2, result.Pagination.PageNumber);
        //    Assert.Equal(5, result.Pagination.PageSize);
        //    _repositoryMock.Verify(r => r.GetFilteredMasterAsync(null, 2, 5), Times.Once);
        //}

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetFilteredMasterAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Repository error"));

            var query = new GetPagedMasterQuery("fail", 1, 1);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            _repositoryMock.Verify(r => r.GetFilteredMasterAsync("fail", 1, 1), Times.Once);
        }

        //[Fact]
        //public async Task Handle_CallsRepositoryWithCorrectParameters()
        //{
        //    // Arrange
        //    var dtos = new List<MasterDto> { new MasterDto() };
        //    _repositoryMock
        //        .Setup(r => r.GetFilteredMasterAsync("abc", 3, 7))
        //        .ReturnsAsync((dtos, 1));

        //    var query = new GetPagedMasterQuery("abc", 3, 7);

        //    // Act
        //    await _handler.Handle(query, CancellationToken.None);

        //    // Assert
        //    _repositoryMock.Verify(r => r.GetFilteredMasterAsync("abc", 3, 7), Times.Once);
        //}
    }
}