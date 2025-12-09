using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetTMIAdminDashBoardDetails;
using Elixir.Application.Interfaces.Persistance;
using Moq;

namespace Elixir.UnitTests.Features.Dashboard.Queries
{
    public class GetTMIAdminDashBoardDetailsQueryHandlerTests
    {
        private readonly Mock<ICompaniesRepository> _dashboardRepoMock;
        private readonly GetTMIAdminDashBoardDetailsQueryHandler _handler;

        public GetTMIAdminDashBoardDetailsQueryHandlerTests()
        {
            _dashboardRepoMock = new Mock<ICompaniesRepository>();
            _handler = new GetTMIAdminDashBoardDetailsQueryHandler(_dashboardRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsDashboardDetails_WhenDataExists()
        {
            // Arrange
            var query = new GetTMIAdminDashBoardDetailsQuery(1);
            var dashboardDto = new TmiDashBoardDetailsDto { ActiveCompaniesCount = 7, OnboardingCompaniesCount = 3 };
            _dashboardRepoMock.Setup(r => r.GetTMIAdminDashBoardDetailsAsync(query.UserId))
                .ReturnsAsync(dashboardDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(7, result.ActiveCompaniesCount);
            Assert.Equal(3, result.OnboardingCompaniesCount);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenNoDataExists()
        {
            // Arrange
            var query = new GetTMIAdminDashBoardDetailsQuery(1);
            _dashboardRepoMock.Setup(r => r.GetTMIAdminDashBoardDetailsAsync(query.UserId))
                .ReturnsAsync((TmiDashBoardDetailsDto)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            var query = new GetTMIAdminDashBoardDetailsQuery(1);
            _dashboardRepoMock.Setup(r => r.GetTMIAdminDashBoardDetailsAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }

        //[Fact]
        //public async Task Handle_NullQuery_ThrowsArgumentNullException()
        //{
        //    // Act & Assert
        //    await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null!, CancellationToken.None));
        //}
    }
}