using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.Commands.UpdateClient.UpdateClient.UpdateClientCompositeCommand;
using Elixir.Application.Interfaces.Persistance;
using MediatR;
using Elixir.Application.Features.Clients.DTOs;
using System.Collections.Generic;
using System;

public class UpdateClientCompostieCommandHandlerTests
{
    private readonly Mock<ITransactionRepository> _transactionRepoMock = new();
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<IClientsRepository> _clientsRepoMock = new();
    private readonly Mock<IClientCompaniesMappingRepository> _clientCompaniesMappingRepoMock = new();
    private readonly Mock<ICompaniesRepository> _companiesRepoMock = new();
    private readonly Mock<IElixirUsersRepository> _usersRepoMock = new();
    private readonly Mock<INotificationsRepository> _notificationsRepoMock = new();

    private UpdateClientCompostieCommandHandler CreateHandler()
    {
        return new UpdateClientCompostieCommandHandler(
            () => _transactionRepoMock.Object,
            _mediatorMock.Object,
            _clientsRepoMock.Object,
            _clientCompaniesMappingRepoMock.Object,
            _companiesRepoMock.Object,
            _usersRepoMock.Object,
            _notificationsRepoMock.Object
        );
    }

    [Fact]
    public async Task Handle_AllUpdatesSucceed_ReturnsTrue()
    {
        // Arrange
        var handler = CreateHandler();
        var dto = new CreateClientDto
        {
            ClientInfo = new ClientInfoDto { ClientId = 1, ClientName = "Test" },
            clientCompanyMappingDtos = new List<ClientCompanyMappingDto> { new() { CompanyId = 1 } },
            ClientAccess = new ClientAccessDto(),
            ClientAdminInfo = new ClientAdminInfoDto(),
            ClientAccountManagers = new List<ClientAccountManagersDto> { new() },
            ClientContactInfo = new List<ClientContactInfoDto> { new() }, // Fixed the type mismatch
            ReportingToolLimits = new ReportingToolLimitsDto()
        };
        var command = new UpdateClientCompositeCommand(1, 1, dto, true);

        _clientsRepoMock.Setup(x => x.GetDistinctClientIdsByCompanyIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<int>());
        _clientsRepoMock.Setup(x => x.GetListOfClientIdsByCompanyNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<int>());
        _transactionRepoMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _mediatorMock.Setup(x => x.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _usersRepoMock.Setup(x => x.ReplaceClientAccountManagersAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<ClientAccountManagersDto>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _transactionRepoMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);
        _companiesRepoMock.Setup(x => x.AddClientAccountManagersAsync(It.IsAny<CreateClientDto>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Handle_UpdateClientFails_ThrowsException()
    {
        // Arrange
        var handler = CreateHandler();
        var dto = new CreateClientDto
        {
            ClientInfo = new ClientInfoDto { ClientId = 1, ClientName = "Test" },
            clientCompanyMappingDtos = new List<ClientCompanyMappingDto> { new() { CompanyId = 1 } }
        };
        var command = new UpdateClientCompositeCommand(1, 1, dto, true);

        _clientsRepoMock.Setup(x => x.GetDistinctClientIdsByCompanyIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<int>());
        _clientsRepoMock.Setup(x => x.GetListOfClientIdsByCompanyNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<int>());
        _transactionRepoMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _mediatorMock.Setup(x => x.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ReplaceClientAccountManagersFails_ThrowsException()
    {
        // Arrange
        var handler = CreateHandler();
        var dto = new CreateClientDto
        {
            ClientInfo = new ClientInfoDto { ClientId = 1, ClientName = "Test" },
            clientCompanyMappingDtos = new List<ClientCompanyMappingDto> { new() { CompanyId = 1 } },
            ClientAccountManagers = new List<ClientAccountManagersDto> { new() }
        };
        var command = new UpdateClientCompositeCommand(1, 1, dto, true);

        _clientsRepoMock.Setup(x => x.GetDistinctClientIdsByCompanyIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<int>());
        _clientsRepoMock.Setup(x => x.GetListOfClientIdsByCompanyNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<int>());
        _transactionRepoMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _mediatorMock.Setup(x => x.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _usersRepoMock.Setup(x => x.ReplaceClientAccountManagersAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<ClientAccountManagersDto>>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AddClientAccountManagersFails_ReturnsFalse()
    {
        // Arrange
        var handler = CreateHandler();
        var dto = new CreateClientDto
        {
            ClientInfo = new ClientInfoDto { ClientId = 1, ClientName = "Test" },
            clientCompanyMappingDtos = new List<ClientCompanyMappingDto> { new() { CompanyId = 1 } },
            ClientAccountManagers = new List<ClientAccountManagersDto> { new() }
        };
        var command = new UpdateClientCompositeCommand(1, 1, dto, true);

        _clientsRepoMock.Setup(x => x.GetDistinctClientIdsByCompanyIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<int>());
        _clientsRepoMock.Setup(x => x.GetListOfClientIdsByCompanyNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<int>());
        _transactionRepoMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _mediatorMock.Setup(x => x.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _usersRepoMock.Setup(x => x.ReplaceClientAccountManagersAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<ClientAccountManagersDto>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _transactionRepoMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);
        _companiesRepoMock.Setup(x => x.AddClientAccountManagersAsync(It.IsAny<CreateClientDto>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_ExceptionDuringTransaction_RollsBackAndThrows()
    {
        // Arrange
        var handler = CreateHandler();
        var dto = new CreateClientDto
        {
            ClientInfo = new ClientInfoDto { ClientId = 1, ClientName = "Test" },
            clientCompanyMappingDtos = new List<ClientCompanyMappingDto> { new() { CompanyId = 1 } }
        };
        var command = new UpdateClientCompositeCommand(1, 1, dto, true);

        _clientsRepoMock.Setup(x => x.GetDistinctClientIdsByCompanyIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<int>());
        _clientsRepoMock.Setup(x => x.GetListOfClientIdsByCompanyNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<int>());
        _transactionRepoMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _mediatorMock.Setup(x => x.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Mediator error"));
        _transactionRepoMock.Setup(x => x.RollbackAsync()).Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        _transactionRepoMock.Verify(x => x.RollbackAsync(), Times.Once);
    }
}