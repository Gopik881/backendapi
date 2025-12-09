using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Company.DTOs;

public class CreateCompany5TabModuleMappingCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsTrue_WhenRepositoryReturnsTrue()
    {
        var repoMock = new Mock<IModuleMappingHistoryRepository>();
        repoMock.Setup(r => r.Company5TabCreateModuleMappingDataAsync(
            It.IsAny<int>(),
            It.IsAny<List<Company5TabModuleMappingDto>>(),
            It.IsAny<int>(),
            CancellationToken.None)) // Explicitly pass the optional argument
            .ReturnsAsync(true);

        var handler = new CreateCompany5TabModuleMappingCommandHandler(repoMock.Object);
        var command = new CreateCompany5TabModuleMappingCommand(1, 2, new List<Company5TabModuleMappingDto>());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ReturnsFalse_WhenRepositoryReturnsFalse()
    {
        var repoMock = new Mock<IModuleMappingHistoryRepository>();
        repoMock.Setup(r => r.Company5TabCreateModuleMappingDataAsync(
            It.IsAny<int>(),
            It.IsAny<List<Company5TabModuleMappingDto>>(),
            It.IsAny<int>(),
            CancellationToken.None)) // Explicitly pass the optional argument
            .ReturnsAsync(false);

        var handler = new CreateCompany5TabModuleMappingCommandHandler(repoMock.Object);
        var command = new CreateCompany5TabModuleMappingCommand(1, 2, new List<Company5TabModuleMappingDto>());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }
}