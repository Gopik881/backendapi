using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.Commands.DeleteClient.DeleteClientComposite;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;
using Moq;
using Xunit;

public class DeleteClientCompositeCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Commit_Transaction_When_All_Deletes_Succeed()
    {
        var transactionMock = new Mock<ITransactionRepository>();
        transactionMock.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
        transactionMock.Setup(t => t.CommitAsync()).Returns(Task.CompletedTask);

        var transactionFactory = new Func<ITransactionRepository>(() => transactionMock.Object);

        var clientsRepoMock = new Mock<IClientsRepository>();
        clientsRepoMock.Setup(r => r.GetClientDetailsByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new ClientInfoDto { ClientName = "TestClient" }); // Correctly instantiate ClientInfoDto
        clientsRepoMock.Setup(r => r.GetListOfClientIdsByCompanyNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<int> { 1 });

        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new DeleteClientCompositeCommandHandler(transactionFactory, clientsRepoMock.Object, mediatorMock.Object);

        var result = await handler.Handle(new DeleteClientCompositeCommand(1), CancellationToken.None);

        Assert.True(result);
        transactionMock.Verify(t => t.CommitAsync(), Times.Once);
        transactionMock.Verify(t => t.RollbackAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Rollback_And_Throw_When_Client_Not_Found()
    {
        var transactionMock = new Mock<ITransactionRepository>();
        transactionMock.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
        transactionMock.Setup(t => t.RollbackAsync()).Returns(Task.CompletedTask);

        var transactionFactory = new Func<ITransactionRepository>(() => transactionMock.Object);

        var clientsRepoMock = new Mock<IClientsRepository>();
        clientsRepoMock.Setup(r => r.GetClientDetailsByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((ClientInfoDto)null); // Explicitly cast null to ClientInfoDto

        var mediatorMock = new Mock<IMediator>();

        var handler = new DeleteClientCompositeCommandHandler(transactionFactory, clientsRepoMock.Object, mediatorMock.Object);

        await Assert.ThrowsAsync<Exception>(() => handler.Handle(new DeleteClientCompositeCommand(1), CancellationToken.None));
        transactionMock.Verify(t => t.RollbackAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Rollback_And_Throw_When_Any_Delete_Fails()
    {
        var transactionMock = new Mock<ITransactionRepository>();
        transactionMock.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
        transactionMock.Setup(t => t.RollbackAsync()).Returns(Task.CompletedTask);

        var transactionFactory = new Func<ITransactionRepository>(() => transactionMock.Object);

        var clientsRepoMock = new Mock<IClientsRepository>();
        clientsRepoMock.Setup(r => r.GetClientDetailsByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new ClientInfoDto { ClientName = "TestClient" }); // Correctly instantiate ClientInfoDto
        clientsRepoMock.Setup(r => r.GetListOfClientIdsByCompanyNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<int> { 1 });

        var mediatorMock = new Mock<IMediator>();
        mediatorMock.SetupSequence(m => m.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false); // Simulate a failure

        var handler = new DeleteClientCompositeCommandHandler(transactionFactory, clientsRepoMock.Object, mediatorMock.Object);

        await Assert.ThrowsAsync<Exception>(() => handler.Handle(new DeleteClientCompositeCommand(1), CancellationToken.None));
        transactionMock.Verify(t => t.RollbackAsync(), Times.Once);
    }
}