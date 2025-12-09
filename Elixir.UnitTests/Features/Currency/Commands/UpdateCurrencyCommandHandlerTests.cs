using Elixir.Application.Features.Currency.Commands.UpdateCurrency;
using Elixir.Application.Features.Currency.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Elixir.Domain.Entities;
using Moq;

namespace Elixir.UnitTests.Features.Currency.Commands
{
    public class UpdateCurrencyCommandHandlerTests
    {
        private readonly Mock<ICurrencyMasterRepository> _repositoryMock;
        private readonly Mock<ICountryMasterRepository> _countryRepositoryMock;
        private readonly Mock<IEntityReferenceService> _entityReferenceServiceMock; // Add mock for IEntityReferenceService
        private readonly UpdateCurrencyCommandHandler _handler;

        public UpdateCurrencyCommandHandlerTests()
        {
            _repositoryMock = new Mock<ICurrencyMasterRepository>();
            _countryRepositoryMock = new Mock<ICountryMasterRepository>();
            _entityReferenceServiceMock = new Mock<IEntityReferenceService>(); // Initialize the mock for IEntityReferenceService
            _handler = new UpdateCurrencyCommandHandler(
                _repositoryMock.Object,
                _countryRepositoryMock.Object,
                _entityReferenceServiceMock.Object // Pass the required parameter
            );
        }
    }
}