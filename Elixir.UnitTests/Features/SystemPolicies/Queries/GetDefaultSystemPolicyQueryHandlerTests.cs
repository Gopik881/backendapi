using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.SystemPolicies.DTOs;
using Elixir.Application.Features.SystemPolicies.Queries;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.SystemPolicies.Queries
{
    public class GetDefaultSystemPolicyQueryHandlerTests
    {
        private readonly Mock<ISystemPoliciesRepository> _repositoryMock;
        private readonly GetDefaultSystemPolicyQueryHandler _handler;

        public GetDefaultSystemPolicyQueryHandlerTests()
        {
            _repositoryMock = new Mock<ISystemPoliciesRepository>();
            _handler = new GetDefaultSystemPolicyQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsSystemPolicyDto_WhenRepositoryReturnsPolicy()
        {
            // Arrange
            var expectedDto = new SystemPolicyDto
            {
                SystemPolicyId = 1,
                FileSizeLimitMb = 10,
                HistoricalPasswords = 5,
                LockInPeriodInMinutes = 15,
                MaxLength = 20,
                MinLength = 8,
                NoOfLowerCase = 2,
                NoOfSpecialCharacters = 1,
                NoOfUpperCase = 2,
                PasswordValidityDays = 90,
                SessionTimeoutMinutes = 30,
                SpecialCharactersAllowed = "!@#",
                UnsuccessfulAttempts = 3
            };
            _repositoryMock
                .Setup(r => r.GetDefaultSystemPolicyAsync())
                .ReturnsAsync(expectedDto);

            var query = new GetDefaultSystemPolicyQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.SystemPolicyId, result.SystemPolicyId);
            Assert.Equal(expectedDto.FileSizeLimitMb, result.FileSizeLimitMb);
            Assert.Equal(expectedDto.HistoricalPasswords, result.HistoricalPasswords);
            Assert.Equal(expectedDto.LockInPeriodInMinutes, result.LockInPeriodInMinutes);
            Assert.Equal(expectedDto.MaxLength, result.MaxLength);
            Assert.Equal(expectedDto.MinLength, result.MinLength);
            Assert.Equal(expectedDto.NoOfLowerCase, result.NoOfLowerCase);
            Assert.Equal(expectedDto.NoOfSpecialCharacters, result.NoOfSpecialCharacters);
            Assert.Equal(expectedDto.NoOfUpperCase, result.NoOfUpperCase);
            Assert.Equal(expectedDto.PasswordValidityDays, result.PasswordValidityDays);
            Assert.Equal(expectedDto.SessionTimeoutMinutes, result.SessionTimeoutMinutes);
            Assert.Equal(expectedDto.SpecialCharactersAllowed, result.SpecialCharactersAllowed);
            Assert.Equal(expectedDto.UnsuccessfulAttempts, result.UnsuccessfulAttempts);
            _repositoryMock.Verify(r => r.GetDefaultSystemPolicyAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetDefaultSystemPolicyAsync())
                .ReturnsAsync((SystemPolicyDto)null);

            var query = new GetDefaultSystemPolicyQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _repositoryMock.Verify(r => r.GetDefaultSystemPolicyAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetDefaultSystemPolicyAsync())
                .ThrowsAsync(new Exception("Repository error"));

            var query = new GetDefaultSystemPolicyQuery();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            _repositoryMock.Verify(r => r.GetDefaultSystemPolicyAsync(), Times.Once);
        }
    }
}