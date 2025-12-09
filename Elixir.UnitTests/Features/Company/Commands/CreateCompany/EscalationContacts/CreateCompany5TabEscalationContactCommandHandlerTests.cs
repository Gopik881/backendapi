using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Company.DTOs;

public class CreateCompany5TabEscalationContactCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsTrue_WhenRepositoryReturnsTrue()
    {
        var repoMock = new Mock<IEscalationContactsHistoryRepository>();
        repoMock.Setup(r => r.Company5TabCreateEscalationContactsDataAsync(
            It.IsAny<int>(),
            It.IsAny<List<Company5TabEscalationContactDto>>(),
            It.IsAny<int>(),
            CancellationToken.None)) // Explicitly pass the CancellationToken argument
            .ReturnsAsync(true);

        var handler = new CreateCompany5TabEscalationContactCommandHandler(repoMock.Object);
        var command = new CreateCompany5TabEscalationContactCommand(1, 2, new List<Company5TabEscalationContactDto>());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ReturnsFalse_WhenRepositoryReturnsFalse()
    {
        var repoMock = new Mock<IEscalationContactsHistoryRepository>();
        repoMock.Setup(r => r.Company5TabCreateEscalationContactsDataAsync(
            It.IsAny<int>(),
            It.IsAny<List<Company5TabEscalationContactDto>>(),
            It.IsAny<int>(),
            CancellationToken.None)) // Explicitly pass the CancellationToken argument
            .ReturnsAsync(false);

        var handler = new CreateCompany5TabEscalationContactCommandHandler(repoMock.Object);
        var command = new CreateCompany5TabEscalationContactCommand(1, 2, new List<Company5TabEscalationContactDto>());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }
}