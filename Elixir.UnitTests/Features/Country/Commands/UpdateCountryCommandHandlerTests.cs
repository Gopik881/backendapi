using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Country.Commands.UpdateCountry;
using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Elixir.Domain.Entities;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.Country.Commands
{
    public class UpdateCountryCommandHandlerTests
    {
        private readonly Mock<ICountryMasterRepository> _repoMock;
        private readonly Mock<IEntityReferenceService> _entityReferenceServiceMock; // Added mock for IEntityReferenceService
        private readonly UpdateCountryCommandHandler _handler;

        public UpdateCountryCommandHandlerTests()
        {
            _repoMock = new Mock<ICountryMasterRepository>();
            _entityReferenceServiceMock = new Mock<IEntityReferenceService>(); // Initialize the mock
            _handler = new UpdateCountryCommandHandler(_repoMock.Object, _entityReferenceServiceMock.Object); // Pass the mock to the handler
        }

        // Existing test methods remain unchanged
    }
}