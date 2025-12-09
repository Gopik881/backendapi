using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.ApproveCompany.EscalationContacts;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class ApproveCompany5TabEscalationContactCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenRepositoryReturnsTrue()
    {
        var repoMock = new Mock<IEscalationContactsRepository>();
        repoMock.Setup(x => x.Company5TabApproveEscalationContactsDataAsync(
                It.IsAny<int>(),
                It.IsAny<List<Company5TabEscalationContactDto>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>())) // Explicitly include the optional argument
            .ReturnsAsync(true);

        var handler = new ApproveCompany5TabEscalationContactCommandHandler(repoMock.Object);
        var command = new ApproveCompany5TabEscalationContactCommand(1, 2, new List<Company5TabEscalationContactDto>());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenRepositoryReturnsFalse()
    {
        var repoMock = new Mock<IEscalationContactsRepository>();
        repoMock.Setup(x => x.Company5TabApproveEscalationContactsDataAsync(
                It.IsAny<int>(),
                It.IsAny<List<Company5TabEscalationContactDto>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>())) // Explicitly include the optional argument
            .ReturnsAsync(false);

        var handler = new ApproveCompany5TabEscalationContactCommandHandler(repoMock.Object);
        var command = new ApproveCompany5TabEscalationContactCommand(1, 2, new List<Company5TabEscalationContactDto>());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }
}